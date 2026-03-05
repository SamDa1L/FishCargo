using UnityEngine;

namespace FishCargo.Runtime.Gameplay.Harpoon
{
    /// <summary>
    /// 绳索关联（S4）
    /// 仅使用 LineRenderer 动态生成绳索（P0 强制，禁止使用静态绳索模型）
    /// 起点每帧绑定 HarpoonMuzzleCurrent，终点每帧绑定 HarpoonProjectile.HarpoonTailPosition
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class HarpoonRope : MonoBehaviour
    {
        [Header("绳索端点")]
        [Tooltip("绳索起点：船体当前发射锚点（每帧实时读取）")]
        public Transform harpoonMuzzleCurrent;

        [Tooltip("绳索终点：鱼叉投射物组件（自动取其 HarpoonTailPosition）")]
        public HarpoonProjectile projectile;

        [Header("调试")]
        public bool showDebugInfo = true;

        private LineRenderer _lineRenderer;
        private HarpoonStateMachine _stateMachine;

        void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            // 状态机从 projectile 同级或父级获取，避免层级依赖
            if (projectile != null)
                _stateMachine = projectile.GetComponent<HarpoonStateMachine>();
            if (_stateMachine == null)
                _stateMachine = GetComponentInParent<HarpoonStateMachine>();

            // 初始化 LineRenderer
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.startWidth = 0.05f;
            _lineRenderer.endWidth = 0.05f;
            _lineRenderer.enabled = false;
        }

        void LateUpdate()
        {
            bool shouldShow = _stateMachine != null &&
                (_stateMachine.CurrentState == HarpoonStateMachine.State.HarpoonOutbound ||
                 _stateMachine.CurrentState == HarpoonStateMachine.State.HarpoonRetracting);

            _lineRenderer.enabled = shouldShow;

            if (!shouldShow) return;

            if (harpoonMuzzleCurrent == null || projectile == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning("[HarpoonRope] 绳索端点未赋值！");
                return;
            }

            // 起点：船体当前锚点（每帧实时读取，P0 强制）
            _lineRenderer.SetPosition(0, harpoonMuzzleCurrent.position);
            // 终点：鱼叉尾部位置（每帧实时读取）
            _lineRenderer.SetPosition(1, projectile.HarpoonTailPosition);
        }
    }
}
