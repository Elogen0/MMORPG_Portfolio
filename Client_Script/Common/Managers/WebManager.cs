using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Kame;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : SingletonMono<WebManager>
{
    public string BaseUrl { get; set; } = "https://localhost:5001/api";

    private void Start()
    {
    }

    public void SendPostRequest<T>(string url, object obj, Action<T> res)
    {
        StartCoroutine(CoSendWebRequest(url, UnityWebRequest.kHttpVerbPOST, obj, res));
    }
    
    IEnumerator CoSendWebRequest<T>(string url, string method, object obj, Action<T> res)
    {
        string sendUrl = $"{BaseUrl}/{url}";

        byte[] jsonBytes = null;
        if (obj != null)
        {
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
        }

        using (var request = new UnityWebRequest(sendUrl, method))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer(); //응답 왔을때
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                T resObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                res.Invoke(resObj);
            }
        }
    }
}
