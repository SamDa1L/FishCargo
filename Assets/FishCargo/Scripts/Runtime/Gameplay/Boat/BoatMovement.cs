using UnityEngine;

namespace FishCargo.Runtime.Gameplay.Boat
{
    /// <summary>
    /// 渔船横向移动控制器（S3）
    /// 线性加速 + Ease-Out 制动，边界到边清零，帧率无关
    /// </summary>
    public class BoatMovement : MonoBehaviour
    {
        [Header("配置")]
        public BoatConfig config;

        [Header("输入")]
        public InputHandler inputHandler;

        [Header("调试")]
        public bool showDebugInfo = true;

        [HideInInspector] public float debugCurrentSpeed;
        [HideInInspector] public float debugMoveAxis;
        [HideInInspector] public bool debugAtLeftBoundary;
        [HideInInspector] public bool debugAtRightBoundary;

        private float _currentSpeed;

        void Update()
        {
            if (config == null || inputHandler == null) return;

            float axis = inputHandler.MoveAxisX * config.inputResponse;
            UpdateSpeed(axis);
            ApplyMovement();
            RecordDebugData(axis);
        }

        /// <summary>
        /// 线性加速 + Ease-Out 制动
        /// </summary>
        void UpdateSpeed(float axis)
        {
            if (Mathf.Abs(axis) > 0.001f)
            {
                // 加速：线性
                float targetSpeed = axis * config.maxMoveSpeed;
                _currentSpeed = Mathf.MoveTowards(
                    _currentSpeed,
                    targetSpeed,
                    config.acceleration * Time.deltaTime
                );
            }
            else
            {
                // 制动：Ease-Out（Lerp 趋近零，前快后慢）
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, config.deceleration * Time.deltaTime);
                if (Mathf.Abs(_currentSpeed) < 0.01f) _currentSpeed = 0f;
            }
        }

        /// <summary>
        /// 应用位移，到边清零速度
        /// </summary>
        void ApplyMovement()
        {
            Vector3 pos = transform.position;
            pos.x += _currentSpeed * Time.deltaTime;

            // 左边界
            if (pos.x <= config.leftBoundary)
            {
                pos.x = config.leftBoundary;
                if (_currentSpeed < 0f) _currentSpeed = 0f;
            }
            // 右边界
            else if (pos.x >= config.rightBoundary)
            {
                pos.x = config.rightBoundary;
                if (_currentSpeed > 0f) _currentSpeed = 0f;
            }

            transform.position = pos;
        }

        void RecordDebugData(float axis)
        {
            debugCurrentSpeed = _currentSpeed;
            debugMoveAxis = axis;
            debugAtLeftBoundary = transform.position.x <= config.leftBoundary;
            debugAtRightBoundary = transform.position.x >= config.rightBoundary;
        }

        void OnDrawGizmosSelected()
        {
            if (!showDebugInfo || config == null) return;
            Gizmos.color = Color.green;
            Vector3 p = transform.position;
            Gizmos.DrawLine(new Vector3(config.leftBoundary, p.y - 1f, p.z),
                            new Vector3(config.leftBoundary, p.y + 1f, p.z));
            Gizmos.DrawLine(new Vector3(config.rightBoundary, p.y - 1f, p.z),
                            new Vector3(config.rightBoundary, p.y + 1f, p.z));
        }
    }
}
