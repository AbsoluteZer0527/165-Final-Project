using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class NPCBehavior : MonoBehaviour
{
    public enum NPCState // 0=Idle, 1=Walking, 2=Sad, 3=Happy, 4=Angry
    {
        Idle, Walking, Sad, Happy, Angry
    }

    private Animator animator;
    private XROrigin playerOrigin;
    private NavMeshAgent navMeshAgent;

    [Header("Movement Settings")]
    public float changeStateInterval = 5f; // How long they do an action
    public float moveSpeed = 2f;
    public Vector2 moveDistanceRange = new(1, 5);

    [HideInInspector] public bool IsNearPlayer; // Set by the trigger zone when the player is close
    [HideInInspector] public NPCState CurrentState = NPCState.Idle; 

    private float timer;
    private Vector3 moveDirection;
    private Vector3 targetPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerOrigin = FindAnyObjectByType<XROrigin>();

        timer = changeStateInterval;
        DetermineNextBehavior();
    }

    void Update()
    {
        if (IsNearPlayer)
        {
            navMeshAgent.destination = transform.position;
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

        if (CurrentState == NPCState.Walking)
        {
            navMeshAgent.destination = targetPosition;

            if (Vector3.Magnitude(transform.position - targetPosition) < 0.1f)
            {
                CurrentState = 0;
            }
        }
        else
        {
            navMeshAgent.destination = transform.position;
        }

        animator.SetInteger("State", (int)CurrentState);
    }

    void DetermineNextBehavior()
    {
        float diceRoll = Random.value;

        // Go to random point on NavMesh
        if (diceRoll < 0.50f) 
        {
            CurrentState = NPCState.Walking;

            // Calculate random point on mesh
            NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
            Mesh mesh = new()
            {
                vertices = triangles.vertices,
                triangles = triangles.indices
            };

            int[] tris = mesh.triangles;
            Vector3[] verts = mesh.vertices;

            // Calculate triangle areas via 1/2 * AB cross AC, CSE 167 ahh code follows
            float[] triAreas = new float[tris.Length / 3];
            for (int i = 0; i < triAreas.Length; i++)
            {
                triAreas[i] = 0.5f * Vector3.Cross(verts[tris[i * 3]] - verts[tris[i * 3 + 1]], verts[tris[i * 3]] - verts[tris[i * 3 + 2]]).magnitude;
            }

            // Random weighted sample to choose random triangle
            float[] triAreasSum = new float[triAreas.Length];
            float areaSum = 0;
            for (int i = 0; i < triAreas.Length; i++)
            {
                areaSum += triAreas[i];
                triAreasSum[i] = areaSum;
            }

            int triIndex = 0;
            float randomsample = Random.value * areaSum;

            for (int i = 0; i < triAreas.Length; i++)
            {
                if (randomsample <= triAreasSum[i])
                {
                    triIndex = i;
                    break;
                }
            }

            // Random sample in chosen triangle in barycentric coords
            Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
            Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
            Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

            float r = Random.value;
            float s = Random.value;

            if (r + s >= 1)
            {
                r = 1 - r;
                s = 1 - s;
            }

            // Barycentric to world coords
            Vector3 randomPoint = a + r * (b - a) + s * (c - a);
            targetPosition = randomPoint;
        }
        else 
        {
            float emotionRoll = Random.value;

            // Random emotion animation
            if (emotionRoll < 0.40f)
            {
                CurrentState = (NPCState)Random.Range(2, 5); 
            }
            // Idle animation
            else
            {
                CurrentState = NPCState.Idle; 
            }
        }
    }

    public void ReactToPlayerMessage(string emotionTag)
    {
        switch (emotionTag)
        {
            case "Sad":
                CurrentState = NPCState.Sad;
                break;
            case "Happy":
                CurrentState = NPCState.Happy;
                break;
            case "Angry":
                CurrentState = NPCState.Angry;
                break;
            default:
                return;
        }
    }
}