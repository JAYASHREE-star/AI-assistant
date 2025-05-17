
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GoogleVisionAnalyzer : MonoBehaviour
{
    [Header("Google Cloud Settings")]
    public string apiKey = "AIzaSyDU4XK4mt56IPDL_e_cS3KTjmKm3jXG_Rk";

    public IEnumerator AnalyzeImage(byte[] imageBytes, Action<string> onComplete)
    {
        string visionApiUrl = $"https://vision.googleapis.com/v1/images:annotate?key={apiKey}";

        // Create the JSON payload for Vision API
        var requestBody = new
        {
            requests = new[]
            {
                new
                {
                    image = new { content = Convert.ToBase64String(imageBytes) },
                    features = new[] { new { type = "LABEL_DETECTION", maxResults = 5 } }
                }
            }
        };

        string jsonData = JsonConvert.SerializeObject(requestBody);

        using (UnityWebRequest request = new UnityWebRequest(visionApiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Vision API Response: " + request.downloadHandler.text);
                onComplete?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Vision API Error: " + request.error);
                onComplete?.Invoke(null);
            }
        }
    }
}

