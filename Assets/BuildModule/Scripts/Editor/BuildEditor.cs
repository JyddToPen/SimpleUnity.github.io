using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BuildModule.Scripts.Editor
{
    public class BuildEditor
    {
        private static PlatformBuild _platformBuild;

        [MenuItem("Assets/Build/WebGL")]
        private static void BuildWebGL()
        {
            _platformBuild = new WebGlBuild();
            _platformBuild.HandleBuild(BuildOptions.None);
        }

        [MenuItem("Assets/Build/WebGL-Develop")]
        private static void BuildWebGLDevelop()
        {
            _platformBuild = new WebGlBuild();
            _platformBuild.HandleBuild(BuildOptions.Development);
        }

        [MenuItem("Assets/Build/WeChat")]
        private static void BuildWeChat()
        {
            _platformBuild = new WxBuild();
            _platformBuild.HandleBuild(BuildOptions.None);
        }

        [MenuItem("Assets/Build/WeChat-Develop")]
        private static void BuildWeChatDevelop()
        {
            _platformBuild = new WxBuild();
            _platformBuild.HandleBuild(BuildOptions.None);
        }

        [MenuItem("Assets/Build/Standalone")]
        private static void BuildStandalone()
        {
            _platformBuild = new StandaloneBuild();
            _platformBuild.HandleBuild(BuildOptions.AutoRunPlayer);
        }

        [MenuItem("Assets/Build/Standalone-Develop")]
        private static void BuildStandaloneDevelop()
        {
            _platformBuild = new StandaloneBuild();
            _platformBuild.HandleBuild(BuildOptions.Development | BuildOptions.AutoRunPlayer |
                                       BuildOptions.ConnectWithProfiler);
        }
        
        /// <summary>
        /// κϢ
        /// </summary>
        /// <param name="inputCommand"></param>
        public static string StartBatMulLineInput(string[] inputCommand)
        {
            string outString = "";
            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.ErrorDataReceived += (o, message) =>
                {
                    if (string.IsNullOrEmpty(message.Data)) UnityEngine.Debug.LogError(message.Data);
                };
                process.Start();
                process.BeginErrorReadLine();
                process.StandardInput.AutoFlush = true;
                if (inputCommand != null)
                {
                    foreach (var command in inputCommand)
                    {
                        UnityEngine.Debug.Log(command);
                        process.StandardInput.WriteLine(command);
                    }
                }

                process.StandardInput.WriteLine("exit");
                outString = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }

            return outString;
        }
    }
}