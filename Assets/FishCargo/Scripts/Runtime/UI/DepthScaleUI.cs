using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace FishCargo.Runtime.UI
{
    /// <summary>
    /// 深度刻度 UI 显示组件
    /// 按策划文档规范：海平线为 0m，向下为正深度，主刻度间隔 10m，次刻度 5m（可选）
    /// </summary>
    public class DepthScaleUI : MonoBehaviour
    {
        [Header("刻度配置")]
        [Tooltip("主刻度间隔（米）")]
        public float mainScaleInterval = 10f;

        [Tooltip("是否显示次刻度")]
        public bool showSubScale = false;

        [Tooltip("次刻度间隔（米）")]
        public float subScaleInterval = 5f;

        [Tooltip("最大深度（米）")]
        public float maxDepth = 100f;

        [Header("显示配置")]
        [Tooltip("刻度文本预制体")]
        public GameObject scaleTextPrefab;

        [Tooltip("刻度容器")]
        public Transform scaleContainer;

        [Tooltip("文本颜色")]
        public Color textColor = Color.white;

        [Tooltip("文本大小")]
        public int fontSize = 16;

        [Header("跟随配置")]
        [Tooltip("相机引用")]
        public Camera mainCamera;

        [Tooltip("海平线世界坐标 Y 值")]
        public float seaLevelY = 0f;

        private List<GameObject> scaleObjects = new List<GameObject>();

        void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            GenerateScales();
        }

        void Update()
        {
            UpdateScalePositions();
        }

        /// <summary>
        /// 生成刻度对象
        /// </summary>
        void GenerateScales()
        {
            // 清理旧刻度
            foreach (var obj in scaleObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            scaleObjects.Clear();

            // 生成主刻度
            for (float depth = 0; depth <= maxDepth; depth += mainScaleInterval)
            {
                CreateScaleText(depth, true);
            }

            // 生成次刻度（可选）
            if (showSubScale)
            {
                for (float depth = subScaleInterval; depth < maxDepth; depth += mainScaleInterval)
                {
                    if (depth % mainScaleInterval != 0)
                    {
                        CreateScaleText(depth, false);
                    }
                }
            }
        }

        /// <summary>
        /// 创建单个刻度文本
        /// </summary>
        void CreateScaleText(float depth, bool isMainScale)
        {
            GameObject scaleObj;

            if (scaleTextPrefab != null)
            {
                scaleObj = Instantiate(scaleTextPrefab, scaleContainer);
            }
            else
            {
                // 如果没有预制体，创建默认文本对象
                scaleObj = new GameObject($"Scale_{depth}m");
                scaleObj.transform.SetParent(scaleContainer);

                Text textComponent = scaleObj.AddComponent<Text>();
                textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                textComponent.fontSize = isMainScale ? fontSize : fontSize - 2;
                textComponent.color = textColor;
                textComponent.alignment = TextAnchor.MiddleLeft;
            }

            // 设置文本内容
            Text text = scaleObj.GetComponent<Text>();
            if (text != null)
            {
                text.text = $"{depth}m";
            }

            // 存储深度信息
            DepthScaleData data = scaleObj.AddComponent<DepthScaleData>();
            data.depth = depth;
            data.isMainScale = isMainScale;

            scaleObjects.Add(scaleObj);
        }

        /// <summary>
        /// 更新刻度位置（跟随相机，动态显隐）
        /// </summary>
        void UpdateScalePositions()
        {
            if (mainCamera == null) return;

            float camY = mainCamera.transform.position.y;
            float halfHeight = mainCamera.orthographicSize;
            float visibleTop = camY + halfHeight;
            float visibleBottom = camY - halfHeight;

            foreach (var scaleObj in scaleObjects)
            {
                if (scaleObj == null) continue;

                DepthScaleData data = scaleObj.GetComponent<DepthScaleData>();
                if (data == null) continue;

                // 计算世界坐标：海平线向下为正深度
                float worldY = seaLevelY - data.depth;

                // 不在视野内则隐藏
                bool inView = worldY <= visibleTop && worldY >= visibleBottom;
                scaleObj.SetActive(inView);
                if (!inView) continue;

                // 转换为屏幕坐标
                Vector3 worldPos = new Vector3(mainCamera.transform.position.x - 8f, worldY, 0);
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

                // 设置 UI 位置
                RectTransform rectTransform = scaleObj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.position = screenPos;
                }
            }
        }

        /// <summary>
        /// 运行时重新生成刻度（参数变更时调用）
        /// </summary>
        public void RegenerateScales()
        {
            GenerateScales();
        }
    }

    /// <summary>
    /// 刻度数据组件
    /// </summary>
    public class DepthScaleData : MonoBehaviour
    {
        public float depth;
        public bool isMainScale;
    }
}
