using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MetaballRender2D : ScriptableRendererFeature
{
    [System.Serializable]
    public class MetaballRender2DSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Range(0f, 1f), Tooltip("Outline size.")]
        public float outlineSize = 1.0f;

        [Tooltip("Inner color.")]
        public Color innerColor = Color.white;

        [Tooltip("Outline color.")]
        public Color outlineColor = Color.black;
    }

    public MetaballRender2DSettings settings = new MetaballRender2DSettings();

    class MetaballRender2DPass : ScriptableRenderPass
    {
        private Material material;

        public float outlineSize;
        public Color innerColor;
        public Color outlineColor;

        private bool isFirstRender = true;

        private RenderTargetIdentifier source;
        private string profilerTag;

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;

            material = new Material(Shader.Find("Hidden/Metaballs2D"));
        }

        public MetaballRender2DPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            if(isFirstRender)
            {
                isFirstRender = false;
                cmd.SetGlobalVectorArray("_MetaballData", new Vector4[1000]);
            }

            List<Metaball2D> metaballs = MetaballSystem2D.Get();
            List<Vector4> metaballData = new List<Vector4>(metaballs.Count);

            for(int i = 0; i < metaballs.Count; ++i)
            {
                Vector2 pos = renderingData.cameraData.camera.WorldToScreenPoint(metaballs[i].transform.position);
                float radius = metaballs[i].GetRadius();
                metaballData.Add(new Vector4(pos.x, pos.y, radius, 0.0f));
            }

            if(metaballData.Count > 0)
            {
                cmd.SetGlobalInt("_MetaballCount", metaballs.Count);
                cmd.SetGlobalVectorArray("_MetaballData", metaballData);
                cmd.SetGlobalFloat("_OutlineSize", outlineSize);
                cmd.SetGlobalColor("_InnerColor", innerColor);
                cmd.SetGlobalColor("_OutlineColor", outlineColor);
                cmd.SetGlobalFloat("_CameraSize", renderingData.cameraData.camera.orthographicSize);

                cmd.Blit(source, source, material);

                context.ExecuteCommandBuffer(cmd);
            }
            
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    MetaballRender2DPass pass;

    public override void Create()
    {
        name = "Metaballs (2D)";

        pass = new MetaballRender2DPass("Metaballs2D");

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
