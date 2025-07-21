using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LearnPostEffect.Scripts.Runtime.SimpleFeature
{
    public class SimpleRenderPass : ScriptableRenderPass
    {
        private readonly RTHandle _targetRTHandle;
        private RTHandle _sourceRtHandle;
        private readonly ProfilingSampler _profilingSampler = new("ReadColorResult");

        public SimpleRenderPass(RenderTexture targetRt)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            _targetRTHandle = RTHandles.Alloc(targetRt);
        }

        public void SetTargets(RTHandle sourceRtHandle)
        {
            _sourceRtHandle = sourceRtHandle;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(_sourceRtHandle);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get();
            using (new ProfilingScope(commandBuffer, _profilingSampler))
            {
                Blitter.BlitCameraTexture(commandBuffer, _sourceRtHandle, _targetRTHandle);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            CommandBufferPool.Release(commandBuffer);
        }
    }
}