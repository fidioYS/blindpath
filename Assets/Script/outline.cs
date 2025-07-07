using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        Material outlineMat;
        RTHandle source;

        public OutlinePass(Material mat)
        {
            this.outlineMat = mat;
            this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public void Setup(RTHandle src)
        {
            source = src;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("OutlinePass");

            // Blit from source → source with outline material
            CoreUtils.SetRenderTarget(cmd, source, ClearFlag.None, Color.clear);
            cmd.Blit(source, source, outlineMat);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [Tooltip("指定後處理 Outline 的 Shader")]
    public Shader outlineShader;

    Material outlineMat;
    OutlinePass outlinePass;

    public override void Create()
    {
        if (outlineShader == null)
        {
            Debug.LogError("Outline Shader is missing in OutlineFeature!");
            return;
        }

        outlineMat = CoreUtils.CreateEngineMaterial(outlineShader);
        outlinePass = new OutlinePass(outlineMat);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        RTHandle cameraColorTarget = renderer.cameraColorTargetHandle;
        outlinePass.Setup(cameraColorTarget);
        renderer.EnqueuePass(outlinePass);
    }
}
