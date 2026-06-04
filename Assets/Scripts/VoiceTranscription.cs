using Oculus.Voice;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceTranscription : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;

    [Header("Input References")]
    [SerializeField] private InputActionReference activateVoiceInput;

    private string lastTranscription;

    void Start()
    {
        // Fires when the full transcription is ready
        appVoiceExperience.VoiceEvents.OnFullTranscription
            .AddListener(OnTranscriptionReceived);
    }

    void Update()
    {
        if(activateVoiceInput.action.WasPressedThisFrame())
        {
            Debug.Log("Activating voice experience...");
            appVoiceExperience.Activate();
        }
    }

    void OnEnable()
    {
        activateVoiceInput.action.Enable();
    }

    void OnDisable()
    {
        activateVoiceInput.action.Disable();
    }

    void OnTranscriptionReceived(string transcription)
    {
        lastTranscription = transcription;
        Debug.Log("Last transcription: " + lastTranscription);

        // Send to your agent here
        SendToAgent(lastTranscription);
    }

    void SendToAgent(string text)
    {
        // hook up to your GeminiAgent script here
    }

    void OnDestroy()
    {
        // Always clean up listeners
        appVoiceExperience.VoiceEvents.OnFullTranscription
            .RemoveListener(OnTranscriptionReceived);
    }
}