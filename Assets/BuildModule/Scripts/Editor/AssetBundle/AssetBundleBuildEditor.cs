using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BuildModule.Scripts.Editor.AssetBundle
{
    public static class AssetBundleBuildEditor
    {
        private const string ForbidAb = "NoAbLabel";

        [MenuItem("Assets/Build/NoAbLabel")]
        private static void NoAbLabel()
        {
            AssetDatabase.SetLabels(Selection.activeObject, new string[] { ForbidAb });
        }

        [MenuItem("Assets/Build/AssetBundle")]
        private static void BuildAssetBundle()
        {
            string selectAsset = AssetDatabase.GetAssetPath(Selection.activeObject);
            string rootFolder = Regex.Match(selectAsset, @"^Assets/(Resources/(?<root>\w+)|(?<root>\w+))")
                .Groups["root"].Value;
            if (string.IsNullOrEmpty(rootFolder))
            {
                Debug.LogError("选择的文件没有根目录!");
                return;
            }

            string outputPath = $"{Application.streamingAssetsPath}/{rootFolder}";
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            Dictionary<string, HashSet<string>> construction = ConstructAbBuildItems();
            if (construction.Count == 0)
            {
                Debug.LogError("Build AssetBundle failed, construction count = 0 !!!");
                return;
            }

            var result = BuildPipeline.BuildAssetBundles(outputPath, construction.Select(item => new AssetBundleBuild()
            {
                assetBundleName = item.Key,
                assetNames = item.Value.ToArray()
            })?.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            if (result)
            {
                Debug.Log("Build AssetBundle succeeded!!!");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Build AssetBundle failed!!!");
            }
        }

        /// <summary>
        /// 获取ab构建条目
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, HashSet<string>> ConstructAbBuildItems()
        {
            string selectAsset = AssetDatabase.GetAssetPath(Selection.activeObject);
            string[] allDependencies = AssetDatabase.GetDependencies(selectAsset);
            List<string> allAssetUrls = allDependencies.ToList();
            allAssetUrls.Add(selectAsset);
            allAssetUrls.AddRange(Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath));
            Dictionary<string, HashSet<string>> rootFolders = new Dictionary<string, HashSet<string>>();
            foreach (var dependency in allAssetUrls)
            {
                string abName = GetAbName(dependency);
                if (string.IsNullOrEmpty(abName))
                {
                    continue;
                }

                if (!rootFolders.TryGetValue(abName, out var assetSet))
                {
                    assetSet = new HashSet<string>();
                    rootFolders.Add(abName, assetSet);
                }

                assetSet.Add(dependency);
            }

            return rootFolders;
        }

        /// <summary>
        /// 获取ab名
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <returns></returns>
        private static string GetAbName(string assetUrl)
        {
            if (string.IsNullOrEmpty(assetUrl))
            {
                return null;
            }

            if (!assetUrl.StartsWith("Assets/") || assetUrl.EndsWith(".cs"))
            {
                return null;
            }

            if (AssetDatabase.GetLabels(AssetDatabase.LoadAssetAtPath<Object>(assetUrl))
                .Any(item => item == ForbidAb))
            {
                return null;
            }

            string folder = Path.GetDirectoryName(assetUrl);
            if (string.IsNullOrEmpty(folder))
            {
                Debug.LogError("选择的文件没有根目录!");
                return null;
            }

            return $"{folder.Replace("\\", "/").Replace("/", ".").ToLower()}.assetbundle";
        }
    }
}