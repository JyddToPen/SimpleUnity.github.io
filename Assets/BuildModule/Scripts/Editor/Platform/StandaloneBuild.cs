using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BuildModule.Scripts.Editor
{
    public class StandaloneBuild : PlatformBuild
    {
        /// <summary>
        /// Standalone目录
        /// </summary>
        protected override string TargetFolder => "Player2Standalone/SimpleGame.exe";

        /// <summary>
        /// cdn
        /// </summary>
        protected override string CdnReadFilePath => null;
    }
}