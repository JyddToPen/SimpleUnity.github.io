using LearnShader.Scripts.Runtime;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using Object = System.Object;

namespace LearnShader.Scripts.Editor
{
    [CustomEditor(typeof(ShaderReplace))]
    public class ShaderReplaceEditor : UnityEditor.Editor
    {
        private static readonly int NormalMap = Shader.PropertyToID("_NormalMap");
        private static readonly int GlossyReflections = Shader.PropertyToID("_GlossyReflections");
        private static readonly int EnvironmentReflections = Shader.PropertyToID("_EnvironmentReflections");
        private const string UrpLit = "Universal Render Pipeline/Lit";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("替换为Built-up Standard Specular"))
            {
                ReplaceRegularShader("Standard (Specular setup)");
            }

            if (GUILayout.Button("替换为Built-up Standard Metallic"))
            {
                ReplaceRegularShader("Standard");
            }
        }

        /// <summary>
        /// 替换常规shader
        /// </summary>
        /// <param name="targetShader"></param>
        private void ReplaceRegularShader(string targetShader)
        {
            GraphicsSettings.renderPipelineAsset = null;
            if (target is ShaderReplace shaderReplace)
            {
                MeshRenderer[] meshRenderers = shaderReplace.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in meshRenderers)
                {
                    Shader newShader = null;
                    bool isUrpLit = meshRenderer.sharedMaterial.shader.name == UrpLit;
                    float reflect = 0;
                    if (isUrpLit)
                    {
                        newShader = Shader.Find(targetShader);
                        reflect = meshRenderer.material.GetFloat(EnvironmentReflections);
                    }

                    if (!newShader)
                    {
                        continue;
                    }

                    meshRenderer.material.shader = newShader;
                    meshRenderer.material.SetFloat(GlossyReflections, reflect);
                }
            }
        }
    }
}