using UnityEngine;
using System;

namespace FishCargo.Runtime.Gameplay.Harpoon
{
    /// <summary>
    /// 鱼叉状态机（S4）
    /// 状态：Ready → Aiming → HarpoonOutbound → HarpoonRetracting → Cooldown → Ready
    /// 锁定规则：Outbound/Retracting/Cooldown 期间禁止再次瞄准与发射
    /// </summary>
    public class HarpoonStateMachine : MonoBehaviour
    {
        public enum State
        {
            Ready,              // 可进入瞄准，可发射
            Aiming,             // 按住瞄准键，显示鱼叉指示方向
            HarpoonOutbound,    // 鱼叉飞出中
            HarpoonRetracting,  // 鱼叉回收中
            Cooldown            // 回收完成后的短冷却
        }

        [Header("配置")]
        public Boat.BoatConfig config;

        [Header("调试")]
        public bool showDebugInfo = true;

        /// <summary>当前状态</summary>
        public State CurrentState { get; private set; } = State.Ready;

        /// <summary>冷却剩余时间（秒）</summary>
        public float CooldownRemaining { get; private set; }

        /// <summary>冷却总时长（秒，从发射到完全冷却）</summary>
        public float CooldownTotal { get; private set; }

        /// <summary>冷却进度（0~1，从发射开始计算）</summary>
        public float CooldownProgress01 => CooldownTotal > 0f ? Mathf.Clamp01(1f - CooldownRemaining / CooldownTotal) : 1f;

        /// <summary>是否可以进入瞄准</summary>
        public bool CanAim => CurrentState == State.Ready;

        /// <summary>是否可以发射</summary>
        public bool CanFire => CurrentState == State.Aiming;

        // 状态切换事件
        public event Action<State, State> OnStateChanged;

        void Update()
        {
            // 在 Outbound、Retracting、Cooldown 状态下都更新冷却倒计时
            if (CurrentState == State.HarpoonOutbound ||
                CurrentState == State.HarpoonRetracting ||
                CurrentState == State.Cooldown)
            {
                CooldownRemaining -= Time.deltaTime;
                if (CooldownRemaining <= 0f && CurrentState == State.Cooldown)
                {
                    TransitionTo(State.Ready);
                }
            }
        }

        /// <summary>进入瞄准状态</summary>
        public void EnterAiming()
        {
            if (CanAim)
            {
                TransitionTo(State.Aiming);
            }
        }

        /// <summary>退出瞄准状态（未发射）</summary>
        public void ExitAiming()
        {
            if (CurrentState == State.Aiming)
            {
                TransitionTo(State.Ready);
            }
        }

        /// <summary>发射鱼叉</summary>
        public void Fire()
        {
            if (CanFire)
            {
                // 计算总冷却时间（从发射到完全冷却）
                if (config != null)
                {
                    float outboundTime = config.harpoonMaxDistance / config.harpoonFlySpeed;
                    float retractTime = config.harpoonMaxDistance / config.harpoonRetractSpeed;
                    CooldownTotal = outboundTime + retractTime + config.postRetractCooldown;
                }
                else
                {
                    CooldownTotal = 5f; // 默认 5 秒
                }

                CooldownRemaining = CooldownTotal;
                TransitionTo(State.HarpoonOutbound);
            }
        }

        /// <summary>开始回收</summary>
        public void StartRetract()
        {
            if (CurrentState == State.HarpoonOutbound)
            {
                TransitionTo(State.HarpoonRetracting);
            }
        }

        /// <summary>回收完成，进入冷却</summary>
        public void CompleteRetract()
        {
            if (CurrentState == State.HarpoonRetracting)
            {
                // 不需要重新设置冷却时间，继续使用 Fire() 时设置的总时长
                TransitionTo(State.Cooldown);
            }
        }

        private void TransitionTo(State newState)
        {
            if (CurrentState == newState) return;

            State oldState = CurrentState;
            CurrentState = newState;

            if (showDebugInfo)
            {
                Debug.Log($"[HarpoonStateMachine] {oldState} -> {newState}");
            }

            OnStateChanged?.Invoke(oldState, newState);
        }

        void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 150, 300, 100));
            GUILayout.Label($"<b>Harpoon State:</b> {CurrentState}", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 });
            if (CurrentState == State.HarpoonOutbound ||
                CurrentState == State.HarpoonRetracting ||
                CurrentState == State.Cooldown)
            {
                GUILayout.Label($"Cooldown: {CooldownRemaining:F2}s / {CooldownTotal:F2}s ({CooldownProgress01:P0})");
            }
            GUILayout.EndArea();
        }
    }
}
