# 🧠 Unity AI Virtual Assistant

An intelligent, voice-interactive 3D assistant powered by Unity and AI technologies like OpenAI GPT, Azure Speech, and real-time APIs. This assistant can listen, understand, speak, animate, and perform desktop tasks — bridging immersive UX and smart automation.

> 🚀 Designed as a portfolio-grade project showcasing AI integration, speech interfaces, and Unity development in a practical real-world assistant app.

---

🧩 Key Features

 1.🔊 Voice Interaction
- Microphone-based speech recognition (Windows or Azure)
- Wake word detection (e.g., "Hey Ava") via Picovoice or custom trigger

2.🧠 Conversational AI
- Dynamic responses using OpenAI's GPT (ChatGPT-like behavior)
- Multi-turn conversation with short-term memory
- Emotion-aware replies using voice tone or context (via sentiment API)

3.🗣️ Realistic Speaking Avatar
- 3D rigged assistant (Mixamo + Unity Animator)
- Auto lip sync (SALSA LipSync) and expressive facial animations
- Custom animations mapped to emotions or task responses

4.📱 Task Execution
- Open installed apps (calculator, browser, notepad)
- Tell weather, date, time
- Launch websites or play music via voice

5.🌍 Real-Time API Integration
- Weather (OpenWeatherMap)
- News headlines (NewsAPI or Bing News Search)
- Optional: Currency converter, calendar sync, reminders

6.📋 UI/UX Features
- Scrollable chat panel (user + assistant messages)
- Dynamic UI feedback (listening, thinking, responding states)
- Dark/light mode toggle
- Voice-to-text transcript view
- Typing fallback when mic is off

---

  🧰 Tech Stack

| Feature        | Stack/Tools                                 |
|----------------|----------------------------------------------|
| Game Engine    | Unity 2021+ (URP or HDRP optional)           |
| Language       | C#                                           |
| NLP            | OpenAI GPT-3.5 / GPT-4 API                   |
| Voice Input    | Azure Speech SDK / System.Speech             |
| TTS            | Azure Neural Voices / Windows TTS            |
| Wake Word      | Porcupine Wake Word SDK (Unity Plugin)       |
| Lip Sync       | SALSA LipSync + Unity Blend Shapes           |
| Avatar Rigging | Mixamo + Animator Controller                 |
| Weather API    | OpenWeatherMap                               |
| UI             | TextMeshPro, Unity UI Toolkit                |

---

## 🎬 Demo Preview
 
> [🔗 GitHub Repo](https://github.com/JAYASHREE-star/AI-assistant)

---

## 🛠️ Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/unity-ai-assistant.git
````

### 2. Open in Unity

Use Unity Hub (2021+ recommended). Ensure the microphone is enabled in Editor permissions.

### 3. Install Dependencies

* [TextMeshPro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html)
* [Azure Speech SDK for Unity](https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/quickstarts)
* [SALSA LipSync](https://assetstore.unity.com/packages/tools/audio/salsa-lipsync-suite-148442)
* [Picovoice Porcupine (optional)](https://github.com/Picovoice/porcupine)

### 4. Configure API Keys

Create a `config.cs` or use environment variables.

```csharp
public static class Keys {
    public const string OPENAI_KEY = "sk-...";
    public const string AZURE_TTS_KEY = "...";
    public const string WEATHER_API_KEY = "...";
}
```

---

## 🗃️ Project Structure

```
Assets/
├── Scripts/
│   ├── AI/OpenAIHandler.cs
│   ├── Voice/VoiceRecognizer.cs
│   ├── TTS/TextToSpeech.cs
│   ├── System/TaskExecutor.cs
│   ├── Utilities/EmotionAnalyzer.cs
├── UI/
│   ├── ChatManager.cs
│   ├── AudioStateIndicator.cs
├── Prefabs/
│   ├── AssistantAvatar.prefab
├── Resources/
│   └── Sounds/
└── StreamingAssets/
    └── Config/
        └── keys.json
```

---

## 1.🌟 Advanced Ideas for Future Development

* 🧠 **Long-Term Memory**: Store user preferences, names, prior context using SQLite or JSON DB.
* 🌐 **Browser UI Integration**: WebView or WebGL build for Web-based assistant.
* 🤳 **AR Mode**: Integrate with Unity AR Foundation to place assistant in real-world environment.
* 🧠 **Self-Learning**: Fine-tune assistant behavior based on feedback or usage patterns.
* 📸 **Facial Recognition**: Detect user via webcam and tailor conversation.

---


### 2. Open in Unity

Use Unity Hub (2021+ recommended). Ensure the microphone is enabled in Editor permissions.

### 3. Install Dependencies

* [TextMeshPro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html)
* [Azure Speech SDK for Unity](https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/quickstarts)
* [SALSA LipSync](https://assetstore.unity.com/packages/tools/audio/salsa-lipsync-suite-148442)
* [Picovoice Porcupine (optional)](https://github.com/Picovoice/porcupine)

### 4. Configure API Keys

Create a `config.cs` or use environment variables.

```csharp
public static class Keys {
    public const string OPENAI_KEY = "sk-...";
    public const string AZURE_TTS_KEY = "...";
    public const string WEATHER_API_KEY = "...";
}
```

---

## 🗃️ Project Structure

```
Assets/
├── Scripts/
│   ├── AI/OpenAIHandler.cs
│   ├── Voice/VoiceRecognizer.cs
│   ├── TTS/TextToSpeech.cs
│   ├── System/TaskExecutor.cs
│   ├── Utilities/EmotionAnalyzer.cs
├── UI/
│   ├── ChatManager.cs
│   ├── AudioStateIndicator.cs
├── Prefabs/
│   ├── AssistantAvatar.prefab
├── Resources/
│   └── Sounds/
└── StreamingAssets/
    └── Config/
        └── keys.json
```

---

## 🌟 Advanced Ideas for Future Development

* 🧠 **Long-Term Memory**: Store user preferences, names, prior context using SQLite or JSON DB.
* 🌐 **Browser UI Integration**: WebView or WebGL build for Web-based assistant.
* 🤳 **AR Mode**: Integrate with Unity AR Foundation to place assistant in real-world environment.
* 🧠 **Self-Learning**: Fine-tune assistant behavior based on feedback or usage patterns.
* 📸 **Facial Recognition**: Detect user via webcam and tailor conversation.

---

## 🤝 Contributing

Want to contribute? Here's how:

```bash
1. Fork the repo
2. Create your feature branch (git checkout -b feature/YourFeature)
3. Commit your changes (git commit -m 'Add Feature')
4. Push to the branch (git push origin feature/YourFeature)
5. Open a Pull Request
```

---

## ⭐ If you like this project...

* Leave a 🌟 on GitHub
* Share the demo video on LinkedIn
