using UnityEngine;

namespace FishCargo.Runtime.Core
{
    /// <summary>
    /// 场景配置数据
    /// 存储 M1 场景的关键参数，供相机、深度刻度、像素化渲染等系统读取
    /// </summary>
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "FishCargo/Config/SceneConfig")]
    public class SceneConfig : ScriptableObject
    {
        [Header("构图配置")]
        [Tooltip("海平线世界坐标 Y 值")]
        public float seaLevelY = 0f;

        [Tooltip("海平线屏幕比例（0~1，从顶部计算）")]
        [Range(0.28f, 0.35f)]
        public float seaLevelScreenRatio = 0.30f;

        [Header("相机配置")]
        [Tooltip("正交相机尺寸（Orthographic Size）")]
        public float orthographicSize = 10f;

        [Tooltip("相机初始位置 X")]
        public float cameraInitialX = 0f;

        [Tooltip("相机初始位置 Y")]
        public float cameraInitialY = 0f;

        [Tooltip("相机初始位置 Z")]
        public float cameraInitialZ = -10f;

        [Header("镜头跟随配置")]
        [Tooltip("横向跟随平滑系数")]
        public float horizontalSmoothSpeed = 5f;

        [Tooltip("横向死区宽度")]
        public float horizontalDeadZone = 0.5f;

        [Tooltip("左边界 X")]
        public float leftBoundary = -12f;

        [Tooltip("右边界 X")]
        public float rightBoundary = 12f;

        [Header("深度刻度配置")]
        [Tooltip("主刻度间隔（米）")]
        public float mainScaleInterval = 10f;

        [Tooltip("是否显示次刻度")]
        public bool showSubScale = false;

        [Tooltip("次刻度间隔（米）")]
        public float subScaleInterval = 5f;

        [Tooltip("最大深度（米）")]
        public float maxDepth = 100f;

        [Header("像素化配置")]
        [Tooltip("目标渲染分辨率")]
        public Vector2Int pixelizedResolution = new Vector2Int(320, 180);

        [Tooltip("放大倍数")]
        public int scaleFactor = 6;

        [Tooltip("是否自动计算放大倍数")]
        public bool autoCalculateScale = true;

        [Header("HUD 安全区")]
        [Tooltip("HUD 安全区边距比例（0~1）")]
        [Range(0.05f, 0.15f)]
        public float hudSafeAreaMargin = 0.05f;
    }
}
