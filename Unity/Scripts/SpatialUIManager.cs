using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections.Generic;

public class SpatialUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loadingPanel;
    public GameObject operationGuidePanel;
    public GameObject questionsPanel;
    public GameObject answerPanel;
    
    [Header("Loading UI")]
    public TextMeshProUGUI loadingText;
    
    [Header("Operation Guide UI")]
    public TextMeshProUGUI guideTitle;
    public TextMeshProUGUI currentStepText;
    public TextMeshProUGUI stepDescriptionText;
    public Button nextStepButton;
    public Button prevStepButton;
    
    [Header("Questions UI")]
    public TextMeshProUGUI objectNameText;
    public TextMeshProUGUI objectDescText;
    public Transform questionsContainer;
    
    [Header("Answer UI")]
    public TextMeshProUGUI answerText;
    public Button backButton;
    
    private int currentStep = 0;
    private List<StepData> steps = new List<StepData>();
    private LineRenderer laserPointer;
    private Transform rightHand;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    
    void Start()
    {
        if (nextStepButton) nextStepButton.onClick.AddListener(NextStep);
        if (prevStepButton) prevStepButton.onClick.AddListener(PrevStep);
        if (backButton) backButton.onClick.AddListener(() => { if (answerPanel) answerPanel.SetActive(false); });
        
        InitLaserPointer();
        
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("EventSystem not found!");
        }
        
        if (questionsPanel != null)
        {
            Canvas canvas = questionsPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            }
        }
    }
    
    void Update()
    {
        UpdateLaserPointer();
        CheckTriggerInput();
    }
    
    void CheckTriggerInput()
    {
        bool triggerPressed = false;
        
        try
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                triggerPressed = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"OVR Input check failed: {e.Message}");
        }
        
        if (triggerPressed && rightHand != null && graphicRaycaster != null && eventSystem != null)
        {
            PointerEventData pointerData = new PointerEventData(eventSystem);
            Vector3 laserEnd = rightHand.position + rightHand.forward * 3f;
            
            if (Camera.main != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(laserEnd);
                pointerData.position = screenPos;
                
                List<RaycastResult> results = new List<RaycastResult>();
                graphicRaycaster.Raycast(pointerData, results);
                
                if (results.Count > 0)
                {
                    GameObject hitObject = results[0].gameObject;
                    Button button = hitObject.GetComponent<Button>();
                    if (button == null)
                    {
                        button = hitObject.GetComponentInParent<Button>();
                    }
                    
                    if (button != null && button.interactable)
                    {
                        button.onClick.Invoke();
                    }
                }
            }
        }
    }
    
    void InitLaserPointer()
    {
        GameObject rightHandObj = GameObject.Find("RightHandAnchor");
        if (rightHandObj == null)
        {
            Debug.LogWarning("RightHandAnchor not found");
            return;
        }
        rightHand = rightHandObj.transform;
        
        GameObject laserObj = new GameObject("LaserPointer");
        laserObj.transform.SetParent(rightHand);
        laserObj.transform.localPosition = Vector3.zero;
        laserObj.transform.localRotation = Quaternion.identity;
        
        laserPointer = laserObj.AddComponent<LineRenderer>();
        laserPointer.startWidth = 0.005f;
        laserPointer.endWidth = 0.005f;
        laserPointer.positionCount = 2;
        laserPointer.useWorldSpace = true;
        
        laserPointer.material = new Material(Shader.Find("Sprites/Default"));
        laserPointer.startColor = new Color(0, 0.5f, 1f, 0.8f);
        laserPointer.endColor = new Color(0, 0.5f, 1f, 0.2f);
    }
    
    void UpdateLaserPointer()
    {
        if (laserPointer == null || rightHand == null) return;
        
        laserPointer.SetPosition(0, rightHand.position);
        laserPointer.SetPosition(1, rightHand.position + rightHand.forward * 3f);
    }
    
    public void ShowLoading(string message = "AI Analyzing...")
    {
        // Hide other panels
        if (operationGuidePanel) operationGuidePanel.SetActive(false);
        if (questionsPanel) questionsPanel.SetActive(false);
        if (answerPanel) answerPanel.SetActive(false);
        
        // Show loading
        if (loadingPanel) loadingPanel.SetActive(true);
        if (loadingText) loadingText.text = message;
    }
    
    public void ShowOperationGuide(string title, List<StepData> stepsList)
    {
        // Hide other panels
        if (loadingPanel) loadingPanel.SetActive(false);
        if (questionsPanel) questionsPanel.SetActive(false);
        if (answerPanel) answerPanel.SetActive(false);
        
        // Show guide
        if (operationGuidePanel) operationGuidePanel.SetActive(true);
        
        steps = stepsList;
        currentStep = 0;
        
        if (guideTitle) guideTitle.text = title;
        UpdateStepDisplay();
    }
    
    private void UpdateStepDisplay()
    {
        if (currentStep >= 0 && currentStep < steps.Count)
        {
            StepData step = steps[currentStep];
            if (currentStepText) 
                currentStepText.text = $"{step.icon} Step {step.step}/{steps.Count}:\n{step.action}";
            if (stepDescriptionText) 
                stepDescriptionText.text = step.description;
            
            if (prevStepButton) prevStepButton.interactable = currentStep > 0;
            if (nextStepButton) 
            {
                nextStepButton.interactable = currentStep < steps.Count - 1;
                var buttonText = nextStepButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText)
                    buttonText.text = (currentStep == steps.Count - 1) ? "Done" : "Next";
            }
        }
    }
    
    private void NextStep()
    {
        if (currentStep < steps.Count - 1)
        {
            currentStep++;
            UpdateStepDisplay();
        }
        else
        {
            if (operationGuidePanel) operationGuidePanel.SetActive(false);
        }
    }
    
    private void PrevStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            UpdateStepDisplay();
        }
    }
    
    public void ShowQuestions(string objectName, string description, List<string> questions, System.Action<string> onQuestionSelected)
    {
        // Hide other panels
        if (loadingPanel) loadingPanel.SetActive(false);
        if (operationGuidePanel) operationGuidePanel.SetActive(false);
        if (answerPanel) answerPanel.SetActive(false);
        
        // Show questions
        if (questionsPanel) questionsPanel.SetActive(true);
        
        if (objectNameText) objectNameText.text = objectName;
        if (objectDescText) objectDescText.text = description;
        
        // Clear existing buttons
        foreach (Transform child in questionsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create buttons
        foreach (string question in questions)
        {
            GameObject btn = CreateQuestionButton(question);
            btn.transform.SetParent(questionsContainer, false);
            
            var button = btn.GetComponent<Button>();
            string q = question;
            button.onClick.AddListener(() => onQuestionSelected(q));
        }
    }
    
    private GameObject CreateQuestionButton(string text)
    {
        GameObject buttonObj = new GameObject("QuestionButton");
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(800, 80);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        image.raycastTarget = true;
        
        Button button = buttonObj.AddComponent<Button>();
        button.interactable = true;
        
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        colors.highlightedColor = new Color(0.3f, 0.5f, 1f, 0.9f);
        colors.pressedColor = new Color(0.1f, 0.3f, 0.8f, 1f);
        button.colors = colors;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.raycastTarget = false;
        
        return buttonObj;
    }
    
    public void ShowAnswer(string answer)
    {
        // Find objects if not assigned
        if (answerPanel == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform answerTransform = canvas.transform.Find("AnswerPanel");
                if (answerTransform != null)
                {
                    answerPanel = answerTransform.gameObject;
                }
            }
        }
        
        if (answerText == null && answerPanel != null)
        {
            answerText = answerPanel.GetComponentInChildren<TextMeshProUGUI>(true);
        }
        
        // Hide other panels
        if (loadingPanel) loadingPanel.SetActive(false);
        if (operationGuidePanel) operationGuidePanel.SetActive(false);
        if (questionsPanel) questionsPanel.SetActive(false);
        
        // Show answer panel
        if (answerPanel) 
        {
            answerPanel.SetActive(true);
            Debug.Log("AnswerPanel shown");
        }
        else
        {
            Debug.LogError("AnswerPanel not found!");
            return;
        }
        
        if (answerText) 
        {
            answerText.text = answer;
            Debug.Log($"Answer set: {answer.Substring(0, Math.Min(50, answer.Length))}...");
        }
        else
        {
            Debug.LogError("AnswerText not found!");
        }
    }
}

[System.Serializable]
public class StepData
{
    public int step;
    public string action;
    public string description;
    public string icon;
}
