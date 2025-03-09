using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BuildModule.Scripts.Runtime.AssetManager
{
    public class ResourcesLoad : IAssetLoad
    {
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="assetLoadResult"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerator AsyLoadAsset<T>(string rootFolder, string assetBundleName, string assetName,
            AssetLoadResult<T> assetLoadResult)
            where T : Object
        {
            assetLoadResult.AssetObject = null;
            if (string.IsNullOrEmpty(rootFolder) || string.IsNullOrEmpty(assetBundleName) ||
                string.IsNullOrEmpty(assetName))
            {
                Debug.LogError(
                    $"ResourcesLoad.AsyLoadAsset error:exist any empty path, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName}!");
                yield break;
            }

            string resourcesFolder = Regex.Match(assetBundleName, @"^assets\.(resources\.)?(?<folder>.+)\.assetbundle$",
                    RegexOptions.IgnoreCase)
                .Groups["folder"].Value.Replace(".", "/");
            if (string.IsNullOrEmpty(resourcesFolder))
            {
                Debug.LogError(
                    $"ResourcesLoad.AsyLoadAsset error resourcesFolder is empty, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName}!");
                yield break;
            }
            var result = Resources.LoadAsync<T>($"{resourcesFolder}/{assetName}");
            yield return result;
            assetLoadResult.AssetObject = result.asset as T;
            if (assetLoadResult.AssetObject) yield break;
            Debug.LogError(
                $"ResourcesLoad.LoadManifest read asset error, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName}!");
        }
    }
}