using UnityEngine;
using UnityEngine.UI;

namespace SpatialAI.UI
{
    /// <summary>
    /// 加载指示器
    /// 显示加载动画和提示文字
    /// </summary>
    public class LoadingIndicator : MonoBehaviour
    {
        [Header("UI 组件")]
        public GameObject loadingPanel;
        public Text loadingText;
        public Image spinnerImage;

        [Header("旋转设置")]
        public float rotationSpeed = 180f;

        private bool isShowing = false;

        void Start()
        {
            Hide();
        }

        void Update()
        {
            if (isShowing && spinnerImage != null)
            {
                // 旋转加载图标
                spinnerImage.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// 显示加载提示
        /// </summary>
        public void Show(string message = "加载中...")
        {
            isShowing = true;

            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
            }

            if (loadingText != null)
            {
                loadingText.text = message;
            }
        }

        /// <summary>
        /// 隐藏加载提示
        /// </summary>
        public void Hide()
        {
            isShowing = false;

            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
