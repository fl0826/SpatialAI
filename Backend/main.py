"""
FastAPI 主服务 - 简化版
拍照 → 识别物体 → 生成问题 → 回答问题
"""

import base64
from datetime import datetime
from pathlib import Path
from typing import List
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel

from config import ServerConfig, validate_config
from vision.service import VisionService
from llm.service import LLMService

# 图片保存目录
IMAGES_DIR = Path(__file__).parent.parent / "captured_images"
IMAGES_DIR.mkdir(exist_ok=True)

app = FastAPI(title="SpatialAI Backend", version="3.0.0")

# CORS（允许 Quest 3 访问）
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# 服务实例
vision_service = VisionService()
llm_service = LLMService()


# ==================== 请求/响应模型 ====================

class AnalyzeRequest(BaseModel):
    image: str  # base64


class AnalyzeResponse(BaseModel):
    object: str
    description: str
    questions: List[str]


class AnswerRequest(BaseModel):
    object: str
    question: str


class AnswerResponse(BaseModel):
    answer: str


# ==================== API 路由 ====================

@app.get("/")
async def root():
    return {"service": "SpatialAI Backend", "version": "3.0.0", "status": "running"}


@app.post("/api/v1/analyze", response_model=AnalyzeResponse)
async def analyze_object(request: AnalyzeRequest):
    """识别物体并生成问题"""
    try:
        # 保存图片
        timestamp = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
        image_path = IMAGES_DIR / f"capture_{timestamp}.jpg"
        
        try:
            image_data = base64.b64decode(request.image)
            with open(image_path, "wb") as f:
                f.write(image_data)
            print(f"💾 图片已保存: {image_path}")
        except Exception as e:
            print(f"⚠️ 保存图片失败: {e}")

        # 识别物体
        object_info = vision_service.analyze_image(request.image)
        
        # 生成问题
        questions = llm_service.generate_questions(object_info)

        return AnalyzeResponse(
            object=object_info.get("object", "Unknown"),
            description=object_info.get("description", ""),
            questions=questions
        )

    except Exception as e:
        import traceback
        traceback.print_exc()
        raise HTTPException(status_code=500, detail=f"分析失败: {str(e)}")


@app.post("/api/v1/answer", response_model=AnswerResponse)
async def answer_question(request: AnswerRequest):
    """回答问题"""
    try:
        object_info = {"object": request.object}
        answer = llm_service.answer_question(object_info, request.question)
        
        return AnswerResponse(answer=answer)

    except Exception as e:
        raise HTTPException(status_code=500, detail=f"回答失败: {str(e)}")


@app.get("/health")
async def health_check():
    return {"status": "healthy"}


# ==================== 启动 ====================

if __name__ == "__main__":
    import uvicorn

    try:
        validate_config()
        print("✅ 配置验证通过")
    except ValueError as e:
        print(f"❌ 配置错误: {e}")
        exit(1)

    print(f"📁 图片保存目录: {IMAGES_DIR.absolute()}")
    print(f"🚀 启动服务: http://{ServerConfig.HOST}:{ServerConfig.PORT}")
    uvicorn.run(app, host=ServerConfig.HOST, port=ServerConfig.PORT)
