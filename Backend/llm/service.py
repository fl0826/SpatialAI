"""
LLM 服务 - 简化版
生成问题 + 回答问题
"""

import json
from typing import Dict, Any, List
from openai import OpenAI
from config import LLMConfig


class LLMService:
    def __init__(self):
        print(f"[LLM] Init: {LLMConfig.API_URL} / {LLMConfig.MODEL_NAME}")
        self.client = OpenAI(
            api_key=LLMConfig.API_KEY,
            base_url=LLMConfig.API_URL,
            timeout=60.0
        )
        self.model = LLMConfig.MODEL_NAME

    def generate_questions(self, object_info: Dict[str, Any]) -> List[str]:
        """生成 3 个推荐问题"""
        prompt = f"""Generate 3 most relevant questions users might ask about this object.

Object: {object_info.get('object', 'Unknown')}

Return JSON format:
{{
  "questions": ["Question 1", "Question 2", "Question 3"]
}}

Requirements: 
- Practical usage, common problems, features
- Each question under 15 words
- Most useful first"""

        try:
            response = self.client.chat.completions.create(
                model=self.model,
                messages=[{"role": "user", "content": prompt}],
                response_format={"type": "json_object"},
                temperature=0.7,
                max_tokens=500,
            )

            result = response.choices[0].message.content.strip()
            data = json.loads(result)
            questions = data.get("questions", [])

            if questions:
                print(f"[LLM] Generated {len(questions)} questions")
                return questions[:3]

            return self._default_questions()

        except Exception as e:
            print(f"[LLM] ❌ Generate questions error: {e}")
            return self._default_questions()

    def answer_question(self, object_info: Dict[str, Any], question: str) -> str:
        """回答用户问题"""
        prompt = f"""User is observing: {object_info.get('object', 'Unknown')}

Question: {question}

Requirements: Answer concisely and practically, within 100 words."""

        try:
            response = self.client.chat.completions.create(
                model=self.model,
                messages=[{"role": "user", "content": prompt}],
                temperature=0.7,
                max_tokens=500,
            )

            answer = response.choices[0].message.content.strip()
            
            if answer:
                print(f"[LLM] Answer length: {len(answer)} chars")
                return answer

            return "Sorry, unable to answer this question."

        except Exception as e:
            print(f"[LLM] ❌ Answer error: {e}")
            return "Sorry, unable to answer this question."

    def _default_questions(self) -> List[str]:
        """默认问题"""
        return [
            "What is this?",
            "How to use it?",
            "What are common problems?",
        ]
