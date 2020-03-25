using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Metaball2DRender : ScriptableRendererFeature
{
    [System.Serializable]
    public class Metaball2DRenderSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(0f, 1f), Tooltip("Outline size.")]
        public float outlineSize = 1.0f;

        [Tooltip("Inner color.")]
        public Color innerColor = Color.white;

        [Tooltip("Outline color.")]
        public Color outlineColor = Color.black;
    }

    public Metaball2DRenderSettings settings = new Metaball2DRenderSettings();

    class Metaball2DRenderPass : ScriptableRenderPass
    {
        private Material material;

        public float outlineSize;
        public Color innerColor;
        public Color outlineColor;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("Hidden/Metaballs2D"));
        }

        public Metaball2DRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            cmd.SetGlobalFloat("_OutlineSize", outlineSize);
            cmd.SetGlobalColor("_InnerColor", innerColor);
            cmd.SetGlobalColor("_OutlineColor", outlineColor);

            cmd.Blit(source, source, material);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    Metaball2DRenderPass pass;

    public override void Create()
    {
        name = "Metaballs (2D)";

        pass = new Metaball2DRenderPass("Metaballs (2D)");

        pass.outlineSize = settings.outlineSize;
        pass.innerColor = settings.innerColor;
        pass.outlineColor = settings.outlineColor;

        pass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(pass);
    }
}
