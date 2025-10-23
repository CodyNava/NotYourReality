using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Clipping Settings")]
    [SerializeField] private float cameraRadius = 0.1f;
    [SerializeField] private LayerMask collisionLayers;

    private float xRotation = 0f;
    private Vector3 originalLocalPos;
    private bool lookEnabled = true;

    private void Awake()
    {
        if (cameraHolder != null)
            originalLocalPos = cameraHolder.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!lookEnabled || playerBody == null || cameraHolder == null)
            return;

        HandleMouseLook();
        HandleCameraClipping();

        // Toggle Look mit Escape
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleLook();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Body dreht sich horizontal
        playerBody.Rotate(Vector3.up * mouseX);

        // Kamera rotiert vertikal
        xRotation -= mouseY;
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

    private void ToggleLook()
    {
        lookEnabled = !lookEnabled;
        Cursor.lockState = lookEnabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lookEnabled;
    }
}
