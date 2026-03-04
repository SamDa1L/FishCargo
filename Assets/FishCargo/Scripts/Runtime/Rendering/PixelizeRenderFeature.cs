using UnityEngine;
using UnityEngine.Rendering.Universal;
using FishCargo.Runtime.Core;

namespace FishCargo.Runtime.Rendering
{
    /// <summary>
    /// 像素化渲染 Feature（Render Graph API，URP 17 / Unity 6）
    /// 注册到 URP Renderer Asset，从场景中的 PixelizedRenderController 读取参数
    /// 在 URP 渲染管线中注入像素化 Blit pass
    /// </summary>
    public class PixelizeRenderFeature : ScriptableRendererFeature
    {
        private PixelizeRenderPass renderPass;

        public override void Create()
        {
            renderPass = new PixelizeRenderPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // 只在 Game 视图和打包后生效，Scene 视图跳过
            if (renderingData.cameraData.cameraType != CameraType.Game) return;

            // 从场景中找到 PixelizedRenderController
            PixelizedRenderController controller = Object.FindFirstObjectByType<PixelizedRenderController>();
            if (controller == null || controller.RenderTextureHandle == null) return;

            renderPass.Setup(controller.RenderTextureHandle);
            renderer.EnqueuePass(renderPass);
        }
    }
}
