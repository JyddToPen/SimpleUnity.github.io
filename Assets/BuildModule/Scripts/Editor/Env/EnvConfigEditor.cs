using System;
using System.Linq;
using BuildModule.Scripts.Runtime;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace BuildModule.Scripts.Editor
{
    [CustomEditor(typeof(EnvConfig))]
    public class EnvConfigEditor : UnityEditor.Editor
    {
        private EnvConfig _envConfig;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            EnvConfig.Instance.runtimeTargets = Enum.GetNames(typeof(RuntimePlatform))
                .Select(item => (RuntimePlatform)Enum.Parse(typeof(RuntimePlatform), item)).ToArray();
            EnvConfig.Instance.runtimeTargetNames =
                EnvConfig.Instance.runtimeTargets.Select(item => item.ToString()).ToArray();
            for (int i = 0; i < EnvConfig.Instance.runtimeTargets.Length; i++)
            {
                switch (EnvConfig.Instance.runtimeTargets[i])
                {
                    case RuntimePlatform.WebGLPlayer:
                        EnvConfig.Instance.runtimeTargetNames[i] = BuildTargetGroup.WebGL.ToString();
                        break;
                    case RuntimePlatform.WindowsPlayer:
                        EnvConfig.Instance.runtimeTargetNames[i] = BuildTargetGroup.Standalone.ToString();
                        break;
                }
            }
        }

        private void OnEnable()
        {
            _envConfig = (EnvConfig)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                PlayerPrefs.SetInt(nameof(EnvConfig.isAb), _envConfig.isAb ? 1 : 0);
            }
        }
    }
}