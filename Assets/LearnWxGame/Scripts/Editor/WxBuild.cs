using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;
using WeChatWASM;

namespace LearnWxGame.Scripts.Editor
{
    public static class WxBuild
    {
        /// <summary>
        /// 本地共享位置
        /// </summary>
        private static string _localSharedPath;

        /// <summary>
        /// 导出前配置
        /// </summary>
        private static string _beforeCdn;

        /// <summary>
        /// 导出前
        /// </summary>
        private static void BeforeExport()
        {
            PlayerSettings.SetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup),
                "ENABLE_WECHAT");
            _localSharedPath = null;
            _beforeCdn = null;
            string sharedCdnConfigFile = $"{Application.dataPath}/../Library/WxCdn";
            if (!File.Exists(sharedCdnConfigFile))
            {
                return;
            }

            //1 \\0.0.0.0\SimpleGame\Wx
            //2 http://0.0.0.0/SimpleGame/Wx
            string[] sharedCdnConfigs = File.ReadAllLines(sharedCdnConfigFile);
            if (sharedCdnConfigs.Length < 2)
            {
                return;
            }

            //优先使用本地服务
            string sharedDiver = sharedCdnConfigs[0];
            string sharedRoot = Path.GetDirectoryName(sharedDiver);
            bool isExistShared = Directory.Exists(sharedRoot);
            if (!isExistShared)
            {
                return;
            }

            if (Directory.Exists(sharedDiver))
            {
                Directory.Delete(sharedDiver, true);
            }

            Directory.CreateDirectory(sharedDiver);
            _beforeCdn = WXConvertCore.config.ProjectConf.CDN;
            WXConvertCore.config.ProjectConf.CDN = sharedCdnConfigs[1];
            _localSharedPath = sharedDiver;
        }

        /// <summary>
        /// 导出后
        /// </summary>
        private static void AfterExport()
        {
            if (string.IsNullOrEmpty(_localSharedPath))
            {
                return;
            }

            string[] dataFiles =
                Directory.GetFiles($"{Application.dataPath}/../{WXConvertCore.webglDir}","*.webgl.data.unityweb.bin.br");
            if (dataFiles.Length > 0)
            {
                File.Copy(dataFiles[0], $"{_localSharedPath}/{Path.GetFileName(dataFiles[0])}", true);
            }

            Debug.Log("data资源已经上传至共享盘！");
        }

        [MenuItem("Build/微信小程序")]
        public static void BuildWxGame()
        {
            BeforeExport();
            WXConvertCore.WXExportError exportError = WXConvertCore.DoExport();
            if (exportError == WXConvertCore.WXExportError.SUCCEED)
            {
                Debug.Log("导出成功");
                AfterExport();
            }
            else
            {
                Debug.Log("导出失败");
            }

            if (!string.IsNullOrEmpty(_beforeCdn))
            {
                WXConvertCore.config.ProjectConf.CDN = _beforeCdn;
            }
            EditorUtility.SetDirty(WXConvertCore.config);
            AssetDatabase.SaveAssetIfDirty(WXConvertCore.config);
        }
    }
}