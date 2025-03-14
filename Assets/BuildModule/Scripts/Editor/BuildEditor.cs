using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BuildModule.Scripts.Editor
{
    public static class BuildEditor
    {
        private static PlatformBuild _platformBuild;

        [MenuItem("Tools/Build/BuildWindow")]
        private static void BuildWindow()
        {
            EditorWindow.GetWindow<BuildWindow>();
        }
    }
}