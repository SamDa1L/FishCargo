using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace FishCargo.Runtime.Rendering
{
    /// <summary>
    /// 像素化渲染 Pass（Render Graph API，URP 17 / Unity 6）
    /// 降采样到低分辨率 RTHandle，再 Point filter 放大回屏幕，实现像素风格
    /// </summary>
    public class PixelizeRenderPass : ScriptableRenderPass
    {
        private RTHandle lowResRT;

        private class PassData
        {
            public TextureHandle source;
            public TextureHandle lowRes;
        }

        public PixelizeRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public void Setup(RTHandle lowResRT)
        {
            this.lowResRT = lowResRT;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (lowResRT == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer) return;

            TextureHandle sourceHandle = resourceData.activeColorTexture;
            TextureHandle lowResHandle = renderGraph.ImportTexture(lowResRT);

            // Pass 1：降采样，相机输出 → 低分辨率 RT
            using (var builder = renderGraph.AddUnsafePass<PassData>("Pixelize Downsample", out var passData))
            {
                passData.source = sourceHandle;
                passData.lowRes = lowResHandle;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.UseTexture(passData.lowRes, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, UnsafeGraphContext ctx) =>
                {
                    CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);
                    Blitter.BlitCameraTexture(cmd, data.source, data.lowRes);
                });
            }

            // Pass 2：放大，低分辨率 RT → 相机输出
            using (var builder = renderGraph.AddUnsafePass<PassData>("Pixelize Upsample", out var passData))
            {
                passData.source = lowResHandle;
                passData.lowRes = sourceHandle;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.UseTexture(passData.lowRes, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, UnsafeGraphContext ctx) =>
                {
                    CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);
                    Blitter.BlitCameraTexture(cmd, data.source, data.lowRes);
                });
            }
        }
    }
}
