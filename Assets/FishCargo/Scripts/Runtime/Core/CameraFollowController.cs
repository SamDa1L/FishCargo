using UnityEngine;

namespace FishCargo.Runtime.Core
{
    /// <summary>
    /// 相机跟随控制器
    /// 按策划文档规范：横向跟随渔船，纵向固定（S1 阶段）
    /// 支持死区、平滑跟随、边界限制
    /// </summary>
    public class CameraFollowController : MonoBehaviour
    {
        [Header("跟随目标")]
        [Tooltip("跟随的目标对象（渔船）")]
        public Transform target;

        [Header("跟随配置")]
        [Tooltip("横向跟随平滑系数（越大越灵敏）")]
        public float horizontalSmoothSpeed = 5f;

        [Tooltip("横向死区宽度（单位）")]
        public float horizontalDeadZone = 0.5f;

        [Tooltip("是否启用纵向跟随（S1 阶段默认关闭）")]
        public bool enableVerticalFollow = false;

        [Tooltip("纵向跟随平滑系数")]
        public float verticalSmoothSpeed = 3f;

        [Header("边界限制")]
        [Tooltip("是否启用边界限制")]
        public bool enableBoundary = true;

        [Tooltip("左边界 X")]
        public float leftBoundary = -50f;

        [Tooltip("右边界 X")]
        public float rightBoundary = 50f;

        [Tooltip("上边界 Y")]
        public float topBoundary = 10f;

        [Tooltip("下边界 Y")]
        public float bottomBoundary = -50f;

        [Header("调试")]
        [Tooltip("是否显示调试信息")]
        public bool showDebugInfo = true;

        private Vector3 targetPosition;
        private Vector3 velocity = Vector3.zero;

        void LateUpdate()
        {
            if (target == null) return;

            UpdateCameraPosition();
        }

        /// <summary>
        /// 更新相机位置
        /// </summary>
        void UpdateCameraPosition()
        {
            Vector3 currentPos = transform.position;
            targetPosition = currentPos;

            // 横向跟随（带死区）
            float horizontalOffset = target.position.x - currentPos.x;
            if (Mathf.Abs(horizontalOffset) > horizontalDeadZone)
            {
                float targetX = target.position.x;
                targetPosition.x = Mathf.Lerp(currentPos.x, targetX, horizontalSmoothSpeed * Time.deltaTime);
            }

            // 纵向跟随（S1 阶段默认关闭）
            if (enableVerticalFollow)
            {
                float targetY = target.position.y;
                targetPosition.y = Mathf.Lerp(currentPos.y, targetY, verticalSmoothSpeed * Time.deltaTime);
            }

            // 边界限制
            if (enableBoundary)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, leftBoundary, rightBoundary);
                targetPosition.y = Mathf.Clamp(targetPosition.y, bottomBoundary, topBoundary);
            }

            // 应用位置
            transform.position = targetPosition;
        }

        /// <summary>
        /// 设置跟随目标
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// 立即移动到目标位置（无平滑）
        /// </summary>
        public void SnapToTarget()
        {
            if (target == null) return;

            Vector3 snapPos = transform.position;
            snapPos.x = target.position.x;

            if (enableVerticalFollow)
            {
                snapPos.y = target.position.y;
            }

            if (enableBoundary)
            {
                snapPos.x = Mathf.Clamp(snapPos.x, leftBoundary, rightBoundary);
                snapPos.y = Mathf.Clamp(snapPos.y, bottomBoundary, topBoundary);
            }

            transform.position = snapPos;
        }

        void OnDrawGizmos()
        {
            if (!showDebugInfo || !enableBoundary) return;

            // 绘制边界框
            Gizmos.color = Color.yellow;
            Vector3 topLeft = new Vector3(leftBoundary, topBoundary, 0);
            Vector3 topRight = new Vector3(rightBoundary, topBoundary, 0);
            Vector3 bottomLeft = new Vector3(leftBoundary, bottomBoundary, 0);
            Vector3 bottomRight = new Vector3(rightBoundary, bottomBoundary, 0);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);

            // 绘制死区
            if (target != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 deadZoneLeft = new Vector3(target.position.x - horizontalDeadZone, target.position.y, 0);
                Vector3 deadZoneRight = new Vector3(target.position.x + horizontalDeadZone, target.position.y, 0);
                Gizmos.DrawLine(deadZoneLeft + Vector3.up * 2, deadZoneLeft - Vector3.up * 2);
                Gizmos.DrawLine(deadZoneRight + Vector3.up * 2, deadZoneRight - Vector3.up * 2);
            }
        }
    }
}
