using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using SpatialAI.Core;

namespace SpatialAI.Network
{
    /// <summary>
    /// Backend API 客户端
    /// </summary>
    public class APIClient : MonoBehaviour
    {
        public static APIClient Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 调用分析接口
        /// </summary>
        public void Analyze(string imageBase64, Action<AnalyzeResponse> onSuccess, Action<string> onError)
        {
            var request = new AnalyzeRequest
            {
                image = imageBase64,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            StartCoroutine(PostRequest(
                Config.Instance.GetAnalyzeURL(),
                request,
                onSuccess,
                onError
            ));
        }

        /// <summary>
        /// 调用回答接口
        /// </summary>
        public void Answer(string objectId, string question, Action<AnswerResponse> onSuccess, Action<string> onError)
        {
            var request = new AnswerRequest
            {
                object_id = objectId,
                question = question
            };

            StartCoroutine(PostRequest(
                Config.Instance.GetAnswerURL(),
                request,
                onSuccess,
                onError
            ));
        }

        /// <summary>
        /// 通用 POST 请求
        /// </summary>
        IEnumerator PostRequest<TRequest, TResponse>(
            string url,
            TRequest requestData,
            Action<TResponse> onSuccess,
            Action<string> onError)
        {
            string json = JsonUtility.ToJson(requestData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                www.timeout = Config.Instance.requestTimeout;

                Config.Instance.Log($"发送请求: {url}");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        string responseText = www.downloadHandler.text;
                        Config.Instance.Log($"收到响应: {responseText}");

                        TResponse response = JsonUtility.FromJson<TResponse>(responseText);
                        onSuccess?.Invoke(response);
                    }
                    catch (Exception e)
                    {
                        Config.Instance.LogError($"解析响应失败: {e.Message}");
                        onError?.Invoke($"解析失败: {e.Message}");
                    }
                }
                else
                {
                    string error = $"请求失败: {www.error}";
                    Config.Instance.LogError(error);
                    onError?.Invoke(error);
                }
            }
        }
    }
}
