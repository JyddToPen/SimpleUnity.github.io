#if UNITY_EDITOR
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BuildModule.Scripts.Runtime.AssetManager
{
    public class EditorLoad : IAssetLoad
    {
        /// <summary>
        /// 加载资源
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
            if (string.IsNullOrEmpty(assetBundleName) ||
                string.IsNullOrEmpty(assetName) || string.IsNullOrEmpty(extension))
            {
                Debug.LogError(
                    $"EditorLoad.AsyLoadAsset error:exist any empty path, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName} extension:{extension}!");
                yield break;
            }

            string assetPath =
                $"{assetBundleName.ToLower().Replace("assetbundle", "").Replace(".", "/")}{assetName}{extension}";
            assetLoadResult.AssetObject = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (assetLoadResult.AssetObject)
            {
                Debug.Log(
                    $"EditorLoad.LoadManifest read asset success, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName}!");
            }
            else
            {
                Debug.LogError(
                    $"EditorLoad.LoadManifest read asset error, rootFolder:{rootFolder} assetBundleName:{assetBundleName} assetName:{assetName}!");
            }
        }
    }
}
#endif