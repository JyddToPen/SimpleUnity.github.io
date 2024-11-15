using System.Collections.Generic;
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
        [MenuItem("Build/微信小程序")]
        public static void BuildWxGame()
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup),"ENABLE_WECHAT");
            WXConvertCore.WXExportError exportError = WXConvertCore.DoExport();
            if (exportError == WXConvertCore.WXExportError.SUCCEED)
            {
                Debug.Log("导出成功");
            }
            else
            {
                Debug.Log("导出失败");
            }
        }
    }
}