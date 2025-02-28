using System.IO;
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
        protected override void PostBuild(string outputPath)
        {
            if (!IsPreferLocalCdn)
            {
                return;
            }

            LocalCdnPhysicalPath = LocalCdnPhysicalPath.Replace("/", "\\");
            File.Copy($"{Application.dataPath}/BuildModule/web.config", $"{outputPath}/web.config");
            BuildEditor.StartBatMulLineInput(new[]
            {
                //安全起见，这里只做覆盖操作，防止出现意外
                //$"if exist {LocalCdnPhysicalPath} rd /s /q {LocalCdnPhysicalPath}",
                $"echo d|xcopy /s /y /q {outputPath.Replace("/", "\\")} {LocalCdnPhysicalPath}",
            });
            Application.OpenURL(LocalCdnRemotePath);
            Debug.Log($"webGl构建的data资源已经上传至共享盘！访问地址:{LocalCdnRemotePath}");
        }
    }
}