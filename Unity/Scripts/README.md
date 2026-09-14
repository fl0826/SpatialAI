# Unity Scripts 说明

## 脚本结构

```text
Scripts/
├── Core/
│   ├── Config.cs           # 全局配置（Backend URL等）
│   └── AppManager.cs       # 应用主控制器
├── Network/
│   ├── Models.cs           # API 数据模型
│   └── APIClient.cs        # Backend API 客户端
├── Camera/
│   └── ImageCapture.cs     # 图像捕获
├── Interaction/
│   ├── RayPointer.cs       # 手柄射线
│   └── ObjectSelector.cs   # 物体选择逻辑
└── UI/
    ├── QuestionPanel.cs    # 问题面板
    └── LoadingIndicator.cs # 加载动画
```

## 核心流程

### 1. 初始化

`AppManager` → 初始化所有组件 → 读取 `Config`

### 2. 用户交互

```text
RayPointer (手柄输入)
    ↓ 触发器按下
ObjectSelector (协调器)
    ↓ 捕获图像
ImageCapture
    ↓ 调用 API
APIClient → Backend
    ↓ 返回结果
QuestionPanel (显示问题)
    ↓ 用户点击问题
APIClient → Backend
    ↓ 返回答案
QuestionPanel (显示答案)
```

## 如何使用

### 场景设置

1. **创建空 GameObject 命名为 `[AppManager]`**
   - 添加 `AppManager.cs`
   - 添加 `Config.cs`（或作为子对象）
   - 添加 `APIClient.cs`（或作为子对象）

2. **创建摄像头管理器**
   - 新建 GameObject 命名为 `CameraManager`
   - 添加 `ImageCapture.cs`

3. **创建手柄射线（挂载到 Controller）**
   - 在 OVRCameraRig 的右手柄上添加 `RayPointer.cs`
   - 或创建独立 GameObject 并设置为手柄子对象

4. **创建物体选择器**
   - 新建 GameObject 命名为 `ObjectSelector`
   - 添加 `ObjectSelector.cs`
   - 在 Inspector 中链接各组件引用

5. **创建 UI Canvas**
   - Canvas Render Mode: `World Space`
   - 添加 UI 元素并挂载对应脚本

### 配置 Backend URL

在 `Config` 组件的 Inspector 中修改 `Backend URL`：

```text
http://192.168.1.100:8000
```

（替换为你的工作站 IP）

## MVP 简化实现

当前脚本是**完整框架**，但有些功能可以先用简化方式实现：

### 图像捕获

`ImageCapture.cs` 目前使用 `ReadPixels` 从屏幕捕获，后续需要集成 Quest Camera API

### UI 创建

可以先在场景手动创建 UI，后续再用预制体动态生成

### 测试流程

1. **不连接 Backend 测试**：注释掉 API 调用，直接模拟返回数据
2. **连接 Backend Mock 测试**：Backend 返回固定 JSON
3. **完整测试**：真实 API + Quest 3 真机

## 注意事项

1. **命名空间**：所有脚本都在 `SpatialAI.*` 命名空间下
2. **依赖关系**：确保 Inspector 中正确链接组件引用
3. **调试**：开启 `Config.enableDebugLog` 查看详细日志
4. **异常处理**：所有 API 调用都有错误回调

## 下一步

1. 在 Unity 中创建场景并添加这些脚本
2. 配置 OVRCameraRig + Passthrough
3. 创建基础 UI 布局
4. 测试手柄输入
5. 连接 Backend 测试完整流程
