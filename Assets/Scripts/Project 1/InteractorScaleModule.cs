using UnityEngine;
using UnityEngine.InputSystem;

public class InteractorScaleModule : MonoBehaviour
{
    public InputActionProperty leftControllerPos, rightControllerPos;
    public InputActionProperty leftControllerGrip, rightControllerGrip;
    public Transform selectedObject = null;
    private Vector3 initLeftControllerPos, initRightControllerPos;
    private float initDist;

    void OnEnable()
    {
        leftControllerGrip.action.performed += OnLeftGrip;
    }

    void OnDisable()
    {
        leftControllerGrip.action.performed -= OnLeftGrip;   
    }

    void Update()
    {
        float leftGripped = leftControllerGrip.action.ReadValue<float>();
        float rightGripped = rightControllerGrip.action.ReadValue<float>();

        bool doubleGripped = leftGripped == 1f && rightGripped == 1f;

        if(doubleGripped && selectedObject != null)
        {
            Vector3 leftPos = leftControllerPos.action.ReadValue<Vector3>();
            Vector3 rightPos = rightControllerPos.action.ReadValue<Vector3>();

            float currDist = Vector3.Distance(leftPos, rightPos);

            // float delta = currDist - initDist;
            // code here
            selectedObject.localScale *= currDist / initDist;
            initDist = currDist;
        }
    }

    void OnLeftGrip(InputAction.CallbackContext context)
    {
        if(selectedObject == null) return;

        initLeftControllerPos = leftControllerPos.action.ReadValue<Vector3>();
        initRightControllerPos = rightControllerPos.action.ReadValue<Vector3>();

        initDist = Vector3.Distance(initLeftControllerPos, initRightControllerPos);
    }
}
