using Oculus.Voice;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

// Put this on each Agent GameObject
public class GeminiAgent : MonoBehaviour
{
    [TextArea(3, 10)]
    public string systemPrompt = "You are a helpful assistant.";

    [SerializeField] private string apiKey = "YOUR_GEMINI_KEY"; //Andrew's Gemini Key

    private string endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    private List<Dictionary<string, object>> chatHistory = new();

    public void ReceiveMessage(string userMessage)
    {
        Debug.Log($"[{gameObject.name}] Received: {userMessage}");

        chatHistory.Add(new Dictionary<string, object>
        {
            { "role", "user" },
            { "parts", new List<object> { new Dictionary<string, string> { { "text", userMessage } } } }
        });

        StartCoroutine(SendToGemini());
    }

    IEnumerator SendToGemini()
    {
        var requestBody = new Dictionary<string, object>
        {
            {
                "system_instruction", new Dictionary<string, object>
                {
                    { "parts", new List<object> { new Dictionary<string, string> { { "text", systemPrompt } } } }
                }
            },
            { "contents", chatHistory }
        };

        string json = JsonConvert.SerializeObject(requestBody);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(endpoint + "?key=" + apiKey, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = ParseGeminiResponse(request.downloadHandler.text);
            Debug.Log($"[{gameObject.name}] Response: {responseText}");

            // Add assistant response to history so agent remembers it
            chatHistory.Add(new Dictionary<string, object>
            {
                { "role", "model" },
                { "parts", new List<object> { new Dictionary<string, string> { { "text", responseText } } } }
            });

            // TODO: Display response in UI or TTS here
            OnResponseReceived(responseText);
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Gemini error: {request.error}\n{request.downloadHandler.text}");
        }
    }

    string ParseGeminiResponse(string json)
    {
        var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        var candidates = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response["candidates"].ToString());
        var content = JsonConvert.DeserializeObject<Dictionary<string, object>>(candidates[0]["content"].ToString());
        var parts = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content["parts"].ToString());
        return parts[0]["text"].ToString();
    }

    void OnResponseReceived(string response)
    {
        // Hook up your UI text or TTS here
        // e.g. GetComponent<TextMeshPro>().text = response;
    }
}