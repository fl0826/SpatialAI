"""
直接测试 API 配置
不通过 Backend，直接调用你的 Qwen-VL 和 LLM API
"""
import base64
import requests
from openai import OpenAI
from dotenv import load_dotenv
import os

load_dotenv()

# 读取配置
VISION_API_KEY = os.getenv("VISION_API_KEY")
VISION_API_URL = os.getenv("VISION_API_URL")
VISION_MODEL = os.getenv("VISION_MODEL_NAME")

LLM_API_KEY = os.getenv("LLM_API_KEY")
LLM_API_URL = os.getenv("LLM_API_URL")
LLM_MODEL = os.getenv("LLM_MODEL_NAME")

print("=" * 60)
print("测试 API 配置")
print("=" * 60)

# 禁用代理
proxies = {"http": None, "https": None}

# 测试 1: 测试 Vision API
print("\n[1] 测试 Vision API")
print(f"   URL: {VISION_API_URL}")
print(f"   Model: {VISION_MODEL}")

try:
    client = OpenAI(
        api_key=VISION_API_KEY,
        base_url=VISION_API_URL,
        timeout=60.0,
        http_client=None  # 使用默认客户端，避免代理问题
    )
    
    # 下载测试图片
    img_url = "https://img-s.msn.cn/tenant/amp/entityid/AA26KvOL.img?w=640&h=820&m=6"
    print(f"   下载测试图片...")
    img_bytes = requests.get(img_url, timeout=10, proxies=proxies).content
    img_base64 = base64.b64encode(img_bytes).decode()
    print(f"   图片大小: {len(img_bytes)} 字节")
    
    print(f"   调用 Vision API...")
    response = client.chat.completions.create(
        model=VISION_MODEL,
        messages=[{
            "role": "user",
            "content": [
                {
                    "type": "image_url",
                    "image_url": {"url": f"data:image/jpeg;base64,{img_base64}"}
                },
                {"type": "text", "text": "这张图里是什么？"}
            ]
        }],
        temperature=0.0,
        max_tokens=200
    )
    
    result = response.choices[0].message.content
    print(f"✅ Vision API 成功:")
    print(f"   {result}")
    
except Exception as e:
    print(f"❌ Vision API 失败: {e}")
    import traceback
    traceback.print_exc()

# 测试 2: 测试 LLM API
print("\n[2] 测试 LLM API")
print(f"   URL: {LLM_API_URL}")
print(f"   Model: {LLM_MODEL}")

try:
    client = OpenAI(
        api_key=LLM_API_KEY,
        base_url=LLM_API_URL,
        timeout=60.0
    )
    
    print(f"   调用 LLM API...")
    response = client.chat.completions.create(
        model=LLM_MODEL,
        messages=[{"role": "user", "content": "你好，请介绍一下你自己"}],
        temperature=0.7,
        max_tokens=100
    )
    
    result = response.choices[0].message.content
    print(f"✅ LLM API 成功:")
    print(f"   {result}")
    
except Exception as e:
    print(f"❌ LLM API 失败: {e}")
    import traceback
    traceback.print_exc()

print("\n" + "=" * 60)
print("测试完成")
print("=" * 60)
