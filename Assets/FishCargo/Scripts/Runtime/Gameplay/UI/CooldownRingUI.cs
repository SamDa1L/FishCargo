using UnityEngine;
using UnityEngine.UI;

namespace FishCargo.Runtime.Gameplay.UI
{
    /// <summary>
    /// 冷却圆环 UI 控制器（S4）
    /// 使用 Unity 内置 UI Image（Filled Radial 360）实现冷却进度显示
    /// 无需外部 PNG 文件，使用 Unity 默认 Sprite
    /// 位置：跟随准心位置（世界坐标转屏幕坐标）
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class CooldownRingUI : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("鱼叉状态机引用")]
        public Harpoon.HarpoonStateMachine stateMachine;

        [Tooltip("瞄准控制器引用（用于获取准心位置）")]
        public Harpoon.AimController aimController;

        [Tooltip("主摄像机（用于世界坐标转屏幕坐标）")]
        public Camera mainCamera;

        [Header("位置配置")]
        [Tooltip("相对于准心的偏移（屏幕像素）")]
        public Vector2 offsetFromCrosshair = Vector2.zero;

        [Header("视觉配置")]
        [Tooltip("圆环颜色（灰白色）")]
        public Color ringColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        [Tooltip("是否启用颜色渐变（M2 阶段功能）")]
        public bool enableColorGradient = false;

        [Header("颜色渐变配置（M2）")]
        public Color cooldownStartColor = Color.red;
        public Color cooldownMidColor = Color.yellow;
        public Color cooldownEndColor = Color.green;

        [Header("调试")]
        public bool showDebugInfo = false;

        private Image _image;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        void Awake()
        {
            _image = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();

            // 如果没有 CanvasGroup，自动添加
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // 自动查找主摄像机
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            // 初始化 Image 配置
            InitializeImage();

            // 初始隐藏
            SetVisible(false);
        }

        void Start()
        {
            // 订阅状态机事件
            if (stateMachine != null)
            {
                stateMachine.OnStateChanged += OnHarpoonStateChanged;
            }
            else
            {
                Debug.LogWarning("[CooldownRingUI] stateMachine 未绑定，UI 将无法正常工作");
            }
        }

        void OnDestroy()
        {
            // 取消订阅
            if (stateMachine != null)
            {
                stateMachine.OnStateChanged -= OnHarpoonStateChanged;
            }
        }

        void Update()
        {
            // 在 Outbound、Retracting、Cooldown 状态下更新进度和位置
            if (stateMachine != null &&
                (stateMachine.CurrentState == Harpoon.HarpoonStateMachine.State.HarpoonOutbound ||
                 stateMachine.CurrentState == Harpoon.HarpoonStateMachine.State.HarpoonRetracting ||
                 stateMachine.CurrentState == Harpoon.HarpoonStateMachine.State.Cooldown))
            {
                UpdateCooldownProgress();
                UpdatePosition();
            }
        }

        /// <summary>初始化 Image 组件配置</summary>
        void InitializeImage()
        {
            if (_image == null) return;

            // 设置为 Filled Image
            _image.type = Image.Type.Filled;
            _image.fillMethod = Image.FillMethod.Radial360;
            _image.fillOrigin = (int)Image.Origin360.Top; // 从顶部（12点方向）开始
            _image.fillClockwise = true; // 顺时针填充
            _image.fillAmount = 0f; // 初始进度为 0

            // 设置颜色
            _image.color = ringColor;

            // 使用 Unity 内置的圆形 Sprite（Knob）
            // 如果没有 Sprite，使用 Unity 默认的 UI Sprite
            if (_image.sprite == null)
            {
                _image.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            }

            if (showDebugInfo)
            {
                Debug.Log("[CooldownRingUI] Image 初始化完成");
            }
        }

        /// <summary>状态机状态变化回调</summary>
        void OnHarpoonStateChanged(Harpoon.HarpoonStateMachine.State oldState, Harpoon.HarpoonStateMachine.State newState)
        {
            switch (newState)
            {
                case Harpoon.HarpoonStateMachine.State.Ready:
                case Harpoon.HarpoonStateMachine.State.Aiming:
                    SetVisible(false);
                    break;

                case Harpoon.HarpoonStateMachine.State.HarpoonOutbound:
                case Harpoon.HarpoonStateMachine.State.HarpoonRetracting:
                case Harpoon.HarpoonStateMachine.State.Cooldown:
                    // 发射后立即显示，并开始更新进度
                    SetVisible(true);
                    break;
            }

            if (showDebugInfo)
            {
                Debug.Log($"[CooldownRingUI] 状态切换: {oldState} -> {newState}");
            }
        }

        /// <summary>更新冷却进度</summary>
        void UpdateCooldownProgress()
        {
            if (_image == null || stateMachine == null) return;

            // 获取冷却进度（0~1）
            float progress = stateMachine.CooldownProgress01;
            _image.fillAmount = progress;

            // 如果启用颜色渐变（M2 阶段功能）
            if (enableColorGradient)
            {
                _image.color = GetGradientColor(progress);
            }
        }

        /// <summary>更新 UI 位置（跟随准心）</summary>
        void UpdatePosition()
        {
            if (_rectTransform == null || aimController == null || mainCamera == null) return;

            // 获取准心的世界坐标（最后一次瞄准的目标点）
            Vector3 crosshairWorldPos = aimController.CurrentAimTargetPoint;

            // 转换为屏幕坐标
            Vector3 screenPos = mainCamera.WorldToScreenPoint(crosshairWorldPos);

            // 应用偏移
            screenPos.x += offsetFromCrosshair.x;
            screenPos.y += offsetFromCrosshair.y;

            // 设置 RectTransform 位置
            _rectTransform.position = screenPos;
        }

        /// <summary>获取渐变颜色（M2 阶段功能）</summary>
        Color GetGradientColor(float progress)
        {
            if (progress < 0.5f)
            {
                // 0~0.5：红色 -> 黄色
                return Color.Lerp(cooldownStartColor, cooldownMidColor, progress * 2f);
            }
            else
            {
                // 0.5~1.0：黄色 -> 绿色
                return Color.Lerp(cooldownMidColor, cooldownEndColor, (progress - 0.5f) * 2f);
            }
        }

        /// <summary>设置可见性</summary>
        void SetVisible(bool visible)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1f : 0f;
                _canvasGroup.interactable = visible;
                _canvasGroup.blocksRaycasts = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 350, 300, 80));
            GUILayout.Label($"<b>Cooldown Ring:</b> Visible={_canvasGroup?.alpha > 0f}", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 });
            if (stateMachine != null &&
                (stateMachine.CurrentState == Harpoon.HarpoonStateMachine.State.HarpoonOutbound ||
                 stateMachine.CurrentState == Harpoon.HarpoonStateMachine.State.HarpoonRetracting ||
                 stateMachine.CurrentState == Harpoon.HarpoonStateMachine.State.Cooldown))
            {
                GUILayout.Label($"Progress: {stateMachine.CooldownProgress01:P0}");
                GUILayout.Label($"Fill Amount: {_image?.fillAmount:F2}");
            }
            GUILayout.EndArea();
        }
    }
}
