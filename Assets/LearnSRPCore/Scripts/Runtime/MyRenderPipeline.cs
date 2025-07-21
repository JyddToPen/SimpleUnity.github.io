using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

public class MyRenderPipeline : RenderPipeline
{
    RenderGraph m_RenderGraph;

    void InitializeRenderGraph()
    {
        m_RenderGraph = new RenderGraph("MyRenderGraph");
    }

    void CleanupRenderGraph()
    {
        m_RenderGraph.Cleanup();
        m_RenderGraph = null;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        
    }
}