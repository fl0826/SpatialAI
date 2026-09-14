# Backend 服务

AI 后端服务，提供视觉识别和语言交互 API

## 目录结构

```text
Backend/
├── vision/         # Qwen-VL 视觉识别服务
├── llm/            # LLM 语言模型服务
├── api/            # FastAPI 统一接口
└── requirements.txt
```

## API 接口

### 1. 分析物体并生成问题

```bash
POST /api/v1/analyze
Content-Type: application/json

{
  "image": "base64_encoded_string",
  "timestamp": 1234567890
}
```

返回：

```json
{
  "object_id": "abc123",
  "object": {
    "name": "冰箱",
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

### 2. 回答问题

```bash
POST /api/v1/answer
Content-Type: application/json

{
  "object_id": "abc123",
  "question": "这个冰箱怎么调温度？"
}
```

返回：

```json
{
  "answer": "温度调节旋钮通常在冷藏室内部顶部或侧面..."
}
```

## 部署

待补充
