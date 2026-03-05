# 绳索 LineRenderer 材质配置说明

**任务编号**：ART-M1S4-ROPE-003
**创建日期**：2026-03-05
**材质路径**：`Assets/FishCargo/Art/Materials/S4_Rope/Mat_Rope_v01.mat`

---

## 一、材质基本信息

### 1.1 材质配置

- **材质名称**：`Mat_Rope_v01`
- **Shader**：URP/Lit (GUID: 933532a4fcc9baf4fa0491de14d08ed7)
- **渲染类型**：Opaque（不透明）
- **版本**：v01（占位版本）

### 1.2 颜色配置

- **Base Color**：RGB(200, 200, 200) = (0.784, 0.784, 0.784)
- **颜色说明**：灰白色，与鱼叉头颜色统一
- **Alpha**：1.0（完全不透明）

### 1.3 材质属性

| 属性 | 值 | 说明 |
|------|-----|------|
| Metallic | 0.0 | 非金属材质 |
| Smoothness | 0.0 | 完全粗糙，无高光 |
| Receive Shadows | 1 | 接收阴影 |
| Specular Highlights | 1 | 启用高光（但 Smoothness=0 实际无高光） |
| Environment Reflections | 1 | 启用环境反射（但 Smoothness=0 实际无反射） |

---

## 二、LineRenderer 配置指南

### 2.1 基础配置

在 Unity 中为 LineRenderer 组件配置此材质：

```
Component: Line Renderer
Materials:
  - Element 0: Mat_Rope_v01

Width:
  - Start Width: 0.05
  - End Width: 0.05

Positions:
  - Position Count: 2
  - Position 0: (起点) HarpoonMuzzleCurrent.position
  - Position 1: (终点) HarpoonProjectile.HarpoonTailPosition

Color Gradient:
  - Start Color: RGB(200, 200, 200)
  - End Color: RGB(200, 200, 200)

Texture Mode: Stretch
Alignment: View
```

### 2.2 程序接入步骤

1. 在 `HarpoonContainer` 节点上添加 `Line Renderer` 组件
2. 将 `Mat_Rope_v01` 拖拽到 `Materials` 数组的 Element 0
3. 设置 `Width` 为 0.05（起点和终点相同）
4. 设置 `Position Count` 为 2
5. 在 `HarpoonRope.cs` 脚本中动态更新两个端点位置：
   - Position 0：`harpoonMuzzleCurrent.position`（船体锚点）
   - Position 1：`projectile.HarpoonTailPosition`（鱼叉尾部）

### 2.3 动态更新示例

```csharp
// 示例代码（程序参考）
LineRenderer lineRenderer = GetComponent<LineRenderer>();

void Update()
{
    if (isRopeActive)
    {
        // 起点：船体发射点（动态跟随船体移动）
        lineRenderer.SetPosition(0, harpoonMuzzleCurrent.position);

        // 终点：鱼叉尾部（跟随鱼叉移动）
        lineRenderer.SetPosition(1, projectile.HarpoonTailPosition);
    }
}
```

---

## 三、像素化渲染适配

### 3.1 渲染环境

- **渲染分辨率**：320×180 RenderTexture
- **放大倍率**：12x Point filtering
- **绳索宽度**：0.05 单位 ≈ 0.5 像素（在 320×180 分辨率下）

### 3.2 可读性验证

**优点：**
- ✅ 灰白色 RGB(200, 200, 200) 在深色海洋背景下清晰可见
- ✅ 0.05 单位宽度在像素化后约 0.5 像素，细而不失可见性
- ✅ URP/Lit Shader 在像素化渲染下表现稳定

**潜在问题：**
- ⚠️ 0.05 单位宽度可能在某些角度下过细，若不可见可调整为 0.08~0.10
- ⚠️ 若绳索与背景对比度不足，可考虑添加描边或调整颜色

**调整建议：**
- 若绳索过细不可见：增加 `Width` 到 0.08 或 0.10
- 若绳索与背景融合：调整颜色为更亮的白色 RGB(255, 255, 255)
- 若需要更强对比：添加黑色描边（需自定义 Shader）

---

## 四、材质优化建议

### 4.1 简化 Shader（可选）

当前使用 URP/Lit，包含光照计算。若性能有压力，可简化为 URP/Unlit：

**URP/Unlit 配置：**
- Shader：Universal Render Pipeline/Unlit
- Base Color：RGB(200, 200, 200)
- 优点：性能更好，无光照计算开销
- 缺点：无阴影和光照效果（但绳索通常不需要）

### 4.2 颜色方案调整（可选）

**方案 A：纯白色（高对比度）**
- Base Color：RGB(255, 255, 255)
- 优点：在任何背景下最清晰
- 缺点：可能过于刺眼

**方案 B：浅灰色（当前方案）**
- Base Color：RGB(200, 200, 200)
- 优点：柔和，与鱼叉颜色统一
- 缺点：在浅色背景下对比度略低

**方案 C：渐变色（M2 阶段）**
- 起点：RGB(200, 200, 200)
- 终点：RGB(150, 150, 150)
- 优点：增加视觉层次感
- 实现：通过 LineRenderer 的 Color Gradient 配置

---

## 五、验收标准

### 5.1 功能验收

- [x] 材质文件已创建：`Mat_Rope_v01.mat`
- [x] Shader 配置正确：URP/Lit
- [x] 颜色配置正确：RGB(200, 200, 200)
- [x] 材质属性合理：Metallic=0, Smoothness=0
- [ ] 程序已接入 LineRenderer（待程序验证）
- [ ] 绳索端点正确跟随船体锚点与鱼叉尾部（待联调验证）

### 5.2 视觉验收

- [ ] 绳索在像素化渲染下清晰可见
- [ ] 绳索宽度适中（0.05 单位），不过粗或过细
- [ ] 绳索颜色与鱼叉颜色统一（灰白色）
- [ ] 绳索在任何背景下可读性良好
- [ ] 绳索端点无跳点或断连现象

### 5.3 性能验收

- [ ] LineRenderer 在 60 FPS 下稳定运行
- [ ] 材质 Shader 复杂度在 URP 标准范围内
- [ ] 无明显性能瓶颈

---

## 六、程序接入说明

### 6.1 HarpoonRope 组件配置

根据程序设计文档，`HarpoonRope.cs` 组件需要以下配置：

```
Component: HarpoonRope (Script)
  - Harpoon Muzzle Current: HarpoonMuzzleCurrent (Transform)
  - Projectile: HarpoonProjectile (Component)
  - Show Debug Info: true (勾选)

Component: Line Renderer
  - Materials: Mat_Rope_v01
  - Width: 0.05
  - Position Count: 2
```

### 6.2 绳索显示时机

- **Ready 状态**：绳索隐藏（LineRenderer.enabled = false）
- **Aiming 状态**：绳索隐藏
- **HarpoonOutbound 状态**：绳索显示，起点跟随船体，终点跟随鱼叉
- **HarpoonRetracting 状态**：绳索显示，起点跟随船体，终点跟随鱼叉
- **Cooldown 状态**：绳索隐藏

### 6.3 动态回收锚点验证

**关键验证点：**
1. 船体静止发射：绳索起点固定在船体锚点
2. 船体移动发射：绳索起点实时跟随船体移动（不回旧发射点）
3. 鱼叉飞出：绳索终点跟随鱼叉头部
4. 鱼叉回收：绳索终点跟随鱼叉头部，起点跟随船体当前位置
5. 无跳点或断连：绳索端点每帧更新，无明显视觉错位

---

## 七、后续迭代计划

### M2 阶段
- 添加绳索渐变色（起点浅，终点深）
- 优化绳索宽度曲线（起点粗，终点细）
- 添加绳索张力表现（拉伸时变细）

### M3 阶段
- 添加绳索物理效果（轻微摆动）
- 添加绳索粒子特效（水花飞溅）
- 优化绳索材质（法线贴图、高光）

### M4 阶段
- 最终美术定稿
- 完整动画打磨
- 性能优化

---

## 八、已知限制

### 8.1 当前限制

- **静态颜色**：当前为纯色，无渐变或纹理
- **无物理效果**：绳索为直线，无摆动或弯曲
- **无张力表现**：绳索宽度固定，不随拉伸变化

### 8.2 M1 阶段可接受

以上限制在 M1 阶段可接受，优先满足功能验证需求。后续迭代将逐步优化。

---

## 九、资产交付清单

**已交付：**
- ✅ `Mat_Rope_v01.mat` - 绳索材质文件
- ✅ `Mat_Rope_v01.mat.meta` - Unity 元数据文件
- ✅ 材质配置说明文档

**待程序接入：**
- ⚠️ LineRenderer 组件配置
- ⚠️ HarpoonRope 脚本绑定
- ⚠️ 动态端点更新逻辑

**待联调验证：**
- ⚠️ 绳索端点跟随正确性
- ⚠️ 动态回收锚点验证
- ⚠️ 像素化渲染可读性验证

---

**交付人**：美术 Agent
**接收人**：程序 Agent
**版本**：v1.0
**状态**：已交付，待程序接入
