# SpatialAI Unity 项目

Quest 3 空间 AI 助手客户端

## 创建项目

### 1. 使用 Unity Hub 创建新项目

- 模板：**3D (URP)** 或 **3D Core**
- Unity 版本：**2022.3 LTS** 或更高
- 项目名称：`SpatialAI`
- 位置：`D:\桌面\code\SpatialAI\Unity\`

### 2. 安装 Meta Quest SDK

**方法 A：通过 Package Manager**

1. `Window` → `Package Manager`
2. 点击左上角 `+` → `Add package from git URL`
3. 添加以下包：
   - `com.unity.xr.openxr`
   - `com.unity.xr.management`

4. 下载 Meta XR All-in-One SDK：
   - 访问 https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657
   - 或直接下载：https://developer.oculus.com/downloads/package/unity-integration/

**方法 B：直接导入 Unity Package**

1. 下载 Meta XR All-in-One SDK `.unitypackage`
2. `Assets` → `Import Package` → `Custom Package`
3. 导入全部

### 3. 配置 Quest 3 构建设置

#### 平台切换

1. `File` → `Build Settings`
2. 选择 `Android`
3. 点击 `Switch Platform`

#### Player Settings

`Edit` → `Project Settings` → `Player`

- **Company Name**: 填写你的名称
- **Product Name**: `SpatialAI`
- **Package Name**: `com.yourname.spatialai`

#### XR Settings

`Edit` → `Project Settings` → `XR Plug-in Management`

1. 切换到 **Android** 标签页
2. 勾选 **OpenXR**
3. 在 `OpenXR` 设置中：
   - 添加 **Interaction Profiles**: `Oculus Touch Controller Profile`
   - 添加 **Feature Groups**: `Meta Quest Support`

#### Android Settings

`Edit` → `Project Settings` → `Player` → `Android` 标签页

- **Minimum API Level**: Android 10.0 (API Level 29)
- **Target API Level**: Android 12.0 (API Level 31) 或更高
- **Scripting Backend**: IL2CPP
- **Target Architectures**: ARM64

---

## 项目结构（计划）

```text
Assets/
├── Scenes/
│   └── Main.unity              # 主场景
├── Scripts/
│   ├── Core/
│   │   ├── AppManager.cs       # 应用主控制器
│   │   └── Config.cs           # 配置管理
│   ├── Camera/
│   │   ├── CameraManager.cs    # 摄像头管理
│   │   └── ImageCapture.cs     # 图像捕获
│   ├── Interaction/
│   │   ├── RayPointer.cs       # 手柄射线
│   │   └── ObjectSelector.cs   # 物体选择
│   ├── Network/
│   │   ├── APIClient.cs        # Backend API 客户端
│   │   └── Models.cs           # 数据模型
│   ├── UI/
│   │   ├── QuestionPanel.cs    # 问题面板
│   │   ├── AnswerPanel.cs      # 答案面板
│   │   └── LoadingIndicator.cs # 加载动画
│   └── Spatial/
│       └── WorldAnchor.cs      # 空间锚点
├── Prefabs/
│   ├── RayPointer.prefab
│   ├── UIPanel.prefab
│   └── LoadingSpinner.prefab
└── Materials/
    └── UI/
```

---

## 核心功能模块

### 1. Passthrough 启用

```csharp
// 在主场景启用 Passthrough
OVRPassthroughLayer passthroughLayer;
```

### 2. 手柄 Ray 交互

```text
Controller → Ray → 指向物体 → 点击 → 触发事件
```

### 3. 摄像头图像获取

```text
Quest 3 Camera API → 获取帧 → 转换为 base64
```

### 4. Backend 通信

```text
Unity → HTTP POST → Backend API → 返回 JSON
```

### 5. 空间 UI 显示

```text
Canvas → World Space → 跟随视角或固定在空间
```

---

## 开发顺序

### Phase 1: 基础环境（1-2天）

- [ ] 创建 Unity 项目
- [ ] 安装 Meta Quest SDK
- [ ] 配置构建设置
- [ ] 启用 Passthrough
- [ ] 测试部署到 Quest 3

### Phase 2: 交互系统（2天）

- [ ] 手柄输入检测
- [ ] Ray 射线实现
- [ ] 点击事件处理
- [ ] 基础 UI 框架

### Phase 3: 摄像头集成（2-3天）

- [ ] Camera API 集成
- [ ] 图像捕获
- [ ] Base64 转换
- [ ] 测试图像质量

### Phase 4: 网络通信（1-2天）

- [ ] HTTP 客户端实现
- [ ] API 调用封装
- [ ] 错误处理
- [ ] 超时重试

### Phase 5: AI 集成（2天）

- [ ] 调用 /analyze 接口
- [ ] 显示推荐问题
- [ ] 调用 /answer 接口
- [ ] 显示答案

### Phase 6: 空间 UI（2天）

- [ ] World Space Canvas
- [ ] UI 跟随逻辑
- [ ] 问题按钮交互
- [ ] Loading 动画

### Phase 7: 优化与测试（2天）

- [ ] 性能优化
- [ ] 用户体验优化
- [ ] 完整流程测试
- [ ] Bug 修复

**总计：12-15 天完成 MVP**

---

## 参考资料

- Meta Quest 开发文档：https://developer.oculus.com/documentation/unity/
- OpenXR 文档：https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest
- Passthrough API：https://developer.oculus.com/documentation/unity/unity-passthrough/
- Camera API Sample：https://github.com/oculus-samples/Unity-PassthroughCameraApiSamples

---

## 注意事项

1. **不要在官方 Sample 上直接开发**，只作参考
2. **每次修改后测试部署**，避免积累问题
3. **先用 Mock 数据测试 UI**，再连接真实 Backend
4. **保持场景简洁**，避免过多 GameObject 影响性能
