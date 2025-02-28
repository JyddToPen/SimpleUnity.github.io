using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildModule.Scripts.Editor
{
    public abstract class PlatformBuild
    {
        /// <summary>
        /// 输出目录
        /// </summary>
        protected abstract string TargetFolder { get; }

        /// <summary>
        /// cdn配置读取文件
        /// </summary>
        protected abstract string CdnReadFilePath { get; }

        /// <summary>
        /// 是否优先选用本地缓存
        /// </summary>
        protected bool IsPreferLocalCdn { get; set; }

        /// <summary>
        /// 本地cdn物理地址
        /// </summary>
        protected string LocalCdnPhysicalPath { get; set; }

        /// <summary>
        /// 本地cdn访问地址
        /// </summary>
        protected string LocalCdnRemotePath { get; set; }

        /// <summary>
        /// 执行打包
        /// </summary>
        public void HandleBuild(BuildOptions buildOptions)
        {
            PreBuild();
            OnProcessBuild(buildOptions);
        }

        /// <summary>
        /// 预处理
        /// </summary>
        protected virtual void PreBuild()
        {
            PlayerSettings.SetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), "");

            LocalCdnPhysicalPath = null;
            IsPreferLocalCdn = false;
            if (!File.Exists(CdnReadFilePath))
            {
                Debug.LogError($"本地CDN配置文件不存在, file:{CdnReadFilePath}。回退到remote模式！！！");
                return;
            }

            //0 LocalCdnPhysicalPath   \\0.0.0.0\SimpleGame\Wx
            //1 LocalCdnRemotePath     http://0.0.0.0/SimpleGame/Wx
            string[] sharedCdnConfigs = File.ReadAllLines(CdnReadFilePath);
            if (sharedCdnConfigs.Length < 2)
            {
                Debug.LogError($"本地CDN配置文件格式不正确, file:{CdnReadFilePath}。回退到remote模式！！！");
                return;
            }

            //优先使用本地服务
            string sharedRoot = Path.GetDirectoryName(sharedCdnConfigs[0]);
            bool isExistShared = Directory.Exists(sharedRoot);
            if (!isExistShared)
            {
                Debug.LogError($"本地CDN配置的物理地址根节点不存在, file:{CdnReadFilePath} sharedRoot:{sharedRoot}。回退到remote模式！！！");
                return;
            }

            if (Directory.Exists(sharedCdnConfigs[0]))
            {
                //安全期间，本地的cdn文件不做删除操作，此动作很危险
                //Directory.Delete(sharedCdnConfigs[0], true);
            }

            Directory.CreateDirectory(sharedCdnConfigs[0]);
            LocalCdnPhysicalPath = sharedCdnConfigs[0];
            LocalCdnRemotePath = sharedCdnConfigs[1];
            IsPreferLocalCdn = true;
        }

        /// <summary>
        /// 处理构建
        /// </summary>
        protected virtual void OnProcessBuild(BuildOptions buildOptions)
        {
            string selectScene = string.Empty;
            if (Selection.activeObject)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (assetPath.StartsWith("Assets/") && assetPath.EndsWith(".unity"))
                {
                    selectScene = assetPath;
                }
            }

            if (string.IsNullOrEmpty(selectScene))
            {
                throw new Exception("请选择一个构建用的启动场景文件！！！");
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { selectScene },
                locationPathName = Application.dataPath + $"/../{TargetFolder}",
                target = EditorUserBuildSettings.activeBuildTarget,
                options = buildOptions,
                extraScriptingDefines = new string[] { }
            };
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes！！");
                PostBuild(summary.outputPath);
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("Build failed！");
            }
        }

        /// <summary>
        /// 后处理
        /// </summary>
        protected virtual void PostBuild(string outputPath)
        {
        }
    }
}