"""
测试 Backend API
快速验证 Backend 是否正常运行
"""

import base64
import requests
import json


def test_backend(backend_url="http://localhost:8000"):
    print("=" * 60)
    print(f"测试 Backend: {backend_url}")
    print("=" * 60)

    # 禁用代理（避免本地测试时使用系统代理）
    proxies = {
        "http": None,
        "https": None,
    }

    # 测试 1: 健康检查
    print("\n[1] 健康检查")
    try:
        resp = requests.get(f"{backend_url}/health", proxies=proxies)
        print(f"✅ 状态: {resp.status_code}")
        print(f"   响应: {resp.json()}")
    except Exception as e:
        print(f"❌ 失败: {e}")
        return

    # 测试 2: 分析接口（使用测试图片）
    print("\n[2] 测试分析接口")
    try:
        # 使用网络图片测试
        img_url = (
            "https://img-s.msn.cn/tenant/amp/entityid/AA26KvOL.img?w=640&h=820&m=6"
        )
        img_bytes = requests.get(img_url, timeout=10, proxies=proxies).content
        img_base64 = base64.b64encode(img_bytes).decode()

        payload = {"image": img_base64, "timestamp": 1234567890}

        print("   发送请求...")
        resp = requests.post(
            f"{backend_url}/api/v1/analyze", json=payload, timeout=60, proxies=proxies
        )

        if resp.status_code == 200:
            result = resp.json()
            print(f"✅ 识别成功:")
            print(f"   物体: {result['object']['object']}")
            print(f"   类别: {result['object']['category']}")
            print(f"   描述: {result['object']['description']}")
            print(f"   问题数量: {len(result['questions'])}")
            print(f"   问题列表:")
            for i, q in enumerate(result["questions"], 1):
                print(f"     {i}. {q}")

            # 保存 object_id 用于测试回答接口
            object_id = result["object_id"]

            # 测试 3: 回答接口
            print("\n[3] 测试回答接口")
            answer_payload = {
                "object_id": object_id,
                "question": (
                    result["questions"][0] if result["questions"] else "这是什么？"
                ),
            }

            print(f"   提问: {answer_payload['question']}")
            resp2 = requests.post(
                f"{backend_url}/api/v1/answer",
                json=answer_payload,
                timeout=60,
                proxies=proxies,
            )

            if resp2.status_code == 200:
                answer = resp2.json()
                print(f"✅ 回答: {answer['answer']}")
            else:
                print(f"❌ 回答失败: {resp2.status_code}")
                print(f"   {resp2.text}")
        else:
            print(f"❌ 分析失败: {resp.status_code}")
            print(f"   {resp.text}")

    except Exception as e:
        print(f"❌ 失败: {e}")

    print("\n" + "=" * 60)
    print("测试完成")
    print("=" * 60)


if __name__ == "__main__":
    import sys

    # 从命令行参数获取 URL，默认 localhost
    url = sys.argv[1] if len(sys.argv) > 1 else "http://localhost:8000"
    test_backend(url)
