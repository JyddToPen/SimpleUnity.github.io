using System;
using System.Collections;
using BuildModule.Scripts.Runtime.AssetManager;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace LearnShader.Scripts.Runtime
{
    public class ShaderMain : MonoBehaviour
    {
        private static readonly int NormalMap = Shader.PropertyToID("_NormalMap");
        private static readonly int GlossyReflections = Shader.PropertyToID("_GlossyReflections");
        private static readonly int EnvironmentReflections = Shader.PropertyToID("_EnvironmentReflections");
        private const string UrpLitShader = "Universal Render Pipeline/Lit";
        private const string StandardShader = "Standard";
        private const string StandardSpecularShader = "Standard (Specular setup)";

        private IEnumerator Start()
        {
            AssetLoadResult<GameObject> assetLoadResult = new AssetLoadResult<GameObject>();
            yield return AssetFactory.Instance.AssetLoad.AsyLoadAsset("LearnShader",
                "Assets.LearnShader.Prefabs.assetbundle", "Sphere", ExtensionName.Prefab, assetLoadResult);
            GameObject sphere = assetLoadResult.AssetObject;
            var sphereIns = GameObject.Instantiate(sphere);
            sphereIns.name = sphere.name;
            ReplaceRegularShader(UrpLitShader, GraphicsSettings.renderPipelineAsset);
        }

        private void OnGUI()
        {
            GUIStyle buttonGUIStyle = new GUIStyle()
            {
                fontSize = 32
            };
            if (GUILayout.Button("替换为Built-up Standard Specular", buttonGUIStyle))
            {
                ReplaceRegularShader(StandardSpecularShader, null);
            }

            if (GUILayout.Button("替换为Built-up Standard Metallic", buttonGUIStyle))
            {
                ReplaceRegularShader(StandardShader, null);
            }
        }

        /// <summary>
        /// 替换常规shader
        /// </summary>
        /// <param name="targetShader"></param>
        /// <param name="renderPipelineAsset"></param>
        private void ReplaceRegularShader(string targetShader, RenderPipelineAsset renderPipelineAsset)
        {
            GraphicsSettings.renderPipelineAsset = renderPipelineAsset;
            foreach (var meshRenderer in Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                Shader newShader = null;
                bool isUrpLit = meshRenderer.sharedMaterial.shader.name == UrpLitShader;
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
                if (targetShader is StandardSpecularShader or StandardShader)
                {
                    meshRenderer.material.SetFloat(GlossyReflections, reflect);
                }
#if UNITY_EDITOR
                UnityEditor.Selection.activeObject = meshRenderer.gameObject;
#endif
            }
        }
    }
}