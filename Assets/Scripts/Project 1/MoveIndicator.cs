using UnityEngine;
using UnityEngine.InputSystem;

public class MoveIndicator : MonoBehaviour
{
    public InputActionProperty moveAction;
    public Transform mainCamera;

    void Update()
    {
        Vector2 joystickVector = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = Quaternion.Euler(0, mainCamera.localEulerAngles.y, 0) * new Vector3(joystickVector.x, 0, joystickVector.y);
        float angle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

        if(joystickVector.magnitude > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.localEulerAngles = new(0f, angle, 0f);
            transform.localPosition = new(mainCamera.localPosition.x, 0, mainCamera.localPosition.z);
        } else
        {
            transform.GetChild(0).gameObject.SetActive(false);            
        }
    }
}
