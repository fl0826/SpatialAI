using System;

namespace SpatialAI.Network
{
    /// <summary>
    /// API 请求数据模型
    /// </summary>
    [Serializable]
    public class AnalyzeRequest
    {
        public string image;
        public long timestamp;
    }

    [Serializable]
    public class AnswerRequest
    {
        public string object_id;
        public string question;
    }

    /// <summary>
    /// API 响应数据模型
    /// </summary>
    [Serializable]
    public class ObjectInfo
    {
        public string @object;
        public string category;
        public string description;
        public string context;
    }

    [Serializable]
    public class AnalyzeResponse
    {
        public string object_id;
        public ObjectInfo @object;
        public string[] questions;
    }

    [Serializable]
    public class AnswerResponse
    {
        public string answer;
    }
}
