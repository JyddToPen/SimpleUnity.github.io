using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BuildModule.Scripts.Editor.AssetBundle
{
    public static class EditorAssetBundleUtility
    {
        
        /// <summary>
        /// 获取根目录
        /// </summary>
        /// <param name="masterAsset"></param>
        /// <returns></returns>
        public static string GetRootFolder(string masterAsset)
        {
            string rootFolder = Regex.Match(masterAsset, @"^Assets/(Resources/(?<root>\w+)|(?<root>\w+))")
                .Groups["root"].Value;
            return rootFolder;
        }

        /// <summary>
        /// 获取ab输出路径
        /// </summary>
        /// <param name="masterAsset"></param>
        /// <returns></returns>
        public static string AssetBundleOutput(string masterAsset)
        {
            string platformName = EditorUserBuildSettings.selectedBuildTargetGroup.ToString().ToLower();

            string outputPath =
                $"{Application.streamingAssetsPath}/{platformName}/{GetRootFolder(masterAsset).ToLower()}";
            return outputPath;
        }
    }
}