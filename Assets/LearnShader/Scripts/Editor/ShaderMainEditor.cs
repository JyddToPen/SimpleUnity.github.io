using LearnShader.Scripts.Runtime;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using Object = System.Object;

namespace LearnShader.Scripts.Editor
{
    [CustomEditor(typeof(ShaderMain))]
    public class ShaderMainEditor : UnityEditor.Editor
    {
        private static readonly int NormalMap = Shader.PropertyToID("_NormalMap");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}