using UnityEngine;
using SpatialAI.Network;
using SpatialAI.Camera;
using SpatialAI.Interaction;

namespace SpatialAI.Core
{
    /// <summary>
    /// 应用主控制器
    /// 初始化所有核心组件
    /// </summary>
    public class AppManager : MonoBehaviour
    {
        [Header("组件引用")]
        public Config config;
        public APIClient apiClient;
        public ImageCapture imageCapture;
        public RayPointer rayPointer;
        public ObjectSelector objectSelector;

        void Awake()
        {
            InitializeComponents();
        }

        void Start()
        {
            Config.Instance.Log("SpatialAI 启动");
            Config.Instance.Log($"Backend URL: {config.backendURL}");
        }

        void InitializeComponents()
        {
            // 确保单例组件存在
            if (config == null)
            {
                config = FindObjectOfType<Config>();
                if (config == null)
                {
                    GameObject configObj = new GameObject("Config");
                    config = configObj.AddComponent<Config>();
                }
            }

            if (apiClient == null)
            {
                apiClient = FindObjectOfType<APIClient>();
                if (apiClient == null)
                {
                    GameObject apiObj = new GameObject("APIClient");
                    apiClient = apiObj.AddComponent<APIClient>();
                }
            }

            // 验证关键组件
            if (imageCapture == null)
            {
                Debug.LogWarning("ImageCapture 未分配");
            }

            if (rayPointer == null)
            {
                Debug.LogWarning("RayPointer 未分配");
            }

            if (objectSelector == null)
            {
                Debug.LogWarning("ObjectSelector 未分配");
            }
        }

        void OnApplicationQuit()
        {
            Config.Instance.Log("SpatialAI 关闭");
        }
    }
}
