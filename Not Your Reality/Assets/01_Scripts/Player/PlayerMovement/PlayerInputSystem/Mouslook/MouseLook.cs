using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerBody;
    public Transform cameraHolder;
    public float mouseSensitivity = 2f;

    [Header("Clipping Settings")]
    [SerializeField] private float cameraRadius = 0.1f;
    [SerializeField] private LayerMask collisionLayers; 

    private float xRotation = 0f;

    [Header("Input Action")]
    public InputActionReference lookAction;

    private Vector3 originalLocalPosition;

    private void Awake()
    {
        if (cameraHolder != null)
            originalLocalPosition = cameraHolder.localPosition;
    }

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

        HandleMouseLook();
        HandleCameraClipping();
    }

    private void HandleMouseLook()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        playerBody.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleCameraClipping()
    {
        Vector3 headPosition = playerBody.position + Vector3.up * 1.8f; 
        Vector3 desiredCameraPos = headPosition + playerBody.TransformDirection(originalLocalPosition);

        if (Physics.SphereCast(headPosition, cameraRadius, desiredCameraPos - headPosition,
            out RaycastHit hit, Vector3.Distance(headPosition, desiredCameraPos), collisionLayers))
        {
            cameraHolder.position = hit.point + hit.normal * 0.01f; 
        }
        else
        {
            cameraHolder.position = desiredCameraPos;
        }
    }
}
