using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerBody;      
    public Transform cameraHolder;    
    public float mouseSensitivity = 2f;

    private float xRotation = 0f;

    [Header("Input Action")]
    public InputActionReference lookAction;

    private void OnEnable()
    {
        lookAction?.action.Enable();
    }

    private void OnDisable()
    {
        lookAction?.action.Disable();
    }

    void Update()
    {
        if (lookAction == null || playerBody == null || cameraHolder == null) return;

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        playerBody.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
