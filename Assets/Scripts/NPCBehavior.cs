using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    private Animator animator;
    
    [Header("Movement Settings")]
    public float changeStateInterval = 5f; // How long they do an action
    public float moveSpeed = 2f;
    
    private float timer;
    private int currentState = 0; // 0=Idle, 1=Walking, 2=Sad, 3=Happy, 4=Angry
    private Vector3 moveDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
        timer = changeStateInterval;
        DetermineNextBehavior();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            DetermineNextBehavior();
            timer = changeStateInterval; // Reset timer
        }

        // Move the NPC if they are in the walking state (State 1)
        if (currentState == 1)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            // Optional: Make the character face the direction they are walking
            if (moveDirection != Vector3.zero)
            {
                transform.forward = moveDirection;
            }
        }
    }

    void DetermineNextBehavior()
    {
        // 1. Roll a percentage dice (0.0 to 1.0)
        float diceRoll = Random.value;

        if (diceRoll < 0.50f) 
        {
            // 50% CHANCE: Walk around
            currentState = 1; 
            
            // Pick a random direction on the ground plane
            float randomAngle = Random.Range(0f, 360f);
            moveDirection = new Vector3(Mathf.Sin(randomAngle), 0, Mathf.Cos(randomAngle)).normalized;
        }
        else 
        {
            // 50% CHANCE: Stay Idle / Stand around
            // Let's decide if they just stand normally or show an emotion
            float emotionRoll = Random.value;

            if (emotionRoll < 0.40f)
            {
                // 40% of the time while standing, choose a random emotion (State 2, 3, or 4)
                currentState = Random.Range(2, 5); 
            }
            else
            {
                // 60% of the time while standing, just play regular boring Idle
                currentState = 0; 
            }
        }

        // Sync our calculation directly to the Animator component
        animator.SetInteger("State", currentState);
    }
}