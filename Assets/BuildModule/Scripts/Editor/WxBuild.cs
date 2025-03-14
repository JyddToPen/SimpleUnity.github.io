using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeChatWASM;

namespace BuildModule.Scripts.Editor
{
    public class WxBuild : PlatformBuild
    {
        protected override string TargetFolder => "Player2Wx";
        protected override string CdnReadFilePath => $"{Application.dataPath}/../Library/WxCdn";
        private string[] _selectScenes;

        /// <summary>
        /// 导出前配置
        /// </summary>
        private static string _beforeCdn;

        /// <summary>
        /// 导出前
        /// </summary>
        protected override void PreBuild(BuildOptions buildOptions, string[] bootScenes)
        {
            base.PreBuild(buildOptions, bootScenes);
            PlayerSettings.SetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup),
                "ENABLE_WECHAT");
            _beforeCdn = WXConvertCore.config.ProjectConf.CDN;
            if (IsPreferLocalCdn)
            {
                WXConvertCore.config.ProjectConf.CDN = LocalCdnRemotePath;
            }

            WXConvertCore.config.CompileOptions.DevelopBuild =
                (buildOptions & BuildOptions.Development) == BuildOptions.Development;
            WXConvertCore.config.CompileOptions.AutoProfile =
                (buildOptions & BuildOptions.ConnectWithProfiler) == BuildOptions.ConnectWithProfiler;
            _selectScenes = EditorSceneManager.GetSceneManagerSetup().Select(item => item.path).ToArray();
            EditorSceneManager.RestoreSceneManagerSetup(bootScenes?.Select(item => new SceneSetup() { path = item })
                .ToArray());
        }

        /// <summary>
        /// 导出后
        /// </summary>
        protected override void PostBuild(string outputPath, string[] bootScenes)
        {
            if (!IsPreferLocalCdn)
            {
                return;
            }

            string[] dataFiles =
                Directory.GetFiles($"{Application.dataPath}/../{WXConvertCore.webglDir}",
                    "*.webgl.data.unityweb.bin.br");
            if (dataFiles.Length > 0)
            {
                File.Copy(dataFiles[0], $"{LocalCdnPhysicalPath}/{Path.GetFileName(dataFiles[0])}", true);
            }

            Debug.Log("微信构建的data资源已经上传至共享盘！");
        }

        /// <summary>
        /// 处理打包
        /// </summary>
        protected override void OnProcessBuild(BuildOptions buildOptions, string[] bootScenes)
        {
            WXConvertCore.WXExportError exportError = WXConvertCore.DoExport();
            if (exportError == WXConvertCore.WXExportError.SUCCEED)
            {
                Debug.Log("微信小游戏导出成功！");
                PostBuild(null, bootScenes);
            }
            else
            {
                Debug.LogError("微信小游戏导出失败!");
            }

            if (IsPreferLocalCdn && !string.IsNullOrEmpty(_beforeCdn))
            {
                WXConvertCore.config.ProjectConf.CDN = _beforeCdn;
            }

            EditorSceneManager.RestoreSceneManagerSetup(_selectScenes?.Select(item => new SceneSetup() { path = item })
                .ToArray());
            EditorUtility.SetDirty(WXConvertCore.config);
            AssetDatabase.SaveAssetIfDirty(WXConvertCore.config);
        }
    }
}