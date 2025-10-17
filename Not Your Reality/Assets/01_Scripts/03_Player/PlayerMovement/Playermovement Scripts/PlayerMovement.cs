using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private bool moveActive = true;
    [SerializeField] private bool jumpActive = true;

    [SerializeField, Range(0f, 50f)] private float acceleration = 10f;
    [SerializeField, Range(0f, 50f)] private float deceleration = 20f;
    [SerializeField, Range(0f, 1f)] private float stopThreshold = 0.2f;
    [SerializeField, Range(0f, 1f)] private float airControlMultiplier = 0.5f;

    private Vector3 currentVelocity;
    private Vector3 velocity;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField, Range(0f, 0.5f)] private float mouseSmoothTime = 0.03f;
    [SerializeField] private bool cameraActive = true;  // ?? Neu

    private CharacterController controller;
    private float xRotation;
    private Vector2 currentMouseDelta;
    private Vector2 smoothMouseDelta;

    [Header("Headbob Settings")]
    [SerializeField] private float bobFrequency = 10f;
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private float bobSway = 0.02f;

    [Header("Sprint Headbob Settings")]
    [SerializeField] private float sprintBobFrequency = 15f;
    [SerializeField] private float sprintBobAmplitude = 0.08f;
    [SerializeField] private float sprintBobSway = 0.03f;

    private float bobTimer = 0f;
    private Vector3 originalCameraPosition;

    public bool MoveActive
    {
        get => moveActive;
        set => moveActive = value;
    }

    public bool JumpActive
    {
        get => jumpActive;
        set => jumpActive = value;
    }

    public bool CameraActive
    {
        get => cameraActive;
        set
        {
            cameraActive = value;
            Cursor.lockState = cameraActive ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !cameraActive;
        }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalCameraPosition = cameraHolder.localPosition;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        ApplyGravity();
        HandleHeadBob();
    }

    private void HandleMouseLook()
    {
        if (!cameraActive) return; 

        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

        smoothMouseDelta.x = Mathf.SmoothDamp(smoothMouseDelta.x, targetMouseDelta.x, ref currentMouseDelta.x, mouseSmoothTime);
        smoothMouseDelta.y = Mathf.SmoothDamp(smoothMouseDelta.y, targetMouseDelta.y, ref currentMouseDelta.y, mouseSmoothTime);

        transform.Rotate(Vector3.up * smoothMouseDelta.x);
        xRotation -= smoothMouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (moveActive)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input.magnitude > 1f) input.Normalize();

            Vector3 targetDirection = transform.right * input.x + transform.forward * input.y;
            float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            Vector3 targetVelocity = targetDirection * targetSpeed;

            float accel = controller.isGrounded ? acceleration : acceleration * airControlMultiplier;
            float decel = controller.isGrounded ? deceleration : deceleration * airControlMultiplier;

            if (targetDirection.magnitude > 0.1f)
            {
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accel * Time.deltaTime);
            }
            else
            {
                currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, decel * Time.deltaTime);
                if (currentVelocity.magnitude < stopThreshold)
                    currentVelocity = Vector3.zero;
            }

            if (jumpActive && controller.isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }
        else
        {
            currentVelocity = Vector3.zero;
        }

        move = currentVelocity + velocity;
        controller.Move(move * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -0.05f;

        velocity.y += gravity * Time.deltaTime;
    }

    private void HandleHeadBob()
    {
        if (currentVelocity.magnitude > 0.1f && controller.isGrounded)
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);

            float frequency = isSprinting ? sprintBobFrequency : bobFrequency;
            float amplitude = isSprinting ? sprintBobAmplitude : bobAmplitude;
            float sway = isSprinting ? sprintBobSway : bobSway;

            bobTimer += Time.deltaTime * frequency;

            float bobOffsetY = Mathf.Sin(bobTimer) * amplitude;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * sway;

            cameraHolder.localPosition = originalCameraPosition + new Vector3(bobOffsetX, bobOffsetY, 0);
        }
        else
        {
            bobTimer = 0;
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, originalCameraPosition, Time.deltaTime * 5f);
        }
    }
}
