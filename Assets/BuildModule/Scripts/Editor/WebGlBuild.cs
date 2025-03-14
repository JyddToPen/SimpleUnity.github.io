using System.IO;
using BaseLib.Scripts.Runtime;
using UnityEditor;
using UnityEngine;

namespace BuildModule.Scripts.Editor
{
    public class WebGlBuild : PlatformBuild
    {
        /// <summary>
        /// webgl目录
        /// </summary>
        protected override string TargetFolder => "Player2WebGl";

        /// <summary>
        /// cdn
        /// </summary>
        protected override string CdnReadFilePath => $"{Application.dataPath}/../Library/WebGlCdn";


        /// <summary>
        /// 导出后
        /// </summary>
        protected override void PostBuild(string outputPath, string[] bootScenes)
        {
            if (!IsPreferLocalCdn)
            {
                return;
            }

            LocalCdnPhysicalPath = LocalCdnPhysicalPath.Replace("/", "\\");
            File.Copy($"{Application.dataPath}/../web.config", $"{outputPath}/web.config", true);
            FileUtility.CopyFolder(outputPath, LocalCdnPhysicalPath);
            Debug.Log($"webGl构建的data资源已经上传至共享盘！访问地址:{LocalCdnRemotePath}");
        }
    }
}