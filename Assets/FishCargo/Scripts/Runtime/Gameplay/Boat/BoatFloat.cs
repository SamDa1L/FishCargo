using UnityEngine;
using FishCargo.Runtime.Gameplay.Boat;

namespace FishCargo.Runtime.Gameplay.Boat
{
    /// <summary>
    /// 渔船浮力控制器（S2）
    /// 简化浮力方案：正弦波高度采样 + 俯仰阻尼 + 海面法线对齐
    /// 所有参数外置至 BoatConfig ScriptableObject
    /// </summary>
    public class BoatFloat : MonoBehaviour
    {
        [Header("配置")]
        public BoatConfig config;

        [Header("调试")]
        [Tooltip("是否启用法线对齐（可在运行时切换对比）")]
        public bool enableNormalAlign = true;

        [Tooltip("显示调试信息")]
        public bool showDebugInfo = true;

        // 运行时状态
        private float currentPitch = 0f;
        private float baseY = 0f;
        private float wavePhase = 0f;
        private Quaternion initialLocalRotation;

        // 调试数据（供多帧率测试记录使用）
        [HideInInspector] public float debugCurrentY;
        [HideInInspector] public float debugCurrentPitch;
        [HideInInspector] public float debugLastPitch;
        [HideInInspector] public float debugPitchDelta;
        [HideInInspector] public float debugSurfaceY;

        void Start()
        {
            baseY = transform.position.y;
            initialLocalRotation = transform.localRotation;
        }

        void Update()
        {
            if (config == null) return;

            UpdateWavePhase();
            float targetY = SampleWaveHeight();
            ApplyVerticalFollow(targetY);
            ApplyPitchRotation();
            RecordDebugData();
        }

        /// <summary>
        /// 推进波浪相位（与帧率无关）
        /// </summary>
        void UpdateWavePhase()
        {
            wavePhase += Time.deltaTime * config.waveFrequency * 2f * Mathf.PI;
        }

        /// <summary>
        /// 正弦波高度采样
        /// </summary>
        float SampleWaveHeight()
        {
            return baseY + Mathf.Sin(wavePhase) * config.waveAmplitude + config.surfaceOffset;
        }

        /// <summary>
        /// 垂向跟随：平滑插值到目标高度
        /// </summary>
        void ApplyVerticalFollow(float targetY)
        {
            float smoothSpeed = config.floatFollow * 10f;
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * smoothSpeed);
            transform.position = pos;
        }

        /// <summary>
        /// 俯仰旋转：跟随海面法线，带阻尼平滑，限制最大角度
        /// </summary>
        void ApplyPitchRotation()
        {
            if (!enableNormalAlign)
            {
                // 法线对齐关闭时，阻尼回正
                currentPitch = Mathf.Lerp(currentPitch, 0f, Time.deltaTime * config.pitchDamping);
            }
            else
            {
                // 根据波浪斜率计算目标俯仰角
                float slope = Mathf.Cos(wavePhase) * config.waveAmplitude * config.waveFrequency * 2f * Mathf.PI;
                float targetPitch = Mathf.Atan(slope) * Mathf.Rad2Deg * config.normalAlign;
                targetPitch = Mathf.Clamp(targetPitch, -config.maxPitch, config.maxPitch);
                currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * config.pitchDamping);
            }

            transform.localRotation = initialLocalRotation * Quaternion.Euler(0f, 0f, currentPitch);
        }

        /// <summary>
        /// 记录调试数据（供 DebugOverlay 和多帧率测试使用）
        /// </summary>
        void RecordDebugData()
        {
            debugLastPitch = debugCurrentPitch;
            debugCurrentY = transform.position.y;
            debugCurrentPitch = currentPitch;
            debugPitchDelta = Mathf.Abs(debugCurrentPitch - debugLastPitch);
            debugSurfaceY = baseY + Mathf.Sin(wavePhase) * config.waveAmplitude + config.surfaceOffset;
        }

        /// <summary>
        /// 获取当前海面高度（供外部系统查询）
        /// </summary>
        public float GetSurfaceY()
        {
            return debugSurfaceY;
        }

        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            // 绘制海面基准线
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                transform.position + Vector3.left * 2f,
                transform.position + Vector3.right * 2f
            );
        }
    }
}
