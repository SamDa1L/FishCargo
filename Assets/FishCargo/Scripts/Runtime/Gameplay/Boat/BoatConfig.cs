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

        [Header("移动参数（S3 阶段使用）")]
        [Tooltip("最大移动速度（单位/秒）")]
        public float maxMoveSpeed = 5.0f;

        [Tooltip("加速度（越大手感越灵敏）")]
        public float acceleration = 15.0f;

        [Tooltip("减速度（松开输入后的制动速度）")]
        public float deceleration = 20.0f;

        [Header("边界参数")]
        [Tooltip("左边界 X（世界坐标）")]
        public float leftBoundary = -12.0f;

        [Tooltip("右边界 X（世界坐标）")]
        public float rightBoundary = 12.0f;
    }
}
