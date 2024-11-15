using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LearnWebGL.Scripts.Editor
{
    public class WebGlBuild : IFilterBuildAssemblies
    {
        public int callbackOrder => 100;

        /// <summary>
        /// 剔除微信相关
        /// </summary>
        /// <param name="buildOptions"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public string[] OnFilterAssemblies(BuildOptions buildOptions, string[] assemblies)
        {
            string symbols =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!symbols.Contains("ENABLE_WECHAT"))
            {
                string[] remove = { "Wx.dll", "wx-runtime.dll", "WxWasmSDKRuntime.dll", "LitJson.dll" };
                var ret = assemblies.Where(item => remove.All(r => !item.EndsWith(r))).ToArray();
                return ret;
            }
            else
            {
                return assemblies;
            }
        }

        [MenuItem("Build/WebGL")]
        public static void BuildWxGame()
        {
            PlayerSettings.SetScriptingDefineSymbols(
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), "");
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/LearnWebGL/Scene/WebGLMain.unity" },
                locationPathName = Application.dataPath + "/../WebGlGame",
                target = EditorUserBuildSettings.activeBuildTarget,
                options = BuildOptions.None,
                extraScriptingDefines = new string[] { }
            };
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }
    }
}