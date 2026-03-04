using UnityEngine;
using UnityEngine.Rendering;

namespace FishCargo.Runtime.Core
{
    /// <summary>
    /// 像素化渲染控制器
    /// 持有像素化渲染参数和 RTHandle，供 PixelizeRenderFeature 读取
    /// 不再使用 OnRenderImage（URP 不支持），改为通过 ScriptableRendererFeature 注入渲染管线
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class PixelizedRenderController : MonoBehaviour
    {
        [Header("像素化配置")]
        [Tooltip("目标渲染分辨率")]
        public Vector2Int targetResolution = new Vector2Int(320, 180);

        [Tooltip("放大倍数（自动计算或手动指定）")]
        public int scaleFactor = 6;

        [Tooltip("是否自动计算放大倍数")]
        public bool autoCalculateScale = true;

        [Header("渲染配置")]
        [Tooltip("渲染纹理过滤模式")]
        public FilterMode filterMode = FilterMode.Point;

        private Camera mainCamera;
        private RTHandle rtHandle;

        public RTHandle RenderTextureHandle => rtHandle;

        void Awake()
        {
            mainCamera = GetComponent<Camera>();
        }

        void Start()
        {
            SetupPixelizedRendering();
        }

        void OnDestroy()
        {
            CleanupRenderTexture();
        }

        /// <summary>
        /// 设置像素化渲染
        /// </summary>
        void SetupPixelizedRendering()
        {
            // 自动计算放大倍数
            if (autoCalculateScale)
            {
                scaleFactor = Mathf.Max(1, Screen.height / targetResolution.y);
            }

            // 创建低分辨率 RTHandle
            rtHandle = RTHandles.Alloc(
                width: targetResolution.x,
                height: targetResolution.y,
                depthBufferBits: DepthBits.None,
                filterMode: filterMode,
                name: "PixelizedRT"
            );

            Debug.Log($"[PixelizedRender] 目标分辨率: {targetResolution}, 放大倍数: {scaleFactor}");
        }

        /// <summary>
        /// 清理 RenderTexture
        /// </summary>
        void CleanupRenderTexture()
        {
            if (rtHandle != null)
            {
                RTHandles.Release(rtHandle);
                rtHandle = null;
            }
        }

        /// <summary>
        /// 运行时重新设置分辨率
        /// </summary>
        public void SetResolution(Vector2Int newResolution)
        {
            targetResolution = newResolution;
            CleanupRenderTexture();
            SetupPixelizedRendering();
        }

        /// <summary>
        /// 运行时重新设置放大倍数
        /// </summary>
        public void SetScaleFactor(int newScale)
        {
            scaleFactor = Mathf.Max(1, newScale);
            autoCalculateScale = false;
            CleanupRenderTexture();
            SetupPixelizedRendering();
        }
    }
}
