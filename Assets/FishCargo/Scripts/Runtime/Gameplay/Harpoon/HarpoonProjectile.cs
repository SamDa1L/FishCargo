using UnityEngine;

namespace FishCargo.Runtime.Gameplay.Harpoon
{
    /// <summary>
    /// 鱼叉飞行与回收逻辑（S4）
    /// 飞出：按发射方向匀速飞行，达到最大距离后自动回收
    /// 回收：每帧读取 HarpoonMuzzleCurrent 实时位置作为目标（动态回收锚点，P0 强制）
    /// 朝向：飞出+回收全程保持初始发射朝向，回收仅改变位置
    /// </summary>
    public class HarpoonProjectile : MonoBehaviour
    {
        [Header("配置")]
        public Boat.BoatConfig config;

        [Header("动态回收锚点（P0 强制）")]
        [Tooltip("船体当前发射锚点，回收目标每帧读取此 Transform 的实时位置")]
        public Transform harpoonMuzzleCurrent;

        [Header("鱼叉模型")]
        [Tooltip("鱼叉视觉模型 Prefab（运行时实例化后重置本地变换）")]
        public GameObject harpoonModelPrefab;

        [Header("子系统引用")]
        [Tooltip("用于瞄准状态下获取当前瞄准方向")]
        public AimController aimController;

        [Header("调试")]
        public bool showDebugInfo = true;

        /// <summary>鱼叉头部世界坐标（命中参考点、视觉朝向统一使用此值）</summary>
        public Vector3 HarpoonTipPosition => _harpoonTip != null ? _harpoonTip.position : transform.position;

        /// <summary>鱼叉尾部世界坐标（绳索终点统一使用此值）</summary>
        public Vector3 HarpoonTailPosition => _harpoonTail != null ? _harpoonTail.position : transform.position;

        // 运行时状态
        private Vector3 _fireDirection;
        private Vector3 _fireOrigin;
        private float _traveledDistance;
        private GameObject _modelInstance;
        private HarpoonStateMachine _stateMachine;
        private Transform _harpoonTip;
        private Transform _harpoonTail;
        private Transform _aimPivot; // 瞄准支点（圆心旋转节点）

        // 调试字段（Inspector 可见）
        [HideInInspector] public float debugTraveledDistance;
        [HideInInspector] public Vector3 debugCurrentPos;
        [HideInInspector] public float debugDistToMuzzle;

        void Awake()
        {
            _stateMachine = GetComponent<HarpoonStateMachine>();
            if (_stateMachine == null)
                _stateMachine = GetComponentInParent<HarpoonStateMachine>();

            // 创建 AimPivot（瞄准支点）作为旋转中心
            GameObject pivotGo = new GameObject("AimPivot");
            pivotGo.transform.SetParent(transform, false);
            pivotGo.transform.localPosition = Vector3.zero;
            pivotGo.transform.localRotation = Quaternion.identity;
            _aimPivot = pivotGo.transform;

            // 提前实例化模型，挂在 AimPivot 下（P0 强制）
            if (_modelInstance == null && harpoonModelPrefab != null)
            {
                _modelInstance = Instantiate(harpoonModelPrefab, _aimPivot);
                _modelInstance.transform.localPosition = Vector3.zero;
                _modelInstance.transform.localRotation = Quaternion.identity;
                float scale = config != null ? config.harpoonVisualScale : 5f;
                _modelInstance.transform.localScale = Vector3.one * scale;
            }

            // 查找或创建 HarpoonTip 子节点
            // P0 强制：优先挂在模型实例下，使 harpoonTipLocalOffset 作用于模型本地坐标（含5倍缩放）
            // 若方向偏90°/180°，优先调 harpoonVisualAngleOffset，不要改瞄准角算法
            Transform tipParent = _modelInstance != null ? _modelInstance.transform : _aimPivot;
            Transform existing = tipParent.Find("HarpoonTip");
            if (existing != null)
            {
                _harpoonTip = existing;
            }
            else
            {
                GameObject tipGo = new GameObject("HarpoonTip");
                tipGo.transform.SetParent(tipParent, false);
                Vector3 offset = config != null ? config.harpoonTipLocalOffset : new Vector3(0.9f, 0f, 0f);
                tipGo.transform.localPosition = offset;
                _harpoonTip = tipGo.transform;
            }

            // 查找或创建 HarpoonTail 子节点（绳索终点）
            Transform existingTail = tipParent.Find("HarpoonTail");
            if (existingTail != null)
            {
                _harpoonTail = existingTail;
            }
            else
            {
                GameObject tailGo = new GameObject("HarpoonTail");
                tailGo.transform.SetParent(tipParent, false);
                Vector3 tailOffset = config != null ? -config.harpoonTipLocalOffset : new Vector3(-0.9f, 0f, 0f);
                tailGo.transform.localPosition = tailOffset;
                _harpoonTail = tailGo.transform;
            }

            // 初始隐藏
            if (_modelInstance != null)
                _modelInstance.SetActive(false);
        }

        /// <summary>发射鱼叉</summary>
        public void Launch(Vector3 origin, Vector3 direction)
        {
            _fireOrigin = origin;
            _fireDirection = direction.normalized;
            _traveledDistance = 0f;

            transform.position = origin;
            // 朝向：初始发射朝向 + 视觉补偿角，全程不变
            // 若方向偏90°/180°，优先调 harpoonVisualAngleOffset，不要改瞄准角算法
            float angle = Mathf.Atan2(_fireDirection.y, _fireDirection.x) * Mathf.Rad2Deg;
            float visualAngle = angle + (config != null ? config.harpoonVisualAngleOffset : 0f);

            // 发射时直接设置 transform 旋转（不再使用 AimPivot）
            transform.rotation = Quaternion.Euler(0f, 0f, visualAngle);

            // 重置 AimPivot 为本地坐标（发射后不再使用）
            if (_aimPivot != null)
            {
                _aimPivot.localPosition = Vector3.zero;
                _aimPivot.localRotation = Quaternion.identity;
            }

            if (_modelInstance != null)
                _modelInstance.SetActive(true);
        }

        void Update()
        {
            if (_stateMachine == null) return;

            var state = _stateMachine.CurrentState;

            if (state == HarpoonStateMachine.State.Aiming)
            {
                UpdateAimingPreview();
            }
            else if (state == HarpoonStateMachine.State.HarpoonOutbound)
            {
                UpdateOutbound();
            }
            else if (state == HarpoonStateMachine.State.HarpoonRetracting)
            {
                UpdateRetracting();
            }
            else if (state == HarpoonStateMachine.State.Ready || state == HarpoonStateMachine.State.Cooldown)
            {
                // Ready/Cooldown 时隐藏鱼叉模型
                if (_modelInstance != null)
                    _modelInstance.SetActive(false);
            }

            // 调试字段更新
            debugTraveledDistance = _traveledDistance;
            debugCurrentPos = transform.position;
            if (harpoonMuzzleCurrent != null)
                debugDistToMuzzle = Vector3.Distance(transform.position, harpoonMuzzleCurrent.position);
        }

        void UpdateAimingPreview()
        {
            if (harpoonMuzzleCurrent == null || aimController == null)
            {
                if (_modelInstance != null)
                    _modelInstance.SetActive(false);
                return;
            }

            // 显示鱼叉模型
            if (_modelInstance != null)
                _modelInstance.SetActive(true);

            // 步骤 1：先计算位置，让 HarpoonTail（鱼叉底部）精确对齐到圆心（P0 强制）
            // 必须在旋转之前计算位置，否则旋转会改变 _harpoonTail.position 导致计算错误
            if (_harpoonTail != null)
            {
                // 计算从 HarpoonTail 当前本地位置到 transform 的偏移（在旋转前计算）
                Vector3 tailToRoot = transform.position - _harpoonTail.position;
                // 让 HarpoonTail 对齐到圆心
                transform.position = harpoonMuzzleCurrent.position + tailToRoot;
            }
            else
            {
                // 回退：若 HarpoonTail 未定义，直接吸附到圆心
                transform.position = harpoonMuzzleCurrent.position;
            }

            // 步骤 2：再旋转 AimPivot，根据瞄准方向旋转（限制在下半圆）
            Vector3 aimDir = aimController.CurrentAimDirection;
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            float visualAngle = angle + (config != null ? config.harpoonVisualAngleOffset : 0f);
            _aimPivot.rotation = Quaternion.Euler(0f, 0f, visualAngle);
        }

        void UpdateOutbound()
        {
            float speed = config != null ? config.harpoonFlySpeed : 18f;
            float maxDist = config != null ? config.harpoonMaxDistance : 12f;

            float step = speed * Time.deltaTime;
            _traveledDistance += step;

            if (_traveledDistance >= maxDist)
            {
                // 到达最大距离，夹紧位置并触发回收
                transform.position = _fireOrigin + _fireDirection * maxDist;
                _traveledDistance = maxDist;
                _stateMachine.StartRetract();
            }
            else
            {
                transform.position = _fireOrigin + _fireDirection * _traveledDistance;
            }
        }

        void UpdateRetracting()
        {
            if (harpoonMuzzleCurrent == null)
            {
                Debug.LogWarning("[HarpoonProjectile] harpoonMuzzleCurrent 未赋值，无法执行动态回收！");
                return;
            }

            float speed = config != null ? config.harpoonRetractSpeed : 22f;

            // 动态回收锚点：每帧读取实时位置（P0 强制，禁止缓存发射瞬间坐标）
            Vector3 targetPos = harpoonMuzzleCurrent.position;
            float distToTarget = Vector3.Distance(transform.position, targetPos);

            float step = speed * Time.deltaTime;

            if (step >= distToTarget)
            {
                // 回收完成，吸附到锚点，隐藏模型
                transform.position = targetPos;
                _stateMachine.CompleteRetract();
                if (_modelInstance != null)
                    _modelInstance.SetActive(false);
            }
            else
            {
                // 朝当前锚点移动（不改变朝向）
                Vector3 dir = (targetPos - transform.position).normalized;
                transform.position += dir * step;
                // 朝向保持初始发射朝向（不反转）
            }
        }

        void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 330, 300, 80));
            GUILayout.Label($"<b>Harpoon Dist:</b> {debugTraveledDistance:F2}", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 });
            GUILayout.Label($"<b>Dist to Muzzle:</b> {debugDistToMuzzle:F2}");
            GUILayout.EndArea();
        }
    }
}
