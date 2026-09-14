using UnityEngine;
using SpatialAI.Camera;
using SpatialAI.Network;
using SpatialAI.UI;

namespace SpatialAI.Interaction
{
    /// <summary>
    /// 物体选择器
    /// 协调图像捕获、AI分析、UI显示
    /// </summary>
    public class ObjectSelector : MonoBehaviour
    {
        [Header("组件引用")]
        public ImageCapture imageCapture;
        public QuestionPanel questionPanel;
        public LoadingIndicator loadingIndicator;

        private string currentObjectId;
        private ObjectInfo currentObjectInfo;

        void Start()
        {
            if (imageCapture == null)
            {
                imageCapture = FindObjectOfType<ImageCapture>();
            }

            if (questionPanel == null)
            {
                questionPanel = FindObjectOfType<QuestionPanel>();
            }

            if (loadingIndicator == null)
            {
                loadingIndicator = FindObjectOfType<LoadingIndicator>();
            }
        }

        /// <summary>
        /// 当用户选择了一个物体（点击手柄）
        /// </summary>
        public void OnObjectSelected(Vector3 hitPoint)
        {
            Debug.Log("开始分析物体...");

            // 显示加载动画
            if (loadingIndicator != null)
            {
                loadingIndicator.Show("正在识别物体...");
            }

            // 捕获图像
            string imageBase64 = imageCapture.CaptureImageAsBase64();

            if (string.IsNullOrEmpty(imageBase64))
            {
                OnAnalyzeError("图像捕获失败");
                return;
            }

            // 调用 API
            APIClient.Instance.Analyze(
                imageBase64,
                OnAnalyzeSuccess,
                OnAnalyzeError
            );
        }

        void OnAnalyzeSuccess(AnalyzeResponse response)
        {
            Debug.Log($"识别成功: {response.@object.@object}");

            // 保存结果
            currentObjectId = response.object_id;
            currentObjectInfo = response.@object;

            // 隐藏加载动画
            if (loadingIndicator != null)
            {
                loadingIndicator.Hide();
            }

            // 显示问题面板
            if (questionPanel != null)
            {
                questionPanel.ShowQuestions(
                    response.@object.@object,
                    response.questions,
                    OnQuestionSelected
                );
            }
        }

        void OnAnalyzeError(string error)
        {
            Debug.LogError($"分析失败: {error}");

            if (loadingIndicator != null)
            {
                loadingIndicator.Hide();
            }

            // TODO: 显示错误提示
        }

        /// <summary>
        /// 当用户选择了一个问题
        /// </summary>
        void OnQuestionSelected(string question)
        {
            Debug.Log($"用户选择问题: {question}");

            // 显示加载动画
            if (loadingIndicator != null)
            {
                loadingIndicator.Show("正在思考...");
            }

            // 调用回答 API
            APIClient.Instance.Answer(
                currentObjectId,
                question,
                OnAnswerSuccess,
                OnAnswerError
            );
        }

        void OnAnswerSuccess(AnswerResponse response)
        {
            Debug.Log($"回答: {response.answer}");

            // 隐藏加载动画
            if (loadingIndicator != null)
            {
                loadingIndicator.Hide();
            }

            // 显示答案
            if (questionPanel != null)
            {
                questionPanel.ShowAnswer(response.answer);
            }
        }

        void OnAnswerError(string error)
        {
            Debug.LogError($"回答失败: {error}");

            if (loadingIndicator != null)
            {
                loadingIndicator.Hide();
            }

            // TODO: 显示错误提示
        }
    }
}
