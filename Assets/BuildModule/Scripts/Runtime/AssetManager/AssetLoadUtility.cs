using System.Text.RegularExpressions;
using UnityEngine;

namespace BuildModule.Scripts.Runtime.AssetManager
{
    public struct ParseAssetUrlResult
    {
        public string AssetBundle { get; set; }
        public string RootFolder { get; set; }
        public string ResourcesPath { get; set; }
    }

    public static class AssetLoadUtility
    {
        /// <summary>
        /// 解析路径
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static ParseAssetUrlResult ParseAssetUrl(string assetBundleName,
            string assetName)
        {
            if (string.IsNullOrEmpty(assetBundleName) || string.IsNullOrEmpty(assetName))
            {
                Debug.LogError(
                    $"AssetLoadUtility.ParseAssetUrl error:exist any empty path, assetBundleName:{assetBundleName} assetName:{assetName}!");
                return default;
            }

            string[] assetBundleNameParis = assetBundleName.Split(";");
            if (assetBundleNameParis.Length < 2)
            {
                Debug.LogError(
                    $"AssetLoadUtility.ParseAssetUrl error:assetBundleNameParis not enough, assetBundleName:{assetBundleName} assetName:{assetName}!");
                return default;
            }

            string realAssetBundleName = assetBundleNameParis[1];
            string rootFolder = assetBundleNameParis[0];

            string resourcesFolder = Regex.Match(assetBundleName, @"^assets\.(resources\.)?(?<folder>.+)\.assetbundle$",
                    RegexOptions.IgnoreCase)
                .Groups["folder"].Value.Replace(".", "/");
            return new ParseAssetUrlResult()
            {
                AssetBundle = realAssetBundleName,
                RootFolder = rootFolder,
                ResourcesPath = resourcesFolder
            };
        }
    }
}