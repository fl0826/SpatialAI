using UnityEngine;

namespace SpatialAI.Core
{
    /// <summary>
    /// 全局配置管理
    /// </summary>
    public class Config : MonoBehaviour
    {
        public static Config Instance { get; private set; }

        [Header("Backend API 配置")]
        [Tooltip("Backend 服务器地址，例如: http://192.168.1.100:8000")]
        public string backendURL = "http://192.168.1.100:8000";

        [Header("超时设置")]
        public int requestTimeout = 10;

        [Header("调试设置")]
        public bool enableDebugLog = true;

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

        public string GetAnalyzeURL() => $"{backendURL}/api/v1/analyze";
        public string GetAnswerURL() => $"{backendURL}/api/v1/answer";

        public void Log(string message)
        {
            if (enableDebugLog)
            {
                Debug.Log($"[SpatialAI] {message}");
            }
        }

        public void LogError(string message)
        {
            Debug.LogError($"[SpatialAI] {message}");
        }
    }
}
