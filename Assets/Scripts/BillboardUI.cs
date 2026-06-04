using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera _mainCamera;

    [Tooltip("Offset above the NPC's head")]
    public Vector3 offset = new Vector3(0f, 2.5f, 0f);

    private Transform _npcRoot;

    void Start()
    {
        _mainCamera = Camera.main;
        // The NPC root is the parent of this Canvas
        _npcRoot = transform.parent;
    }

    void LateUpdate()
    {
        // Keep positioned above NPC
        transform.position = _npcRoot.position + offset;

        // Face the camera (true billboard)
        transform.rotation = Quaternion.LookRotation(
            transform.position - _mainCamera.transform.position
        );
    }
}