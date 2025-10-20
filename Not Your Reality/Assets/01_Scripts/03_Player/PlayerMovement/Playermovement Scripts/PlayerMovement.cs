using UnityEngine;

namespace _01_Scripts._03_Player.PlayerMovement.Playermovement_Scripts
{
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

      private Vector3 _currentVelocity;
      private Vector3 _velocity;

      [Header("Camera Settings")]
      [SerializeField] private Transform cameraHolder;
      [SerializeField] private float mouseSensitivity = 2f;
      [SerializeField] private float maxLookAngle = 80f;
      [SerializeField, Range(0f, 0.5f)] private float mouseSmoothTime = 0.03f;
      [SerializeField] private bool cameraActive = true; 

      private CharacterController _controller;
      private float _xRotation;
      private Vector2 _currentMouseDelta;
      private Vector2 _smoothMouseDelta;

      [Header("Headbob Settings")]
      [SerializeField] private float bobFrequency = 10f;
      [SerializeField] private float bobAmplitude = 0.05f;
      [SerializeField] private float bobSway = 0.02f;

      [Header("Sprint Headbob Settings")]
      [SerializeField] private float sprintBobFrequency = 15f;
      [SerializeField] private float sprintBobAmplitude = 0.08f;
      [SerializeField] private float sprintBobSway = 0.03f;

      private float _bobTimer = 0f;
      private Vector3 _originalCameraPosition;

      public bool MoveActive { get => moveActive; set => moveActive = value; }

      public bool JumpActive { get => jumpActive; set => jumpActive = value; }

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
         _controller = GetComponent<CharacterController>();
         Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false;

         _originalCameraPosition = cameraHolder.localPosition;
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

         _smoothMouseDelta.x = Mathf.SmoothDamp(
            _smoothMouseDelta.x,
            targetMouseDelta.x,
            ref _currentMouseDelta.x,
            mouseSmoothTime
         );
         _smoothMouseDelta.y = Mathf.SmoothDamp(
            _smoothMouseDelta.y,
            targetMouseDelta.y,
            ref _currentMouseDelta.y,
            mouseSmoothTime
         );

         transform.Rotate(Vector3.up * _smoothMouseDelta.x);
         _xRotation -= _smoothMouseDelta.y;
         _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);
         cameraHolder.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
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

            float accel = _controller.isGrounded ? acceleration : acceleration * airControlMultiplier;
            float decel = _controller.isGrounded ? deceleration : deceleration * airControlMultiplier;

            if (targetDirection.magnitude > 0.1f)
            {
               _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, accel * Time.deltaTime);
            }
            else
            {
               _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, decel * Time.deltaTime);
               if (_currentVelocity.magnitude < stopThreshold) _currentVelocity = Vector3.zero;
            }

            if (jumpActive && _controller.isGrounded && Input.GetButtonDown("Jump"))
            {
               _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
         }
         else { _currentVelocity = Vector3.zero; }

         move = _currentVelocity + _velocity;
         if (_controller.enabled) _controller.Move(move * Time.deltaTime);
      }

      private void ApplyGravity()
      {
         if (_controller.isGrounded && _velocity.y < 0f) _velocity.y = -0.05f;

         _velocity.y += gravity * Time.deltaTime;
      }

      private void HandleHeadBob()
      {
         if (_currentVelocity.magnitude > 0.1f && _controller.isGrounded)
         {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);

            float frequency = isSprinting ? sprintBobFrequency : bobFrequency;
            float amplitude = isSprinting ? sprintBobAmplitude : bobAmplitude;
            float sway = isSprinting ? sprintBobSway : bobSway;

            _bobTimer += Time.deltaTime * frequency;

            float bobOffsetY = Mathf.Sin(_bobTimer) * amplitude;
            float bobOffsetX = Mathf.Cos(_bobTimer * 0.5f) * sway;

            cameraHolder.localPosition = _originalCameraPosition + new Vector3(bobOffsetX, bobOffsetY, 0);
         }
         else
         {
            _bobTimer = 0;
            cameraHolder.localPosition = Vector3.Lerp(
               cameraHolder.localPosition,
               _originalCameraPosition,
               Time.deltaTime * 5f
            );
         }
      }
   }
}