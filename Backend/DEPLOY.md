# SpatialAI Backend 部署指南

## 快速开始

### 1. 安装依赖

```bash
cd Backend
pip install -r requirements.txt
```

### 2. 配置 API

复制配置模板：

```bash
cp .env.example .env
```

编辑 `.env` 文件，填写你的 API 信息：

```bash
# Vision Model (Qwen-VL)
VISION_API_KEY=your_actual_key
VISION_API_URL=https://your-api-endpoint.com/v1
VISION_MODEL_NAME=qwen-vl-plus

# Language Model (LLM)
LLM_API_KEY=your_actual_key
LLM_API_URL=https://your-api-endpoint.com/v1
LLM_MODEL_NAME=gpt-4

# Server Config
HOST=0.0.0.0
PORT=8000
```

### 3. 启动服务

```bash
python main.py
```

服务将在 `http://0.0.0.0:8000` 启动

### 4. 测试 API

访问 `http://localhost:8000/docs` 查看自动生成的 API 文档

---

## API 使用示例

### 分析物体

```bash
curl -X POST http://localhost:8000/api/v1/analyze \
  -H "Content-Type: application/json" \
  -d '{
    "image": "base64_encoded_image_string",
    "timestamp": 1234567890
  }'
```

返回：

```json
{
  "object_id": "abc-123-def",
  "object": {
    "object": "冰箱",
    "category": "家电/制冷",
    "description": "双开门家用冰箱",
    "context": "厨房环境，可能用于食物储存"
  },
  "questions": [
    "这个冰箱怎么调温度？",
    "冰箱的保鲜室在哪里？",
    "如何清洁冰箱？"
  ]
}
```

### 回答问题

```bash
curl -X POST http://localhost:8000/api/v1/answer \
  -H "Content-Type: application/json" \
  -d '{
    "object_id": "abc-123-def",
    "question": "这个冰箱怎么调温度？"
  }'
```

返回：

```json
{
  "answer": "温度调节旋钮通常在冷藏室内部顶部或侧面..."
}
```

---

## 局域网访问（Quest 3）

1. 查看工作站 IP：

```bash
# Windows
ipconfig

# 找到局域网 IP，例如 192.168.1.100
```

2. Quest 3 访问地址：

```text
http://192.168.1.100:8000
```

3. 确保防火墙允许 8000 端口

---

## 项目结构

```text
Backend/
├── main.py              # FastAPI 主服务
├── config.py            # 配置管理
├── requirements.txt     # Python 依赖
├── .env                 # API 配置（不提交到 git）
├── .env.example         # 配置模板
├── vision/
│   ├── __init__.py
│   └── service.py       # Qwen-VL 服务
└── llm/
    ├── __init__.py
    └── service.py       # LLM 服务
```

---

## 注意事项

1. **不要提交 .env 文件到 git**
2. **确保 API Key 安全**
3. **建议使用虚拟环境**：`python -m venv venv`
4. **生产环境建议使用 gunicorn**
