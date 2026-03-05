using UnityEngine;

namespace FishCargo.Runtime.Gameplay.Boat
{
    /// <summary>
    /// 渔船配置参数（ScriptableObject）
    /// 包含浮力、移动、边界等所有船体相关参数
    /// </summary>
    [CreateAssetMenu(fileName = "BoatConfig", menuName = "FishCargo/Config/BoatConfig")]
    public class BoatConfig : ScriptableObject
    {
        [Header("浮力参数")]
        [Tooltip("波浪振幅（海面上下浮动幅度）")]
        [Range(0.10f, 0.25f)]
        public float waveAmplitude = 0.15f;

        [Tooltip("波浪频率（Hz，波浪周期快慢）")]
        [Range(0.5f, 1.2f)]
        public float waveFrequency = 0.8f;

        [Tooltip("船体垂向跟随强度（高度贴合力度）")]
        [Range(0.7f, 1.3f)]
        public float floatFollow = 1.0f;

        [Tooltip("俯仰阻尼（越大越稳，响应更慢）")]
        [Range(2.0f, 5.0f)]
        public float pitchDamping = 3.0f;

        [Tooltip("法线对齐强度（0=不对齐，1=完全对齐）")]
        [Range(0.0f, 1.0f)]
        public float normalAlign = 0.5f;

        [Tooltip("最大俯仰角限制（度，防止视觉过激摆动）")]
        [Range(6f, 12f)]
        public float maxPitch = 8f;

        [Tooltip("贴海面高度偏移（用于微调船底接触观感）")]
        [Range(-0.05f, 0.10f)]
        public float surfaceOffset = 0.0f;

        [Header("移动参数（S3）")]
        [Tooltip("最大移动速度（单位/秒）")]
        [Range(4.0f, 6.5f)]
        public float maxMoveSpeed = 5.0f;

        [Tooltip("加速度（按住输入后的提速强度）")]
        [Range(10.0f, 22.0f)]
        public float acceleration = 15.0f;

        [Tooltip("减速度（松开输入后的制动强度）")]
        [Range(12.0f, 28.0f)]
        public float deceleration = 20.0f;

        [Tooltip("输入响应系数（输入到速度变化的整体倍率）")]
        [Range(0.8f, 1.2f)]
        public float inputResponse = 1.0f;

        [Tooltip("摇杆死区（小于此值的输入按 0 处理）")]
        [Range(0.10f, 0.25f)]
        public float stickDeadZone = 0.15f;

        [Header("边界参数")]
        [Tooltip("左边界 X（世界坐标）")]
        public float leftBoundary = -12.0f;

        [Tooltip("右边界 X（世界坐标）")]
        public float rightBoundary = 12.0f;

        [Header("瞄准参数（S4）")]
        [Tooltip("最小瞄准角（船底半圆，-180°）")]
        [Range(-180f, -90f)]
        public float minAimAngle = -180f;

        [Tooltip("最大瞄准角（船底半圆，0°）")]
        [Range(-90f, 0f)]
        public float maxAimAngle = 0f;

        [Tooltip("手柄右摇杆角速度（度/秒）")]
        [Range(90f, 300f)]
        public float aimRotateSpeed = 180f;

        [Tooltip("右摇杆死区（小于此值不更新角度）")]
        [Range(0.15f, 0.30f)]
        public float aimStickDeadZone = 0.20f;

        [Tooltip("瞄准时是否锁定移动（S4 默认 false）")]
        public bool lockMoveWhenAiming = false;

        [Header("鱼叉参数（S4）")]
        [Tooltip("鱼叉最大飞行距离")]
        [Range(8f, 18f)]
        public float harpoonMaxDistance = 12f;

        [Tooltip("鱼叉飞出速度（单位/秒）")]
        [Range(10f, 28f)]
        public float harpoonFlySpeed = 18f;

        [Tooltip("鱼叉回收速度（单位/秒）")]
        [Range(12f, 35f)]
        public float harpoonRetractSpeed = 22f;

        [Tooltip("回收完成后额外冷却时间（秒）")]
        [Range(0f, 1.2f)]
        public float postRetractCooldown = 0.35f;

        [Tooltip("鱼叉头部本地偏移（相对于 HarpoonContainer，X=前方）")]
        public Vector3 harpoonTipLocalOffset = new Vector3(0.9f, 0f, 0f);

        [Tooltip("鱼叉视觉缩放（运行时实例化后应用）")]
        [Range(0.2f, 10.0f)]
        public float harpoonVisualScale = 5f;

        [Tooltip("鱼叉视觉朝向补偿角（度，用于修正模型朝向轴与发射方向不一致，默认 0）")]
        [Range(-180f, 180f)]
        public float harpoonVisualAngleOffset = 0f;
    }
}
