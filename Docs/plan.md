# Quest 3 AI Spatial Assistant

## 1. 目标

开发一个 Quest 3 空间 AI 助手：

```text
Quest 3
→ Passthrough 看到现实世界
→ 用户用手柄指向并点击目标物体
→ 获取目标物体图像
→ Qwen-VL 识别物体
→ LLM 生成 3~5 个推荐问题
→ 用户用手柄点击问题
→ LLM 回答
→ Quest 3 在物体附近显示结果
```

第一版不做：机器人、遥操作、语音、复杂手势、世界模型。

---

## 2. 模型(支持本地部署和API接入)

### Qwen-VL
负责视觉：

```text
图像 → 物体名称 + 类别 + 可见属性 + 描述
```

要求结构化输出，例如：

```json
{
  "object": "冰箱",
  "category": "家用冰箱",
  "brand": null,
  "model": null,
  "visible_features": ["双开门"],
  "description": "一台双开门家用冰箱"
}
```

不要猜测图片无法确定的信息。

### LLM
负责语言交互：

```text
物体信息 → 推荐问题
物体信息 + 用户选择的问题 → 回答
```

第一版用户只点击推荐问题；自由输入后续再做。

---

## 3. Quest 3

技术栈：

- Unity + C#
- Meta Quest / Horizon OS
- OpenXR
- Passthrough
- Passthrough Camera API
- Quest 3 Controller

核心流程：

```text
手柄 Ray
→ 指向物体
→ 点击
→ 获取目标图像
→ AI 分析
```

需要实现：

- Passthrough
- 摄像头图像获取
- 手柄 Ray / 点击
- 目标物体选择
- Camera → World 坐标
- 空间 UI

目标是让 AI 信息显示在现实物体附近。

---

## 4. 官方 Sample 的使用方式

**不要直接在官方 Sample 上开发。**

官方项目仅作为 API / 技术参考：

```text
Unity-PassthroughCameraApiSamples
├── CameraViewer
├── CameraToWorld
└── MultiObjectDetection
```

先 clone 并跑通关键 Sample，理解实现方式，然后**从 0 创建自己的 Unity 项目**，只抽取需要的代码/API。

原因：官方 Sample 是功能展示工程，不是本项目架构；直接修改容易产生大量无关代码。

---

## 5. 项目架构

```text
Quest3-AI-Spatial-Assistant/
├── Assets/
│   ├── Scripts/
│   │   ├── Camera/
│   │   ├── Interaction/
│   │   ├── Spatial/
│   │   ├── AI/
│   │   ├── UI/
│   │   └── Network/
│   ├── Prefabs/
│   └── Scenes/
│
└── Backend/
    ├── vision/
    ├── llm/
    └── api/
```

Unity 不直接绑定具体模型。

Backend 提供统一接口，例如：

```text
POST /vision/analyze
POST /llm/questions
POST /llm/answer
```

支持后续在 API 与本地模型之间切换。

---

## 6. MVP 开发顺序

```text
1. Quest 3 + Unity + Passthrough
2. 手柄 Ray + 点击选择
3. 获取摄像头图像
4. 图像 → Backend → Qwen-VL
5. Qwen-VL → LLM → 推荐问题
6. 手柄点击问题 → LLM → 回答
7. Camera → World → 空间 UI
```

完成第 7 步即完成第一版 Demo。

---

## 7. 部署

推荐架构：

```text
Quest 3
   ↓ Wi-Fi
局域网
   ↓
工作站
   ├── Backend
   ├── Qwen-VL
   └── LLM
```

模型可以使用 API，也可以本地部署。

- Windows：可以
- Ubuntu：可以
- 后期大量 GPU / vLLM / ORCA：优先 Ubuntu

Quest 3 不需要 USB 有线连接工作站；正式运行可通过 Wi-Fi 通信。USB 主要用于 APK 安装、ADB 和调试。

建议工作站走网线，Quest 3 使用 5GHz Wi-Fi。

---

## 8. 后续：ORCA 世界模型

第一版：

```text
Quest 3 → Qwen-VL → LLM → UI
```

后续：

```text
Quest 3
→ 视觉 / 空间状态
→ ORCA World Model
→ 状态理解 / 预测
→ LLM / Agent
→ UI 或行动
```
