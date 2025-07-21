using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LearnPostEffect.Scripts.Runtime.SimpleFeature
{
    public class SimpleRendererFeature : ScriptableRendererFeature
    {
        private SimpleRenderPass _simpleRenderPass;
        [SerializeField] private RenderTexture targetRt;

        public override void Create()
        {
            if (targetRt && _simpleRenderPass == null)
            {
                _simpleRenderPass = new SimpleRenderPass(targetRt);
            }
        }
        
        
        private bool IsAllow(in CameraData cameraData)
        {
            return cameraData.cameraType == CameraType.Game &&
                   cameraData.camera.name == "SubCamera";
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_simpleRenderPass == null)
            {
                return;
            }


            if (!IsAllow(in renderingData.cameraData))
            {
                return;
            }
            renderer.EnqueuePass(_simpleRenderPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (_simpleRenderPass == null)
            {
                return;
            }

            if (!IsAllow(in renderingData.cameraData))
            {
                return;
            }

            _simpleRenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            _simpleRenderPass.SetTargets(renderer.cameraColorTargetHandle);
        }
    }
}