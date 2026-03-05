# Spr_CooldownRing_v01 制作指南

**文档编号**：DOC-ART-M1-UI-002
**版本**：v1.0
**创建日期**：2026-03-05
**资产类型**：UI Sprite（冷却圆环进度条）

---

## 一、资产基本信息

- **文件名**：`Spr_CooldownRing_v01.png`
- **保存路径**：`Assets/FishCargo/Art/UI/S4_AimingSystem/Spr_CooldownRing_v01.png`
- **尺寸**：64×64 像素
- **格式**：PNG（RGBA，支持透明）
- **用途**：冷却系统进度条，使用 Unity UI Image Filled (Radial 360) 显示冷却进度

---

## 二、视觉设计规格

### 2.1 画布设置

```
宽度：64 像素
高度：64 像素
背景：透明
颜色模式：RGBA
```

### 2.2 圆环设计规格

**外圆：**
- 直径：60 像素
- 圆心位置：(32, 32) - 画布中心
- 距离边缘：2 像素（留白）

**内圆：**
- 直径：48 像素
- 圆心位置：(32, 32) - 与外圆同心

**环宽：**
- 宽度：6 像素（外圆半径 30px - 内圆半径 24px）

### 2.3 颜色方案

**底层圆环（完整圆环）：**
- 颜色：灰色 RGB(80, 80, 80)
- 不透明度：100%
- 用途：显示冷却未完成部分

**进度填充（白色圆环）：**
- 颜色：白色 RGB(255, 255, 255)
- 不透明度：100%
- 用途：Unity Filled Image 会根据 Fill Amount 显示此部分

**背景：**
- 透明（Alpha = 0）

### 2.4 设计要点

1. **完整圆环**：必须是完整的 360° 圆环，Unity 会通过 Filled Image 控制显示部分
2. **同心圆**：内外圆必须完全同心，确保环宽均匀
3. **像素对齐**：所有边缘必须对齐像素网格，避免模糊
4. **无抗锯齿**：使用硬边缘（Hard Edge），不使用抗锯齿，保持像素风格

---

## 三、制作步骤（Photoshop）

### 步骤 1：创建画布

1. 新建文件：64×64 像素
2. 背景内容：透明
3. 颜色模式：RGB 颜色，8 位

### 步骤 2：绘制灰色底层圆环

1. 选择"椭圆工具"（U）
2. 设置前景色为 RGB(80, 80, 80)
3. 按住 Shift 键，从 (2, 2) 拖动到 (62, 62)，绘制 60×60 像素的正圆
4. 填充为灰色
5. 使用"选择 > 修改 > 收缩"，收缩 6 像素
6. 按 Delete 删除内部，形成 6 像素宽的圆环

### 步骤 3：绘制白色进度圆环

1. 复制灰色圆环图层
2. 将颜色改为白色 RGB(255, 255, 255)
3. 合并两个图层（或保持分层，方便后续调整）

### 步骤 4：检查与导出

1. 放大到 800% 检查边缘是否对齐像素网格
2. 确认背景透明
3. 文件 > 导出 > 导出为 PNG
4. 设置：
   - 格式：PNG-24
   - 透明度：勾选
   - 交错：不勾选
5. 保存到：`Assets/FishCargo/Art/UI/S4_AimingSystem/Spr_CooldownRing_v01.png`

---

## 四、制作步骤（GIMP）

### 步骤 1：创建画布

1. 文件 > 新建图像
2. 宽度：64，高度：64
3. 高级选项 > 填充：透明度

### 步骤 2：绘制灰色底层圆环

1. 选择"椭圆选择工具"（E）
2. 设置"固定"为"宽高比 1:1"
3. 从 (2, 2) 拖动到 (62, 62)，创建 60×60 像素的圆形选区
4. 设置前景色为 RGB(80, 80, 80)
5. 编辑 > 填充前景色
6. 选择 > 收缩，收缩量：6 像素
7. 按 Delete 删除内部

### 步骤 3：绘制白色进度圆环

1. 图层 > 复制图层
2. 颜色 > 色调-饱和度，调整为白色 RGB(255, 255, 255)
3. 合并图层

### 步骤 4：检查与导出

1. 视图 > 缩放 > 800%，检查边缘
2. 文件 > 导出为
3. 选择文件类型：PNG 图像
4. 保存选项：
   - 交错：不勾选
   - 保存背景色：不勾选
5. 导出到：`Assets/FishCargo/Art/UI/S4_AimingSystem/Spr_CooldownRing_v01.png`

---

## 五、制作步骤（Aseprite）

### 步骤 1：创建画布

1. 文件 > 新建
2. 宽度：64，高度：64
3. 颜色模式：RGBA
4. 背景：透明

### 步骤 2：绘制灰色底层圆环

1. 选择"圆形工具"（Shift+U）
2. 设置前景色为 RGB(80, 80, 80)
3. 设置"填充"为"无"，"描边"为 6 像素
4. 按住 Shift 键，从中心 (32, 32) 绘制半径 30 像素的圆
5. 使用"填充工具"填充圆环

### 步骤 3：绘制白色进度圆环

1. 新建图层
2. 设置前景色为白色 RGB(255, 255, 255)
3. 重复步骤 2 的操作，绘制白色圆环
4. 合并图层

### 步骤 4：检查与导出

1. 视图 > 网格 > 显示网格，检查像素对齐
2. 文件 > 导出
3. 格式：PNG
4. 保存到：`Assets/FishCargo/Art/UI/S4_AimingSystem/Spr_CooldownRing_v01.png`

---

## 六、Unity 导入设置

制作完成后，将 PNG 文件导入 Unity，并设置以下导入参数：

```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Point (no filter)  ← 关键设置，保持像素风格
Compression: None
Max Size: 2048
Wrap Mode: Clamp
```

**设置步骤：**
1. 在 Unity Project 窗口中选中 `Spr_CooldownRing_v01.png`
2. 在 Inspector 窗口中修改上述参数
3. 点击"Apply"应用设置

---

## 七、程序接入配置

### 7.1 Canvas UI 配置

**Canvas 设置：**
```
Render Mode: Screen Space - Overlay
Canvas Scaler: Scale With Screen Size
Reference Resolution: 1920×1080
Match: 0.5
```

**UI Image 配置：**
```
GameObject 名称: CooldownRing
父节点: Canvas
RectTransform:
  - Anchor: Bottom Center
  - Anchor Position: (0, 80, 0)
  - Size: (64, 64)

Image 组件:
  - Source Image: Spr_CooldownRing_v01
  - Image Type: Filled
  - Fill Method: Radial 360
  - Fill Origin: Top (12点方向)
  - Clockwise: Yes
  - Fill Amount: 0.0 ~ 1.0 (程序控制)
```

### 7.2 显示逻辑

**显示时机：**
- Ready 状态：隐藏
- Aiming 状态：隐藏
- HarpoonOutbound 状态：显示，Fill Amount = 0
- HarpoonRetracting 状态：显示，Fill Amount = 0
- Cooldown 状态：显示，Fill Amount = currentTime / totalTime

**进度计算示例代码：**
```csharp
// 在 HarpoonStateMachine 或 UI 控制脚本中
float cooldownProgress = currentCooldownTime / PostRetractCooldown;
cooldownRingImage.fillAmount = cooldownProgress;
```

---

## 八、验收标准

### 8.1 视觉验收

- [ ] 圆环完整，无缺口
- [ ] 圆环同心，环宽均匀（6 像素）
- [ ] 边缘清晰，无模糊或抗锯齿
- [ ] 背景完全透明
- [ ] 颜色正确：灰色 RGB(80, 80, 80) + 白色 RGB(255, 255, 255)

### 8.2 技术验收

- [ ] 文件尺寸：64×64 像素
- [ ] 文件格式：PNG（RGBA）
- [ ] Unity 导入设置正确（Filter Mode: Point）
- [ ] 文件大小合理（< 5KB）

### 8.3 功能验收

- [ ] Unity UI Image 可正确显示
- [ ] Filled Image Radial 360 填充方向正确（顺时针，从顶部开始）
- [ ] Fill Amount 从 0 到 1 变化流畅，无跳帧
- [ ] 在任何背景下可读性良好
- [ ] 尺寸适中，不遮挡游戏画面

---

## 九、常见问题

### Q1：圆环边缘模糊怎么办？

**原因**：使用了抗锯齿或 Filter Mode 设置错误

**解决方案**：
1. 制作时关闭抗锯齿，使用硬边缘
2. Unity 导入时设置 Filter Mode 为 Point (no filter)

### Q2：圆环不同心怎么办？

**原因**：绘制时未对齐中心点

**解决方案**：
1. 使用参考线标记画布中心 (32, 32)
2. 使用椭圆工具时按住 Shift 键确保正圆
3. 使用"对齐到中心"功能

### Q3：Unity 中 Filled Image 填充方向错误？

**原因**：Fill Origin 设置错误

**解决方案**：
1. 确认 Fill Method 为 Radial 360
2. 确认 Fill Origin 为 Top（12点方向）
3. 确认 Clockwise 为 Yes

### Q4：圆环在 Unity 中显示不完整？

**原因**：RectTransform Size 设置错误

**解决方案**：
1. 确认 RectTransform Size 为 (64, 64)
2. 确认 Anchor 设置正确
3. 确认 Canvas Scaler 设置正确

---

## 十、参考资料

### 10.1 相关文档

- `FishCargo_UI规范_M1_瞄准与蓄力UI视觉规范.md` v1.0 - 第四章
- `FishCargo_系统设计_M1S4渔船瞄准操作手册.md` v1.6
- `ART-M1S4-SUMMARY-004_S4美术任务完成总结.md` v2.1

### 10.2 Unity 官方文档

- [UI Image Component](https://docs.unity3d.com/Manual/script-Image.html)
- [Filled Image Types](https://docs.unity3d.com/Manual/script-Image.html#filled)
- [Canvas Scaler](https://docs.unity3d.com/Manual/script-CanvasScaler.html)

---

## 十一、设计预览（ASCII 示意图）

```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                        ████████████                            │
│                    ████            ████                        │
│                  ██                    ██                      │
│                ██                        ██                    │
│              ██                            ██                  │
│              ██                            ██                  │
│            ██                                ██                │
│            ██                                ██                │
│            ██                                ██                │
│            ██                                ██                │
│            ██                                ██                │
│            ██                                ██                │
│            ██                                ██                │
│            ██                                ██                │
│              ██                            ██                  │
│              ██                            ██                  │
│                ██                        ██                    │
│                  ██                    ██                      │
│                    ████            ████                        │
│                        ████████████                            │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
│                                                                │
└────────────────────────────────────────────────────────────────┘
64×64 像素画布，圆环直径 60px，环宽 6px
灰色底层 RGB(80, 80, 80) + 白色填充 RGB(255, 255, 255)
```

---

## 十二、制作检查清单

制作完成后，请逐项检查：

- [ ] 文件名正确：`Spr_CooldownRing_v01.png`
- [ ] 文件路径正确：`Assets/FishCargo/Art/UI/S4_AimingSystem/`
- [ ] 画布尺寸：64×64 像素
- [ ] 背景透明
- [ ] 圆环完整（360°）
- [ ] 圆环同心
- [ ] 外圆直径：60 像素
- [ ] 内圆直径：48 像素
- [ ] 环宽：6 像素
- [ ] 颜色正确：灰色 RGB(80, 80, 80) + 白色 RGB(255, 255, 255)
- [ ] 边缘清晰，无抗锯齿
- [ ] 已导入 Unity
- [ ] Unity 导入设置正确（Filter Mode: Point）
- [ ] 已在 Unity 中测试显示效果

---

**文档作者**：美术 Agent
**版本**：v1.0
**状态**：已完成
**创建日期**：2026-03-05
