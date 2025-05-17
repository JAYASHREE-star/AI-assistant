
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json; // Install Newtonsoft.Json from Unity Package Manager

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;
        [SerializeField] private TextToSpeech textToSpeech; // Text-to-speech reference

        private float height;
        private OpenAIApi openai;
        private List<ChatMessage> messages = new List<ChatMessage>();

        private string prompt = "Act as an AI Model. Reply to user questions.";

        // Google Drive direct download link
        private string jsonFileUrl = "https://drive.google.com/uc?export=download&id=1qz_udb7R2-3KuMr5NqloUamCDNI1sFHw";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
            StartCoroutine(FetchAPIKeyFromGoogleDrive());
        }

        private IEnumerator FetchAPIKeyFromGoogleDrive()
        {
            UnityWebRequest request = UnityWebRequest.Get(jsonFileUrl);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var jsonData = JsonConvert.DeserializeObject<AuthData>(request.downloadHandler.text);
                    openai = new OpenAIApi(jsonData.api_key, jsonData.organization);
                    Debug.Log("API Key and Organization loaded successfully!");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("Failed to fetch JSON: " + request.error);
            }
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            if (openai == null)
            {
                Debug.LogError("OpenAI API is not initialized yet!");
                return;
            }

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };

            AppendMessage(newMessage);

            // If this is the first message, prepend the prompt.
            if (messages.Count == 0)
                newMessage.Content = prompt + "\n" + inputField.text;

            messages.Add(newMessage);

            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-4o-mini",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);
                AppendMessage(message);

                // Trigger text-to-speech for ChatGPT's response.
                textToSpeech.GenerateAudio(message.Content);
            }
            else
            {
                Debug.LogWarning("No response generated.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }

        // PUBLIC METHOD: Allows other scripts (like CameraManager) to append a received message
        // and have it spoken via TextToSpeech.
        public void AppendReceivedMessage(string messageContent)
        {
            ChatMessage newMessage = new ChatMessage()
            {
                Role = "assistant",
                Content = messageContent
            };
            AppendMessage(newMessage);

            // Trigger text-to-speech for the received message.
            if (textToSpeech != null)
            {
                textToSpeech.GenerateAudio(messageContent);
            }
            else
            {
                Debug.LogWarning("TextToSpeech component not assigned in ChatGPT.");
            }
        }

        // Class to deserialize JSON
        private class AuthData
        {
            public string api_key;
            public string organization;
        }
    }
}

