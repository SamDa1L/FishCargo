using UnityEngine;

namespace FishCargo.Runtime.Gameplay.Harpoon
{
    /// <summary>
    /// 鱼叉主控制器（S4）
    /// 串联状态机、瞄准控制、发射逻辑，响应输入并驱动各子系统
    /// </summary>
    public class HarpoonController : MonoBehaviour
    {
        [Header("输入")]
        public Boat.InputHandler inputHandler;

        [Header("子系统引用")]
        public HarpoonStateMachine stateMachine;
        public AimController aimController;
        public HarpoonProjectile projectile;

        [Header("动态回收锚点（P0 强制）")]
        [Tooltip("船体当前发射锚点，必须在 Boat_Test.prefab 上创建并赋值")]
        public Transform harpoonMuzzleCurrent;

        void Update()
        {
            if (stateMachine == null || inputHandler == null) return;

            HandleAimInput();
            HandleFireInput();
        }

        void HandleAimInput()
        {
            bool aimHold = inputHandler.AimHold;

            if (aimHold && stateMachine.CanAim)
            {
                // 进入瞄准状态
                stateMachine.EnterAiming();
            }
            else if (!aimHold && stateMachine.CurrentState == HarpoonStateMachine.State.Aiming)
            {
                // 松开瞄准键且未发射，退出瞄准
                stateMachine.ExitAiming();
            }
        }

        void HandleFireInput()
        {
            bool firePressed = inputHandler.FirePressed;

            if (firePressed && stateMachine.CanFire)
            {
                // 发射鱼叉
                Fire();
            }
        }

        void Fire()
        {
            if (aimController == null || projectile == null || harpoonMuzzleCurrent == null)
            {
                Debug.LogWarning("[HarpoonController] 子系统引用不完整，无法发射！");
                return;
            }

            // 发射方向：直接取 AimPivot 的前向轴（P0 强制，半圆瞄准机制）
            // AimPivot 已经根据鼠标方向旋转，其 forward 即为发射方向
            Vector3 fireDirection = aimController.CurrentAimDirection;
            Vector3 fireOrigin = harpoonMuzzleCurrent.position;

            // 切换状态
            stateMachine.Fire();

            // 启动鱼叉飞行
            projectile.Launch(fireOrigin, fireDirection);
        }

        void Start()
        {
            // 将 aimController 引用注入到 projectile（用于瞄准预览）
            if (projectile != null && aimController != null)
            {
                projectile.aimController = aimController;
            }
        }
    }
}
