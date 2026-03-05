using UnityEngine;
using UnityEngine.InputSystem;

namespace FishCargo.Runtime.Gameplay.Harpoon
{
    /// <summary>
    /// 瞄准角度控制器（S4）
    /// 角度域：船底半圆 0° ~ -180°（世界 XY 平面直接法，不走船体局部转换）
    /// 鼠标映射：HarpoonMuzzleCurrent -> 鼠标世界点向量，世界 XY atan2 直接计算
    /// 指示器位置：键鼠模式下跟随鼠标落点（或边界夹紧点），手柄模式下使用固定半径
    /// 上半平面（toMouse.y > 0）：夹紧到 0° 或 -180°
    /// 手柄映射：右摇杆 X 轴映射为角度增减速度（度/秒）
    /// </summary>
    public class AimController : MonoBehaviour
    {
        [Header("配置")]
        public Boat.BoatConfig config;
        public Boat.InputHandler inputHandler;

        [Header("瞄准起点（P0 强制）")]
        [Tooltip("发射锚点 Transform，瞄准角计算必须从此点出发，而非船体根节点")]
        public Transform harpoonMuzzleCurrent;

        [Header("瞄准指示器")]
        public Transform aimIndicator;

        [Header("调试")]
        public bool showDebugInfo = true;

        /// <summary>当前瞄准角度（度，世界 XY 平面，0°=右，-90°=下，-180°=左）</summary>
        public float CurrentAimAngle { get; private set; } = -90f;

        /// <summary>当前瞄准方向（世界坐标系单位向量）</summary>
        public Vector3 CurrentAimDirection { get; private set; }

        /// <summary>当前瞄准目标点（世界坐标，键鼠模式下跟随鼠标落点或边界夹紧点）</summary>
        public Vector3 CurrentAimTargetPoint { get; private set; }

        // 调试字段（用于验收）
        [HideInInspector] public float debugMouseAngle;
        [HideInInspector] public float debugAimAngle;
        [HideInInspector] public float debugAngleErrorAbs;

        private HarpoonStateMachine _stateMachine;
        private bool _isUsingMouse;

        void Awake()
        {
            _stateMachine = GetComponent<HarpoonStateMachine>();
        }

        void Update()
        {
            if (_stateMachine == null || _stateMachine.CurrentState != HarpoonStateMachine.State.Aiming)
            {
                return;
            }

            UpdateAimAngle();
            UpdateAimDirection();
            UpdateTargetPoint();
            UpdateIndicator();
        }

        void UpdateAimAngle()
        {
            if (config == null || inputHandler == null) return;

            // 瞄准起点：P0 强制使用 harpoonMuzzleCurrent，回退到船体根节点
            Vector3 origin = harpoonMuzzleCurrent != null ? harpoonMuzzleCurrent.position : transform.position;

            // 检测输入源：鼠标优先，手柄次之
            bool usingMouse = Mouse.current != null && Mouse.current.rightButton.isPressed;
            bool usingGamepad = Mathf.Abs(inputHandler.RightStickX) > config.aimStickDeadZone;

            _isUsingMouse = usingMouse;

            if (usingMouse)
            {
                Vector3 mousePos = inputHandler.MouseWorldPosition;
                Vector3 toMouse = mousePos - origin;

                // 世界 XY 平面直接法：不走 InverseTransformDirection，避免 3D 姿态引入偏差
                float rawAngle = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

                // 归一化到 [-180, 180]
                while (rawAngle > 180f) rawAngle -= 360f;
                while (rawAngle < -180f) rawAngle += 360f;

                // 调试：记录鼠标原始角度
                debugMouseAngle = rawAngle;

                // 半圆约束：下半平面直接 clamp，上半平面夹到最近边界
                // 使用 config 参数，若 config 为空则回退到默认值 -180/0
                float minAngle = config != null ? config.minAimAngle : -180f;
                float maxAngle = config != null ? config.maxAimAngle : 0f;

                if (toMouse.y <= 0f)
                {
                    // 下半平面：合法区域，直接夹紧到 [minAimAngle, maxAimAngle]
                    CurrentAimAngle = Mathf.Clamp(rawAngle, minAngle, maxAngle);
                }
                else
                {
                    // 上半平面：夹紧到最近边界（maxAimAngle 或 minAimAngle）
                    CurrentAimAngle = (toMouse.x >= 0f) ? maxAngle : minAngle;
                }
            }
            else if (usingGamepad)
            {
                // 手柄映射：右摇杆 X 轴 -> 角度增减速度
                // 使用 config 参数，若 config 为空则回退到默认值 -180/0
                float minAngle = config != null ? config.minAimAngle : -180f;
                float maxAngle = config != null ? config.maxAimAngle : 0f;
                float deltaAngle = inputHandler.RightStickX * config.aimRotateSpeed * Time.deltaTime;
                CurrentAimAngle = Mathf.Clamp(CurrentAimAngle + deltaAngle, minAngle, maxAngle);
            }

            // 调试字段更新
            debugAimAngle = CurrentAimAngle;
            debugAngleErrorAbs = Mathf.Abs(debugMouseAngle - debugAimAngle);
        }

        void UpdateAimDirection()
        {
            // 世界 XY 平面直接法：角度直接转世界方向，不走 TransformDirection
            CurrentAimDirection = new Vector3(
                Mathf.Cos(CurrentAimAngle * Mathf.Deg2Rad),
                Mathf.Sin(CurrentAimAngle * Mathf.Deg2Rad),
                0f
            );
        }

        void UpdateTargetPoint()
        {
            Vector3 origin = harpoonMuzzleCurrent != null ? harpoonMuzzleCurrent.position : transform.position;

            if (_isUsingMouse && inputHandler != null)
            {
                // 键鼠模式：目标点跟随鼠标落点（或边界夹紧点）
                Vector3 mousePos = inputHandler.MouseWorldPosition;
                Vector3 toMouse = mousePos - origin;
                float radius = toMouse.magnitude;

                // 防止除零，最小半径 0.1
                if (radius < 0.1f) radius = 0.1f;

                // 目标点 = 原点 + 夹紧后的方向 * 鼠标距离
                CurrentAimTargetPoint = origin + CurrentAimDirection * radius;
            }
            else
            {
                // 手柄模式：使用固定半径（无鼠标落点参考）
                float fixedRadius = 1.5f;
                CurrentAimTargetPoint = origin + CurrentAimDirection * fixedRadius;
            }
        }

        void UpdateIndicator()
        {
            if (aimIndicator == null) return;

            // 指示器位置：使用计算好的目标点
            aimIndicator.position = CurrentAimTargetPoint;

            // 指示器朝向：直接用 CurrentAimAngle（世界角度）
            aimIndicator.rotation = Quaternion.Euler(0f, 0f, CurrentAimAngle);

            // 仅在瞄准时显示
            aimIndicator.gameObject.SetActive(_stateMachine.CurrentState == HarpoonStateMachine.State.Aiming);
        }

        void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 250, 320, 100));
            GUILayout.Label($"<b>Aim Angle:</b> {CurrentAimAngle:F1}°", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 });
            GUILayout.Label($"<b>Mouse Angle:</b> {debugMouseAngle:F1}°  <b>Err:</b> {debugAngleErrorAbs:F1}°", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 13 });
            GUILayout.Label($"<b>Aim Dir:</b> ({CurrentAimDirection.x:F2}, {CurrentAimDirection.y:F2})");
            GUILayout.EndArea();
        }
    }
}
