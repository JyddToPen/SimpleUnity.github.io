using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLib.Scripts.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace BuildModule.Scripts.Runtime.AssetManager
{
    /// <summary>
    /// 加载状态
    /// </summary>
    internal enum AssetBundleLoadStage
    {
        /// <summary>
        /// 闲置
        /// </summary>
        None,

        /// <summary>
        /// 加载
        /// </summary>
        Loading,

        /// <summary>
        /// 完成
        /// </summary>
        Complete
    }

    /// <summary>
    /// 加载请求
    /// </summary>
    internal class LoadRequest
    {
        /// <summary>
        /// 根目录
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// ab包名
        /// </summary>
        public string AssetBundleName { get; set; }

        /// <summary>
        /// 加载状态
        /// </summary>
        public AssetBundleLoadStage AssetBundleLoadStage { get; set; }

        /// <summary>
        /// ab包
        /// </summary>
        public AssetBundle AssetBundle { get; set; }

        /// <summary>
        /// 完整度
        /// </summary>
        public bool DependencyFull { get; set; }
    }

    public class AssetBundleLoad : IAssetLoad
    {
        private readonly Dictionary<string, LoadRequest> _abFileLoadRequests = new();
        private readonly Dictionary<string, AssetBundleManifest> _assetBundleManifests = new();


        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="extension"></param>
        /// <param name="assetLoadResult"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerator AsyLoadAsset<T>(string rootFolder, string assetBundleName, string assetName,
            string extension,
            AssetLoadResult<T> assetLoadResult)
            where T : Object
        {
            assetLoadResult.AssetObject = null;
            if (string.IsNullOrEmpty(rootFolder) || string.IsNullOrEmpty(assetBundleName) ||
                string.IsNullOrEmpty(assetName))
            {
                Debug.LogError(
                    $"AssetBundleLoad.AsyLoadAsset error:exist any empty path, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName}!");
                yield break;
            }

            assetBundleName = assetBundleName.ToLower();
            rootFolder = rootFolder.ToLower();
            MonoUtility.Instance.UpdateLoopEvent -= LoadFileLoop;
            MonoUtility.Instance.UpdateLoopEvent += LoadFileLoop;

            yield return LoadManifest(rootFolder);

            if (!_assetBundleManifests.TryGetValue(rootFolder, out var manifest))
            {
                Debug.LogError(
                    $"AssetBundleLoad.AsyLoadAsset error, AssetBundleManifest error,AssetBundleManifest = null, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName}!");
                yield break;
            }

            yield return LoadDependencies(manifest, rootFolder, assetBundleName);
            if (!_abFileLoadRequests.TryGetValue(assetBundleName, out var request) || !request.AssetBundle)
            {
                Debug.LogError(
                    $"AssetBundleLoad.AsyLoadAsset error, assetBundle read error, rootFolder:{rootFolder}  assetBundleName:{assetBundleName} assetName:{assetName}!");
                yield break;
            }

            var result = request.AssetBundle.LoadAssetAsync<T>(assetName);
            yield return result;
            assetLoadResult.AssetObject = result.asset as T;
            if (assetLoadResult.AssetObject)
            {
                Debug.Log(
                    $"AssetBundleLoad.AsyLoadAsset success, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName}!");
            }
            else
            {
                Debug.LogError(
                    $"AssetBundleLoad.AsyLoadAsset error, read Asset error, rootFolder:{rootFolder}  assetBundleName:{assetBundleName} assetName:{assetName}!");
            }
        }

        /// <summary>
        /// 加载manifest
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <returns></returns>
        private IEnumerator LoadManifest(string rootFolder)
        {
            if (_assetBundleManifests.ContainsKey(rootFolder))
            {
                yield break;
            }

            RequestLoadFile(rootFolder, rootFolder);
            while (!IsLoadComplete(rootFolder))
            {
                yield return null;
            }

            if (_assetBundleManifests.ContainsKey(rootFolder))
            {
                yield break;
            }

            if (!_abFileLoadRequests.TryGetValue(rootFolder, out var request) || !request.AssetBundle) yield break;
            var result = request.AssetBundle.LoadAssetAsync<AssetBundleManifest>(nameof(AssetBundleManifest));
            yield return result;
            if (!result.asset)
            {
                Debug.LogError(
                    $"AssetBundleLoad.LoadManifest read AssetBundleManifest error,rootFolder:{rootFolder}!");
                yield break;
            }

            if (!_assetBundleManifests.ContainsKey(rootFolder))
            {
                _assetBundleManifests.Add(rootFolder, result.asset as AssetBundleManifest);
            }
        }

        /// <summary>
        /// 加载依赖
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="rootFolder"></param>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        private IEnumerator LoadDependencies(AssetBundleManifest manifest, string rootFolder,
            string assetBundleName)
        {
            if (!manifest)
            {
                Debug.LogError(
                    $"AssetBundleLoad.LoadDependencies error:manifest = null!");
                yield break;
            }

            if (_abFileLoadRequests.TryGetValue(assetBundleName, out var request) && request.DependencyFull)
            {
                yield break;
            }

            string[] allDependencies = manifest.GetAllDependencies(assetBundleName);
            foreach (var dependency in allDependencies)
            {
                RequestLoadFile(rootFolder, dependency);
            }

            while (allDependencies.Any(item => !IsLoadComplete(item)))
            {
                yield return null;
            }

            RequestLoadFile(rootFolder, assetBundleName);
            while (!IsLoadComplete(assetBundleName))
            {
                yield return null;
            }

            _abFileLoadRequests[assetBundleName].DependencyFull = true;
        }

        #region 从文件加载

        /// <summary>
        /// 加载循环
        /// </summary>
        private void LoadFileLoop()
        {
            if (_abFileLoadRequests.Count <= 0)
            {
                return;
            }

            foreach (var loadRequest in _abFileLoadRequests)
            {
                if (loadRequest.Value.AssetBundleLoadStage != AssetBundleLoadStage.None) continue;
                MonoUtility.Instance.StartCoroutine(LoadSingleAssetBundle(loadRequest.Value.RootFolder,
                    loadRequest.Value.AssetBundleName));
                loadRequest.Value.AssetBundleLoadStage = AssetBundleLoadStage.Loading;
            }
        }


        /// <summary>
        /// 是否加载完
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        private bool IsLoadComplete(string assetBundleName)
        {
            return _abFileLoadRequests.TryGetValue(assetBundleName, out var request) &&
                   request.AssetBundleLoadStage == AssetBundleLoadStage.Complete;
        }

        /// <summary>
        /// 请求加载文件
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        private void RequestLoadFile(string rootFolder, string assetBundleName)
        {
            _abFileLoadRequests.TryAdd(assetBundleName, new LoadRequest()
            {
                RootFolder = rootFolder,
                AssetBundleName = assetBundleName
            });
        }

        /// <summary>
        /// 加载单个ab
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        private IEnumerator LoadSingleAssetBundle(string rootFolder, string assetBundleName)
        {
            if (IsLoadComplete(assetBundleName)) yield break;
            string targetName = "";
#if UNITY_EDITOR
            targetName = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup.ToString();
#else
            EnvConfig.Instance.RuntimePlatformMaps.TryGetValue(Application.platform, out targetName);
#endif
            if (string.IsNullOrEmpty(targetName))
            {
                Debug.LogError(
                    $"AssetBundleLoad.LoadSingleAssetBundle read error:targetName related of {Application.platform} is null!");
                yield break;
            }

            var abFullUrl = $"{Application.streamingAssetsPath}/{targetName}/{rootFolder}/{assetBundleName}";
            var unityWebRequest = UnityWebRequest.Get(abFullUrl);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    $"AssetBundleLoad.LoadSingleAssetBundle read error:{unityWebRequest.error}, assetBundleName:{assetBundleName} rootFolder:{rootFolder}!");
                yield break;
            }

            var assetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(unityWebRequest.downloadHandler.data);
            yield return assetBundleCreateRequest;
            if (!assetBundleCreateRequest.assetBundle)
            {
                Debug.LogError(
                    $"AssetBundleLoad.LoadSingleAssetBundle load ab error, assetBundleName:{assetBundleName} rootFolder:{rootFolder}!");
                yield break;
            }

            if (_abFileLoadRequests.TryGetValue(assetBundleName, out var request) &&
                request.AssetBundleLoadStage == AssetBundleLoadStage.Complete) yield break;
            request ??= new LoadRequest()
            {
                RootFolder = rootFolder,
                AssetBundleName = assetBundleName,
            };
            request.AssetBundleLoadStage = AssetBundleLoadStage.Complete;
            request.AssetBundle = assetBundleCreateRequest.assetBundle;
            _abFileLoadRequests[assetBundleName] = request;
        }

        #endregion
    }
}