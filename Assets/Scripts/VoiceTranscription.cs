using Oculus.Voice;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceTranscription : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;

    [Header("Input References")]
    [SerializeField] private InputActionReference activateVoiceInput;

    [Header("Settings")]
    [SerializeField] private float activationRange = 3f; // how close to talk to an agent

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
        GeminiAgent nearestAgent = FindNearestAgent();
        if (nearestAgent != null)
        {
            nearestAgent.ReceiveMessage(text);
        }
        else
        {
            Debug.Log("No agent in range!");
        }
    }

    GeminiAgent FindNearestAgent()
    {
        GeminiAgent[] agents = FindObjectsByType<GeminiAgent>();
        GeminiAgent nearest = null;
        float minDist = activationRange; // only talk to agents within range

        foreach (var agent in agents)
        {
            float dist = Vector3.Distance(transform.position, agent.transform.position);
            if (dist < minDist && FindAnyObjectByType<NPCTriggerZone>().npcsInZone.Find(x => x.GetComponent<GeminiAgent>().Equals(agent)) != null)
            {
                minDist = dist;
                nearest = agent;
            }
        }
        return nearest;
    }

    void OnDestroy()
    {
        // Always clean up listeners
        appVoiceExperience.VoiceEvents.OnFullTranscription
            .RemoveListener(OnTranscriptionReceived);
    }
}