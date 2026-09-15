using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

public class QuestVisionCapture : MonoBehaviour
{
    public string backendUrl = "http://172.16.16.75:8000/api/v1/analyze";
    public int jpegQuality = 80;
    public float cooldown = 2f;
    public SpatialUIManager uiManager;
    
    private WebCamTexture webcam;
    private float lastTime = 0f;
    private bool busy = false;
    private string currentObject = "";
    
    void Start()
    {
        Debug.Log("QuestVisionCapture V4.0 - A键拍照 扳机选择");
        StartCoroutine(InitWebcam());
    }
    
    IEnumerator InitWebcam()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            Debug.Log($"Found {devices.Length} cameras");
            
            if (devices.Length > 0)
            {
                string camName = devices[0].name;
                for (int i = 0; i < devices.Length; i++)
                {
                    if (!devices[i].isFrontFacing)
                    {
                        camName = devices[i].name;
                        break;
                    }
                }
                
                webcam = new WebCamTexture(camName, 1920, 1080, 30);
                webcam.Play();
                
                int wait = 0;
                while (webcam.width < 100 && wait < 100)
                {
                    yield return new WaitForSeconds(0.1f);
                    wait++;
                }
                
                Debug.Log($"Camera ready: {webcam.width}x{webcam.height}");
            }
        }
    }
    
    void Update()
    {
        if (webcam == null || !webcam.isPlaying) return;
        
        if (!busy)
        {
            bool capture = false;
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                capture = true;
                Debug.Log("Space pressed");
            }
            
            try
            {
                if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
                {
                    capture = true;
                    Debug.Log("Right A pressed");
                }
            }
            catch { }
            
            if (capture && Time.time - lastTime > cooldown)
            {
                lastTime = Time.time;
                StartCoroutine(CaptureAndAnalyze());
            }
        }
    }
    
    IEnumerator CaptureAndAnalyze()
    {
        busy = true;
        
        if (uiManager) uiManager.ShowLoading("Capturing...");
        
        yield return new WaitForEndOfFrame();
        
        Texture2D photo = new Texture2D(webcam.width, webcam.height, TextureFormat.RGB24, false);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();
        
        byte[] jpg = photo.EncodeToJPG(jpegQuality);
        string b64 = Convert.ToBase64String(jpg);
        Destroy(photo);
        
        if (uiManager) uiManager.ShowLoading("AI Analyzing...");
        yield return StartCoroutine(SendToBackend(b64));
        
        busy = false;
    }
    
    IEnumerator SendToBackend(string img)
    {
        string json = "{\"image\":\"" + img + "\"}";
        byte[] body = Encoding.UTF8.GetBytes(json);
        
        UnityWebRequest req = new UnityWebRequest(backendUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.timeout = 120;
        
        yield return req.SendWebRequest();
        
        if (req.result == UnityWebRequest.Result.Success)
        {
            ProcessResponse(req.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Request failed: {req.error}");
            if (uiManager) uiManager.ShowLoading("Request failed");
        }
        
        req.Dispose();
    }
    
    void ProcessResponse(string jsonResponse)
    {
        try
        {
            JObject json = JObject.Parse(jsonResponse);
            
            currentObject = json["object"].ToString();
            string description = json["description"].ToString();
            JArray questionsArray = json["questions"] as JArray;
            
            List<string> questions = new List<string>();
            foreach (var q in questionsArray)
            {
                questions.Add(q.ToString());
            }
            
            Debug.Log($"Object: {currentObject}, Questions: {questions.Count}");
            
            if (uiManager)
            {
                uiManager.ShowQuestions(currentObject, description, questions, OnQuestionSelected);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Parse error: {e.Message}");
            if (uiManager) uiManager.ShowLoading("Parse error");
        }
    }
    
    void OnQuestionSelected(string question)
    {
        Debug.Log($"Question selected: {question}");
        StartCoroutine(AskQuestion(question));
    }
    
    IEnumerator AskQuestion(string question)
    {
        if (uiManager) uiManager.ShowLoading("Thinking...");
        
        string url = backendUrl.Replace("/analyze", "/answer");
        string json = $"{{\"object\":\"{currentObject}\",\"question\":\"{question}\"}}";
        byte[] body = Encoding.UTF8.GetBytes(json);
        
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.timeout = 120;
        
        yield return req.SendWebRequest();
        
        if (req.result == UnityWebRequest.Result.Success)
        {
            JObject json2 = JObject.Parse(req.downloadHandler.text);
            string answer = json2["answer"].ToString();

            Debug.Log($"Answer received: {answer.Substring(0, Math.Min(50, answer.Length))}...");

            if (uiManager) 
            {
                uiManager.ShowAnswer(answer);
            }
        }
        else
        {
            Debug.LogError($"Answer failed: {req.error}");
        }

        req.Dispose();
    }
}
