using UnityEngine;

namespace SpatialAI.Camera
{
    /// <summary>
    /// 图像捕获
    /// 从 Quest 3 摄像头获取图像并转换为 base64
    /// </summary>
    public class ImageCapture : MonoBehaviour
    {
        [Header("捕获设置")]
        [Tooltip("图像宽度")]
        public int captureWidth = 640;

        [Tooltip("图像高度")]
        public int captureHeight = 480;

        private Texture2D captureTexture;

        void Start()
        {
            captureTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        }

        /// <summary>
        /// 捕获当前视图并转换为 base64
        /// MVP 版本：捕获屏幕中心区域
        /// TODO: 后续集成 Quest 3 Camera API 获取真实摄像头画面
        /// </summary>
        public string CaptureImageAsBase64()
        {
            try
            {
                // MVP: 使用 ReadPixels 从屏幕捕获
                // 注意: Passthrough 环境需要特殊处理
                int x = (Screen.width - captureWidth) / 2;
                int y = (Screen.height - captureHeight) / 2;

                captureTexture.ReadPixels(new Rect(x, y, captureWidth, captureHeight), 0, 0);
                captureTexture.Apply();

                // 转换为 JPEG 并编码为 base64
                byte[] imageBytes = captureTexture.EncodeToJPG(75);
                string base64 = System.Convert.ToBase64String(imageBytes);

                Debug.Log($"捕获图像: {imageBytes.Length} 字节");

                return base64;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"图像捕获失败: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 使用 Quest 3 Camera API 捕获
        /// 需要集成 OVRCameraRig 和 Camera Frame Provider
        /// </summary>
        public string CaptureFromQuestCamera()
        {
            // TODO: 集成 Meta Quest Camera API
            // 参考: Unity-PassthroughCameraApiSamples/CameraViewer
            
            Debug.LogWarning("Quest Camera API 尚未实现，使用屏幕捕获替代");
            return CaptureImageAsBase64();
        }

        void OnDestroy()
        {
            if (captureTexture != null)
            {
                Destroy(captureTexture);
            }
        }
    }
}
