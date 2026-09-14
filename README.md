# SpatialAI

Quest 3 空间 AI 助手 - 指向现实物体，AI 识别并回答问题

## 功能流程

```text
① Quest 3 Passthrough 看到现实环境
        ↓
② 用户用手柄指向物体并点击
        ↓
③ 捕获图像
        ↓
④ Qwen-VL 识别物体
        ↓
⑤ LLM 生成 3-5 个推荐问题
        ↓
⑥ 用户点击选择问题
        ↓
⑦ LLM 回答
        ↓
⑧ Quest 3 空间 UI 显示答案
```

## 项目结构

```text
SpatialAI/
├── Backend/        # AI 后端服务（Python FastAPI）
│   ├── vision/     # Qwen-VL 视觉识别
│   ├── llm/        # 语言模型服务
│   ├── main.py     # FastAPI 主服务
│   └── config.py   # 配置管理
├── Unity/          # Quest 3 应用（Unity 项目）
│   ├── Scripts/    # C# 脚本
│   │   ├── Core/
│   │   ├── Network/
│   │   ├── Camera/
│   │   ├── Interaction/
│   │   └── UI/
│   └── PROJECT_SETUP.md
└── Docs/
    └── plan.md     # 详细开发计划
```

## 快速开始

### 1. Backend 部署

```bash
cd Backend

# 安装依赖
pip install -r requirements.txt

# 配置 API
cp .env.example .env
# 编辑 .env 填写你的 API Key

# 启动服务
python main.py
```

详见 `Backend/DEPLOY.md`

### 2. Unity 项目

1. 使用 Unity Hub 创建新项目（Unity 2022.3 LTS）
2. 安装 Meta Quest SDK
3. 导入 `Unity/Scripts/` 下的所有脚本
4. 配置场景和 UI
5. 构建并部署到 Quest 3

详见 `Unity/PROJECT_SETUP.md`

### 3. 测试连接

```bash
# 测试 Backend API
python Backend/test_api.py

# 或指定工作站 IP
python Backend/test_api.py http://192.168.1.100:8000
```

## 环境要求

### 硬件

- **Quest 3**（Wi-Fi 连接，可访问局域网）
- **工作站**（显存 16GB+，网线连接）

### 软件

- Python 3.10+
- Unity 2022.3 LTS+
- Meta Quest SDK
- Qwen-VL API / LLM API

## 网络配置

```text
Quest 3（Wi-Fi 5GHz）
   ↓
局域网路由器
   ↓
工作站（有线网络）
   ├── Backend API (端口 8000)
   ├── Qwen-VL
   └── LLM
```

Unity 中配置 Backend URL：`http://工作站IP:8000`

## 开发状态

- ✅ Backend API 框架完成
- ✅ Unity 脚本框架完成
- ⏳ Unity 场景搭建（待进行）
- ⏳ Quest Camera API 集成（待进行）
- ⏳ 完整流程测试（待进行）

## 下一步

1. **配置 Backend**：填写 `.env` 中的 API 信息
2. **测试 Backend**：运行 `test_api.py` 确保服务正常
3. **创建 Unity 项目**：按照 `Unity/PROJECT_SETUP.md` 操作
4. **导入脚本**：将 `Unity/Scripts/` 导入项目
5. **搭建场景**：配置 OVRCameraRig + UI
6. **部署测试**：构建到 Quest 3 测试完整流程

## 文档

- 详细计划：`Docs/plan.md`
- Backend 部署：`Backend/DEPLOY.md`
- Unity 设置：`Unity/PROJECT_SETUP.md`
- 脚本说明：`Unity/Scripts/README.md`
