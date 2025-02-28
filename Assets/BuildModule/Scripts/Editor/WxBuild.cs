using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using WeChatWASM;

namespace BuildModule.Scripts.Editor
{
    public class WxBuild : PlatformBuild
    {
        protected override string TargetFolder => "Player2Wx";
        protected override string CdnReadFilePath => $"{Application.dataPath}/../Library/WxCdn";

        /// <summary>
        /// 导出前配置
        /// </summary>
        private static string _beforeCdn;

        /// <summary>
        /// 导出前
        /// </summary>
        protected override void PreBuild()
        {
            base.PreBuild();
            PlayerSettings.SetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup),
                "ENABLE_WECHAT");
            _beforeCdn = WXConvertCore.config.ProjectConf.CDN;
            if (IsPreferLocalCdn)
            {
                WXConvertCore.config.ProjectConf.CDN = LocalCdnRemotePath;
            }
        }

        /// <summary>
        /// 导出后
        /// </summary>
        protected override void PostBuild(string outputPath)
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
                File.Copy(dataFiles[0], $"{IsPreferLocalCdn}/{Path.GetFileName(dataFiles[0])}", true);
            }

            Debug.Log("微信构建的data资源已经上传至共享盘！");
        }

        /// <summary>
        /// 处理打包
        /// </summary>
        protected override void OnProcessBuild(BuildOptions buildOptions = BuildOptions.None)
        {
            WXConvertCore.WXExportError exportError = WXConvertCore.DoExport();
            if (exportError == WXConvertCore.WXExportError.SUCCEED)
            {
                Debug.Log("微信小游戏导出成功！");
                PostBuild(null);
            }
            else
            {
                Debug.LogError("微信小游戏导出失败!");
            }

            if (IsPreferLocalCdn && !string.IsNullOrEmpty(_beforeCdn))
            {
                WXConvertCore.config.ProjectConf.CDN = _beforeCdn;
            }

            EditorUtility.SetDirty(WXConvertCore.config);
            AssetDatabase.SaveAssetIfDirty(WXConvertCore.config);
        }
    }
}