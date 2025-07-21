using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class ColorBlitRendererFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Shader m_Shader;
    [SerializeField]
    private float m_Intensity;

    private Material m_Material;

    private ColorBlitPass m_RenderPass = null;

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game && renderingData.cameraData.camera == Camera.main)
            renderer.EnqueuePass(m_RenderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer,
        in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game && renderingData.cameraData.camera == Camera.main)
        {
            // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
            // ensures that the opaque texture is available to the Render Pass.
            m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_RenderPass.SetTarget(renderer.cameraColorTargetHandle, m_Intensity);
        }
    }

    public override void Create()
    {
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_RenderPass = new ColorBlitPass(m_Material);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }
}