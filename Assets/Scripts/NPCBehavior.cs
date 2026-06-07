using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehavior : MonoBehaviour
{
    private Animator animator;
    
    [Header("Movement Settings")]
    public float changeStateInterval = 5f; // How long they do an action
    public float moveSpeed = 2f;
    public Vector2 moveDistanceRange = new(1, 5);

    [HideInInspector] public bool IsNearPlayer; // Set by the trigger zone when the player is close
    [HideInInspector] public int CurrentState = 0; // 0=Idle, 1=Walking, 2=Sad, 3=Happy, 4=Angry

    private float timer;
    private Vector3 moveDirection;
    private Vector3 targetPosition;

    private XROrigin playerOrigin;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerOrigin = FindAnyObjectByType<XROrigin>();
        timer = changeStateInterval;
        DetermineNextBehavior();
    }

    void Update()
    {
        if (IsNearPlayer)
        {
            transform.LookAt(playerOrigin.transform);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            DetermineNextBehavior();
            timer = changeStateInterval; // Reset timer
        }

        // Move the NPC if they are in the walking state (State 1)
        if (CurrentState == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (moveDirection != Vector3.zero)
            {
                transform.forward = moveDirection;
            }

            if (transform.position == targetPosition)
            {
                CurrentState = 0;
            }
        }

        animator.SetInteger("State", CurrentState);
    }

    void DetermineNextBehavior()
    {
        // 1. Roll a percentage dice (0.0 to 1.0)
        float diceRoll = Random.value;

        if (diceRoll < 0.50f) 
        {
            // 50% CHANCE: Walk around
            CurrentState = 1; 
            
            // Pick a random direction on the ground plane
            float randomAngle = Random.Range(0f, 360f);
            moveDirection = new Vector3(Mathf.Sin(randomAngle), 0, Mathf.Cos(randomAngle)).normalized;

            NavMesh.SamplePosition(transform.position + moveDirection * Random.Range(moveDistanceRange.x, moveDistanceRange.y), out NavMeshHit hit, moveDistanceRange.y, 1);
            targetPosition = hit.position;
        }
        else 
        {
            // 50% CHANCE: Stay Idle / Stand around
            // Let's decide if they just stand normally or show an emotion
            float emotionRoll = Random.value;

            if (emotionRoll < 0.40f)
            {
                // 40% of the time while standing, choose a random emotion (State 2, 3, or 4)
                CurrentState = Random.Range(2, 5); 
            }
            else
            {
                // 60% of the time while standing, just play regular boring Idle
                CurrentState = 0; 
            }
        }
    }

    public void ReactToPlayerMessage(string emotionTag)
    {
        switch (emotionTag)
        {
            case "Sad":
                CurrentState = 2; // Sad
                break;
            case "Happy":
                CurrentState = 3; // Happy
                break;
            case "Angry":
                CurrentState = 4; // Angry
                break;
            default:
                return; // If we get an unrecognized tag, do nothing
        }
    }
}