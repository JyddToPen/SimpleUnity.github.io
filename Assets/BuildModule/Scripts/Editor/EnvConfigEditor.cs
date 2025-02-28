using BuildModule.Scripts.Runtime;
using UnityEditor;

namespace BuildModule.Scripts.Editor
{
    [CustomEditor(typeof(EnvConfig))]
    public class EnvConfigEditor : UnityEditor.Editor
    {
        private EnvConfig _envConfig;

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