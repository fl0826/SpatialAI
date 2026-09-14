# 快速启动指南

## 第一步：配置并测试 Backend

### 1. 安装 Python 依赖

```bash
cd Backend
pip install -r requirements.txt
```

### 2. 配置 API

复制配置文件：

```bash
cp .env.example .env
```

编辑 `.env` 文件，填写你的 API 信息：

```bash
# Vision Model (Qwen-VL)
VISION_API_KEY=你的API_KEY
VISION_API_URL=https://your-api-endpoint.com/v1
VISION_MODEL_NAME=qwen-vl-plus

# Language Model (LLM)
LLM_API_KEY=你的API_KEY
LLM_API_URL=https://your-api-endpoint.com/v1
LLM_MODEL_NAME=gpt-4
```

### 3. 启动 Backend

```bash
python main.py
```

看到以下输出表示成功：

```text
✅ 配置验证通过
🚀 启动服务: http://0.0.0.0:8000
```

### 4. 测试 Backend

**新开一个终端**，运行测试脚本：

```bash
python test_api.py
```

或指定 IP（如果在另一台机器测试）：

```bash
python test_api.py http://192.168.1.100:8000
```

成功输出示例：

```text
[1] 健康检查
✅ 状态: 200

[2] 测试分析接口
✅ 识别成功:
   物体: 冰箱
   类别: 家电/制冷
   问题数量: 3

[3] 测试回答接口
✅ 回答: ...
```

---

## 第二步：创建 Unity 项目

### 1. 使用 Unity Hub 创建项目

- 打开 Unity Hub
- 点击 **New Project**
- 选择模板：**3D (URP)** 或 **3D Core**
- Unity 版本：**2022.3 LTS** 或更高
- 项目名称：`SpatialAI-Quest`
- 位置：选择 `SpatialAI/Unity/` 文件夹
- 点击 **Create**

### 2. 安装 Meta Quest SDK

#### 方法 A：通过 Package Manager

1. `Window` → `Package Manager`
2. 点击左上角 `+` → `Add package by name`
3. 依次添加：
   - `com.unity.xr.openxr`
   - `com.unity.xr.management`

4. 导入 Meta XR SDK：
   - 访问 Asset Store 搜索 "Meta XR All-in-One SDK"
   - 或访问：https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657
   - 点击 **Add to My Assets** → **Import**

#### 方法 B：直接下载 Unity Package

1. 下载 Meta XR SDK：https://developer.oculus.com/downloads/package/unity-integration/
2. Unity 中：`Assets` → `Import Package` → `Custom Package`
3. 选择下载的 `.unitypackage` 文件
4. 点击 **Import**

### 3. 配置 Quest 3 构建设置

#### 切换到 Android 平台

1. `File` → `Build Settings`
2. 选择 **Android**
3. 点击 **Switch Platform**（需要等待一段时间）

#### Player Settings

`Edit` → `Project Settings` → `Player`

在 **Android** 标签页下：

- **Company Name**: 填写你的名称
- **Product Name**: `SpatialAI`
- **Package Name**: `com.yourname.spatialai`（必须是唯一的）

**Other Settings** 区域：

- **Minimum API Level**: Android 10.0 (API Level 29)
- **Target API Level**: Android 12.0 (API Level 31) 或更高
- **Scripting Backend**: IL2CPP
- **Target Architectures**: 勾选 **ARM64**（取消勾选 ARMv7）

#### XR Settings

`Edit` → `Project Settings` → `XR Plug-in Management`

1. 点击 **Android** 标签页
2. 勾选 **OpenXR**
3. 在 `OpenXR` 设置中：
   - **Interaction Profiles**: 添加 `Oculus Touch Controller Profile`
   - **Feature Groups**: 勾选 `Meta Quest Support`

### 4. 导入项目脚本

将 `Unity/Scripts/` 文件夹复制到 Unity 项目的 `Assets/` 目录下：

```bash
# 在项目根目录执行
cp -r Unity/Scripts/ Unity/SpatialAI-Quest/Assets/
```

或手动复制 `Scripts` 文件夹到 Unity 项目的 `Assets` 目录。

### 5. 基础场景搭建（简化版）

#### 创建主场景

1. `File` → `New Scene` → `Empty Scene`
2. 保存为 `Main.unity`

#### 添加 OVR Camera Rig

1. 在 Hierarchy 中右键 → `XR` → `OVR Camera Rig`（如果有）
2. 或创建空 GameObject 命名为 `OVRCameraRig`

#### 添加核心组件

创建以下 GameObject：

1. **[AppManager]**
   - 添加 `AppManager.cs`
   - 添加 `Config.cs`
   - 添加 `APIClient.cs`
   - 在 `Config` Inspector 中设置 `Backend URL` 为你的工作站 IP

2. **CameraManager**
   - 添加 `ImageCapture.cs`

3. **ObjectSelector**
   - 添加 `ObjectSelector.cs`
   - 在 Inspector 中链接各组件引用

4. **RayPointer**（挂载到右手柄）
   - 如果有 OVRCameraRig，挂载到右手柄
   - 或创建独立 GameObject
   - 添加 `RayPointer.cs`

5. **UI Canvas**
   - 创建 Canvas（Render Mode: World Space）
   - 添加子对象并挂载 UI 脚本

### 6. 连接 Quest 3

1. **开启开发者模式**：
   - 在手机 Meta Quest 应用中启用开发者模式

2. **USB 连接**：
   - USB-C 线连接 Quest 3 和电脑
   - Quest 3 中允许 USB 调试

3. **构建并运行**：
   - `File` → `Build Settings`
   - 点击 **Build And Run**
   - 选择保存 APK 位置

---

## 第三步：完整测试

### 网络配置

1. **查看工作站 IP**

```bash
# Windows
ipconfig

# 找到局域网 IP，例如 192.168.1.100
```

2. **确保 Quest 3 在同一局域网**

在 Quest 3 浏览器中访问：`http://工作站IP:8000`

应该能看到 Backend 首页。

3. **Unity 中配置 Backend URL**

在 `Config` 组件的 Inspector 中填写：

```text
http://192.168.1.100:8000
```

### 测试流程

1. ✅ Backend 在工作站运行
2. ✅ Quest 3 部署了 Unity 应用
3. ✅ 两者在同一局域网
4. ✅ Unity 配置了正确的 Backend URL

**开始测试**：

1. 戴上 Quest 3，启动应用
2. 进入 Passthrough 模式（看到现实环境）
3. 用手柄指向一个物体
4. 按下触发器
5. 等待 Loading
6. 查看推荐问题
7. 点击一个问题
8. 查看答案

---

## 常见问题

### Backend 相关

**Q: 提示缺少配置项**
```bash
❌ 配置错误: 缺少配置项: VISION_API_KEY
```

A: 检查 `.env` 文件是否存在，API Key 是否正确填写

**Q: API 调用失败**

A: 检查 API URL 和 Model Name 是否正确

### Unity 相关

**Q: 找不到 Meta Quest SDK**

A: 确保安装了 OpenXR 和 Meta XR SDK

**Q: 构建失败**

A: 检查 Android SDK、NDK、JDK 是否正确配置

**Q: Quest 3 无法连接**

A: 确保开启了开发者模式，USB 调试已允许

### 网络相关

**Q: Quest 3 无法访问 Backend**

A: 
1. 检查防火墙是否允许 8000 端口
2. 确保两者在同一局域网
3. 尝试 ping 工作站 IP

---

## 下一步优化

完成 MVP 后可以考虑：

1. 集成真实的 Quest Camera API（替换 ReadPixels）
2. 优化 UI 设计和交互
3. 添加空间锚点（UI 固定在物体附近）
4. 支持语音输入
5. 本地缓存识别过的物体
6. 集成 ORCA 世界模型

---

更多详细信息请参考各模块的文档：

- `Backend/DEPLOY.md`
- `Unity/PROJECT_SETUP.md`
- `Docs/plan.md`
