using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpatialAI.UI
{
    /// <summary>
    /// 问题面板
    /// 显示推荐问题和答案
    /// </summary>
    public class QuestionPanel : MonoBehaviour
    {
        [Header("UI 组件")]
        public Text titleText;
        public Transform questionContainer;
        public GameObject questionButtonPrefab;
        public Text answerText;
        public GameObject answerPanel;

        [Header("设置")]
        public float buttonSpacing = 10f;

        private Action<string> onQuestionSelected;

        void Start()
        {
            // 初始隐藏
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示推荐问题
        /// </summary>
        public void ShowQuestions(string objectName, string[] questions, Action<string> onSelected)
        {
            gameObject.SetActive(true);
            onQuestionSelected = onSelected;

            // 设置标题
            if (titleText != null)
            {
                titleText.text = $"关于 {objectName}";
            }

            // 清空旧按钮
            ClearQuestions();

            // 创建问题按钮
            foreach (string question in questions)
            {
                CreateQuestionButton(question);
            }

            // 隐藏答案面板
            if (answerPanel != null)
            {
                answerPanel.SetActive(false);
            }
        }

        void ClearQuestions()
        {
            if (questionContainer != null)
            {
                foreach (Transform child in questionContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        void CreateQuestionButton(string question)
        {
            GameObject buttonObj;

            if (questionButtonPrefab != null)
            {
                buttonObj = Instantiate(questionButtonPrefab, questionContainer);
            }
            else
            {
                // 如果没有预制体，创建简单按钮
                buttonObj = new GameObject("QuestionButton");
                buttonObj.transform.SetParent(questionContainer);
                
                var button = buttonObj.AddComponent<Button>();
                var text = buttonObj.AddComponent<Text>();
                text.text = question;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 14;
                text.color = Color.white;
            }

            // 获取或添加 Text 组件
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = question;
            }

            // 设置按钮点击事件
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnQuestionButtonClicked(question));
            }

            buttonObj.transform.localScale = Vector3.one;
        }

        void OnQuestionButtonClicked(string question)
        {
            Debug.Log($"点击问题: {question}");
            onQuestionSelected?.Invoke(question);
        }

        /// <summary>
        /// 显示答案
        /// </summary>
        public void ShowAnswer(string answer)
        {
            if (answerPanel != null)
            {
                answerPanel.SetActive(true);
            }

            if (answerText != null)
            {
                answerText.text = answer;
            }
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
