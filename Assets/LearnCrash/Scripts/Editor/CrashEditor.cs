using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LearnCrash.Scripts.Editor
{
    public static class CrashEditor
    {
        [MenuItem("Build/Crash")]
        public static void BuildCrash()
        {
            string targetFolder = Application.dataPath + "/../Windows";
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/LearnCrash/Scene/CrashScene.unity" },
                locationPathName = targetFolder + "/crash.exe",
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