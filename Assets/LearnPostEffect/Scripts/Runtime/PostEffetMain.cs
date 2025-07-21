using System;
using System.Collections;
using BuildModule.Scripts.Runtime.AssetManager;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace LearnPostEffect.Scripts.Runtime
{
    [ExecuteAlways]
    public class PostEffectMain : MonoBehaviour
    {
        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera renderCamera)
        {
            // Debug.Log($"OnBeginCameraRendering {renderCamera}");
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }
    }
}