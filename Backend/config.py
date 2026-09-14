"""
配置管理
从环境变量或 .env 文件加载 API 配置
"""
import os
from pathlib import Path
from dotenv import load_dotenv

BASE_DIR = Path(__file__).resolve().parent
load_dotenv(BASE_DIR / ".env")


class VisionConfig:
    """Qwen-VL 视觉模型配置"""
    API_KEY = os.getenv("VISION_API_KEY", "")
    API_URL = os.getenv("VISION_API_URL", "")
    MODEL_NAME = os.getenv("VISION_MODEL_NAME", "qwen-vl-plus")


class LLMConfig:
    """语言模型配置"""
    API_KEY = os.getenv("LLM_API_KEY", "")
    API_URL = os.getenv("LLM_API_URL", "")
    MODEL_NAME = os.getenv("LLM_MODEL_NAME", "gpt-4")


class ServerConfig:
    """服务器配置"""
    HOST = os.getenv("HOST", "0.0.0.0")
    PORT = int(os.getenv("PORT", 8000))


def validate_config():
    """验证配置是否完整"""
    missing = []
    
    if not VisionConfig.API_KEY:
        missing.append("VISION_API_KEY")
    if not VisionConfig.API_URL:
        missing.append("VISION_API_URL")
    if not LLMConfig.API_KEY:
        missing.append("LLM_API_KEY")
    if not LLMConfig.API_URL:
        missing.append("LLM_API_URL")
    
    if missing:
        raise ValueError(f"缺少配置项: {', '.join(missing)}")
    
    return True
