# SpatialAI

Quest 3 空间 AI 助手 - 拍照识别物体并智能问答

## 功能流程

```
拍照 → 识别物体 → 生成问题 → 选择问题 → 显示答案
```

## 项目结构

```
SpatialAI/
├── Backend/              # Python FastAPI 后端
│   ├── vision/          # 视觉识别服务 (DeepSeek Vision)
│   ├── llm/             # LLM 服务 (DeepSeek)
│   ├── main.py          # 主服务
│   └── config.py        # 配置管理
└── captured_images/     # 拍照图片保存
```

## 快速开始

### 1. Backend 配置

```bash
cd Backend

# 安装依赖
pip install -r requirements.txt

# 配置 API
cp .env.example .env
# 编辑 .env 填写 DeepSeek API Key

# 启动服务
python main.py
```

### 2. 配置说明

`.env` 文件：

```env
# DeepSeek API
DEEPSEEK_API_KEY=your_api_key_here
DEEPSEEK_API_URL=https://api.deepseek.com/v1

# 服务配置
SERVER_HOST=0.0.0.0
SERVER_PORT=8000
```

### 3. Unity 配置

在 `QuestVisionCapture.cs` 中修改后端地址：

```csharp
public string backendUrl = "http://YOUR_PC_IP:8000/api/v1/analyze";
```

## API 接口

### POST /api/v1/analyze
识别物体并生成问题

**Request:**
```json
{
  "image": "base64_encoded_image"
}
```

**Response:**
```json
{
  "object": "computer monitor",
  "description": "A desktop monitor display",
  "questions": [
    "What is this?",
    "How to use it?",
    "What are common problems?"
  ]
}
```

### POST /api/v1/answer
回答问题

**Request:**
```json
{
  "object": "computer monitor",
  "question": "How to use it?"
}
```

**Response:**
```json
{
  "answer": "To use a monitor..."
}
```

## 技术栈

- **后端**: Python 3.10+, FastAPI
- **视觉**: DeepSeek Vision API
- **LLM**: DeepSeek API
- **前端**: Unity 2022.3 LTS, Meta Quest SDK
- **设备**: Meta Quest 3

## 网络要求

- Quest 3 和 PC 在同一局域网
- PC 防火墙允许 8000 端口
- Quest 3 可以访问 PC 的 IP 地址

## 开发状态

- ✅ Backend API 完成
- ✅ 视觉识别集成
- ✅ LLM 问答集成
- ✅ Unity Quest 3 集成
- ✅ 完整流程测试通过

## License

MIT
