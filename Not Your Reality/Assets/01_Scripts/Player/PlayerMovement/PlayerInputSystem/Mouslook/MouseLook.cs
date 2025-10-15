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

    [Header("Input Action")]
    public InputActionReference lookAction;

    private float xRotation = 0f;
    private Vector3 originalLocalPos;

    private void Awake()
    {
        if (cameraHolder != null)
            originalLocalPos = cameraHolder.localPosition;
    }

    private void OnEnable() => lookAction?.action.Enable();
    private void OnDisable() => lookAction?.action.Disable();

    private void Update()
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
        float eyeHeight = 1.8f;
        Vector3 headPos = playerBody.position + Vector3.up * eyeHeight;

        Vector3 desiredCamPos = cameraHolder.position;
        Vector3 dir = desiredCamPos - headPos;
        float dist = dir.magnitude;

        if (Physics.SphereCast(headPos, cameraRadius, dir.normalized, out RaycastHit hit, dist, collisionLayers))
        {
            cameraHolder.position = Vector3.Lerp(cameraHolder.position, hit.point - dir.normalized * cameraRadius, 15f * Time.deltaTime);
        }
        else
        {
            cameraHolder.position = Vector3.Lerp(cameraHolder.position, desiredCamPos, 15f * Time.deltaTime);
        }
    }
}
