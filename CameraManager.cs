using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using OpenAI;

public class CameraManager : MonoBehaviour
{
    [Header("UI Reference")]
    // This single RawImage is used for both live preview and for displaying the captured image.
    public RawImage cameraPreview;

    [Header("External References")]
    // ChatGPT GameObject that contains the ChatGPT component.
    public GameObject chatGPTGameObject;
    // TextToSpeech component reference.
    public TextToSpeech textToSpeech;

    private ChatGPT chatGPTScript;  // Cached ChatGPT component reference.
    private WebCamTexture webcamTexture;

    // Replace these with your actual API keys.
    private string googleCloudApiKey = "AIzaSyDU4XK4mt56IPDL_e_cS3KTjmKm3jXG_Rk";
    private string openAIApiKey = "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";

    private void Start()
    {
        // Start with the RawImage hidden.
        if (cameraPreview != null)
            cameraPreview.gameObject.SetActive(false);

        // Initialize and assign the webcam texture.
        webcamTexture = new WebCamTexture();
        if (cameraPreview != null)
            cameraPreview.texture = webcamTexture;

        // Cache the ChatGPT component.
        if (chatGPTGameObject != null)
        {
            chatGPTScript = chatGPTGameObject.GetComponent<ChatGPT>();
            if (chatGPTScript == null)
                Debug.LogError("ChatGPT script not found on the provided ChatGPT GameObject!");
        }
        else
        {
            Debug.LogWarning("ChatGPT GameObject is not assigned in CameraManager!");
        }
    }

    /// <summary>
    /// Called when the Capture button is clicked.
    /// This method starts the capture sequence:
    /// 1. Enables and starts the live camera preview.
    /// 2. Waits 5 seconds.
    /// 3. Captures a static image from the live feed.
    /// 4. Stops the live feed so that the RawImage now shows the captured image.
    /// 5. Sends the image for analysis.
    /// 6. Waits until TTS starts (up to a timeout) and then waits until TTS finishes speaking.
    /// 7. Finally, hides the RawImage.
    /// </summary>
    public void StartCaptureSequence()
    {
        StartCoroutine(CaptureSequence());
    }

    IEnumerator CaptureSequence()
    {
        // --- Ensure that the live feed is used ---
        if (cameraPreview != null)
        {
            // Reassign the RawImage's texture to the live webcam texture
            cameraPreview.texture = webcamTexture;
            cameraPreview.gameObject.SetActive(true);
        }
        if (webcamTexture != null && !webcamTexture.isPlaying)
        {
            webcamTexture.Play();
        }

        // 2. Wait 5 seconds so the user can see what they are about to capture.
        yield return new WaitForSeconds(5f);

        // 3. Capture a snapshot from the live feed.
        Texture2D capturedTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
        capturedTexture.SetPixels(webcamTexture.GetPixels());
        capturedTexture.Apply();

        // 4. Stop the live feed so that the RawImage now shows the static captured image.
        if (webcamTexture.isPlaying)
            webcamTexture.Stop();

        // Replace the live texture with the captured snapshot.
        cameraPreview.texture = capturedTexture;

        // Convert the captured image to Base64.
        byte[] imageBytes = capturedTexture.EncodeToJPG();
        string imageBase64 = System.Convert.ToBase64String(imageBytes);

        // 5. Send the captured image to the Google Vision API for analysis.
        yield return StartCoroutine(AnalyzeImage(imageBase64));

        // --- New waiting logic for TTS ---
        // Allow a brief moment for ChatGPT to process and trigger TTS.
        yield return new WaitForSeconds(0.5f);

        // Wait for up to 15 seconds for TTS to start speaking.
        float waited = 0f;
        while (waited < 15f && !textToSpeech.IsSpeaking())
        {
            waited += Time.deltaTime;
            yield return null;
        }
        // If TTS has started, then wait until it finishes.
        if (textToSpeech.IsSpeaking())
        {
            yield return new WaitUntil(() => !textToSpeech.IsSpeaking());
        }
        // --- End new waiting logic ---

        // 7. Hide the RawImage after TTS is finished (or if it never started).
        if (cameraPreview != null)
            cameraPreview.gameObject.SetActive(false);
    }

    IEnumerator AnalyzeImage(string imageBase64)
    {
        string url = "https://vision.googleapis.com/v1/images:annotate?key=" + googleCloudApiKey;

        var requestBody = new
        {
            requests = new List<object>
            {
                new
                {
                    image = new { content = imageBase64 },
                    features = new List<object>
                    {
                        new { type = "LABEL_DETECTION", maxResults = 10 },
                        new { type = "TEXT_DETECTION" },
                        new { type = "OBJECT_LOCALIZATION", maxResults = 10 }
                    }
                }
            }
        };

        string jsonPayload = JsonConvert.SerializeObject(requestBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Google Vision API Response: " + request.downloadHandler.text);
                ParseVisionResponse(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                if (chatGPTScript != null)
                    chatGPTScript.AppendReceivedMessage("Error analyzing image.");
            }
        }
    }

    void ParseVisionResponse(string jsonResponse)
    {
        // Deserialize the JSON response.
        var response = JsonConvert.DeserializeObject<GoogleVisionResponse>(jsonResponse);

        if (response != null && response.responses.Count > 0)
        {
            var visionResponse = response.responses[0];

            // Prepare the detected labels string.
            string detectedLabels = "Detected Objects:\n";
            if (visionResponse.labelAnnotations != null)
            {
                foreach (var label in visionResponse.labelAnnotations)
                {
                    detectedLabels += $"{label.description} ({label.score * 100:F1}%)\n";
                }
            }
            else
            {
                detectedLabels += "No labels detected.\n";
            }

            // Prepare the detected text string.
            string detectedText = "Detected Text: ";
            if (visionResponse.textAnnotations != null && visionResponse.textAnnotations.Count > 0)
            {
                detectedText += visionResponse.textAnnotations[0].description;
            }
            else
            {
                detectedText += "No text detected.";
            }

            string combinedDetails = $"{detectedLabels}\n{detectedText}";
            Debug.Log(combinedDetails);

            // Send the analysis details to ChatGPT for further processing.
            if (chatGPTScript != null)
                StartCoroutine(SendToChatGPT(combinedDetails));
        }
        else
        {
            Debug.LogError("No valid response from Google Vision API.");
            if (chatGPTScript != null)
                chatGPTScript.AppendReceivedMessage("No objects or text detected.");
        }
    }

    IEnumerator SendToChatGPT(string visionDetails)
    {
        string url = "https://api.openai.com/v1/chat/completions";

        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "You are an assistant that analyzes objects detected by an image analysis system." },
                new { role = "user", content = "Analyze the following details and provide a comprehensive description of the object(s):\n" + visionDetails }
            }
        };

        string jsonPayload = JsonConvert.SerializeObject(requestBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + openAIApiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log("GPT-4 Response: " + response);
                DisplayResponse(response);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                if (chatGPTScript != null)
                    chatGPTScript.AppendReceivedMessage("Error generating analysis.");
            }
        }
    }

    void DisplayResponse(string jsonResponse)
    {
        var response = JsonConvert.DeserializeObject<OpenAIResponse>(jsonResponse);
        if (response != null && response.choices.Count > 0)
        {
            string detailedDescription = response.choices[0].message.content;
            Debug.Log("Detailed Description: " + detailedDescription);
            if (chatGPTScript != null)
                chatGPTScript.AppendReceivedMessage(detailedDescription);
        }
        else
        {
            Debug.LogError("Invalid response from ChatGPT.");
            if (chatGPTScript != null)
                chatGPTScript.AppendReceivedMessage("Error generating detailed description.");
        }
    }
}

// ---------------------------
// Google Vision Response Classes
// ---------------------------
[System.Serializable]
public class GoogleVisionResponse
{
    public List<VisionResponse> responses;
}

[System.Serializable]
public class VisionResponse
{
    public List<LabelAnnotation> labelAnnotations;
    public List<TextAnnotation> textAnnotations;
}

[System.Serializable]
public class LabelAnnotation
{
    public string description;
    public float score;
}

[System.Serializable]
public class TextAnnotation
{
    public string description;
}

// ---------------------------
// OpenAI Response Classes
// ---------------------------
[System.Serializable]
public class OpenAIResponse
{
    public List<OpenAIChoice> choices;
}

[System.Serializable]
public class OpenAIChoice
{
    public OpenAIMessage message;
}

[System.Serializable]
public class OpenAIMessage
{
    public string content;
}


