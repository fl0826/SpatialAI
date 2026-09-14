"""
视觉识别服务 - 简化版
识别图像中的物体
"""
import json
from typing import Dict, Any
from openai import OpenAI
from config import VisionConfig


class VisionService:
    def __init__(self):
        print(f"[Vision] 初始化: {VisionConfig.MODEL_NAME}")
        self.client = OpenAI(
            api_key=VisionConfig.API_KEY,
            base_url=VisionConfig.API_URL,
            timeout=60.0
        )
        self.model = VisionConfig.MODEL_NAME
    
    def analyze_image(self, image_base64: str) -> Dict[str, Any]:
        """分析图像并返回物体信息"""
        
        prompt = """Identify the main object in the image. Return JSON:
{
  "object": "object name",
  "category": "category", 
  "description": "brief description"
}

Only return valid JSON."""
        
        try:
            print(f"[Vision] Calling: {self.model}")
            response = self.client.chat.completions.create(
                model=self.model,
                messages=[{
                    "role": "user",
                    "content": [
                        {"type": "text", "text": prompt},
                        {
                            "type": "image_url",
                            "image_url": {"url": f"data:image/jpeg;base64,{image_base64}"}
                        }
                    ]
                }],
                response_format={'type': 'json_object'},
                temperature=0.3,
                max_tokens=500
            )
            
            result_text = response.choices[0].message.content.strip()
            
            if not result_text:
                print(f"[Vision] ⚠️ Empty response")
                return self._default()
            
            # 解析 JSON
            result = self._parse_json(result_text)
            
            if result:
                print(f"[Vision] ✅ Success: {result['object']}")
                return result
            
            print(f"[Vision] ⚠️ JSON parse failed")
            return self._default()
                
        except Exception as e:
            print(f"[Vision] ❌ Error: {e}")
            return self._default()
    
    def _parse_json(self, text: str) -> Dict[str, Any]:
        """解析 JSON"""
        try:
            return json.loads(text)
        except:
            pass
        
        # 提取代码块
        try:
            if "```json" in text:
                json_text = text.split("```json")[1].split("```")[0].strip()
                return json.loads(json_text)
            elif "```" in text:
                json_text = text.split("```")[1].split("```")[0].strip()
                return json.loads(json_text)
        except:
            pass
        
        return None
    
    def _default(self) -> Dict[str, Any]:
        """默认返回"""
        return {
            "object": "Unknown Object",
            "category": "Uncategorized",
            "description": "Recognition failed"
        }
