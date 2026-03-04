using UnityEngine;
using System.Text;

namespace FishCargo.Runtime.Tools
{
    /// <summary>
    /// 调试信息叠加层
    /// 统一输出调试信息，避免散落在各脚本中
    /// 打包时可一键关闭
    /// </summary>
    public class DebugOverlay : MonoBehaviour
    {
        [Header("显示配置")]
        [Tooltip("是否显示调试信息")]
        public bool showDebugInfo = true;

        [Tooltip("文本颜色")]
        public Color textColor = Color.white;

        [Tooltip("文本大小")]
        public int fontSize = 14;

        [Tooltip("显示位置")]
        public TextAnchor alignment = TextAnchor.UpperLeft;

        [Header("性能监控")]
        [Tooltip("是否显示 FPS")]
        public bool showFPS = true;

        [Tooltip("是否显示内存使用")]
        public bool showMemory = false;

        private StringBuilder debugText = new StringBuilder();
        private float deltaTime = 0f;
        private GUIStyle guiStyle;

        void Update()
        {
            if (!showDebugInfo) return;

            // 计算 FPS
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if (!showDebugInfo) return;

            // 初始化 GUI 样式
            if (guiStyle == null)
            {
                guiStyle = new GUIStyle(GUI.skin.label);
                guiStyle.fontSize = fontSize;
                guiStyle.normal.textColor = textColor;
                guiStyle.alignment = alignment;
            }

            // 构建调试文本
            debugText.Clear();

            if (showFPS)
            {
                float fps = 1.0f / deltaTime;
                debugText.AppendLine($"FPS: {fps:F1}");
            }

            if (showMemory)
            {
                float memoryMB = System.GC.GetTotalMemory(false) / (1024f * 1024f);
                debugText.AppendLine($"Memory: {memoryMB:F2} MB");
            }

            // 显示调试信息
            Rect rect = GetDebugRect();
            GUI.Label(rect, debugText.ToString(), guiStyle);
        }

        /// <summary>
        /// 获取调试信息显示区域
        /// </summary>
        Rect GetDebugRect()
        {
            float width = 300f;
            float height = 200f;
            float padding = 10f;

            switch (alignment)
            {
                case TextAnchor.UpperLeft:
                    return new Rect(padding, padding, width, height);
                case TextAnchor.UpperRight:
                    return new Rect(Screen.width - width - padding, padding, width, height);
                case TextAnchor.LowerLeft:
                    return new Rect(padding, Screen.height - height - padding, width, height);
                case TextAnchor.LowerRight:
                    return new Rect(Screen.width - width - padding, Screen.height - height - padding, width, height);
                default:
                    return new Rect(padding, padding, width, height);
            }
        }

        /// <summary>
        /// 添加自定义调试信息
        /// </summary>
        public void AddDebugInfo(string key, string value)
        {
            if (!showDebugInfo) return;
            debugText.AppendLine($"{key}: {value}");
        }

        /// <summary>
        /// 切换调试信息显示
        /// </summary>
        public void ToggleDebugInfo()
        {
            showDebugInfo = !showDebugInfo;
        }
    }
}
