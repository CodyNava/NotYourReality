using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.PlayerMovement.Movement
{
   [RequireComponent(typeof(CharacterController))]
   public class PlayerController : MonoBehaviour
   {
      [Header("Movement Settings")]
      [SerializeField] private float walkSpeed = 5f;
      [SerializeField] private float sprintSpeed = 8f;
      [SerializeField] private bool moveActive = true;

      [SerializeField, Range(0f, 50f)] private float acceleration = 10f;
      [SerializeField, Range(0f, 50f)] private float deceleration = 20f;
      [SerializeField, Range(0f, 1f)] private float stopThreshold = 0.2f;

      [Header("Camera Settings")]
      [SerializeField] private Transform cameraHolder;
      [SerializeField] private float mouseSensitivity = 2f;
      [SerializeField] private float maxLookAngle = 80f;
      [SerializeField, Range(0f, 0.5f)] private float mouseSmoothTime = 0.03f;
      [SerializeField] private bool cameraActive = true;

      [Header("Headbob Settings")]
      [SerializeField] private float bobFrequency = 10f;
      [SerializeField] private float bobAmplitude = 0.05f;
      [SerializeField] private float bobSway = 0.02f;

      [Header("Sprint Headbob Settings")]
      [SerializeField] private float sprintBobFrequency = 15f;
      [SerializeField] private float sprintBobAmplitude = 0.08f;
      [SerializeField] private float sprintBobSway = 0.03f;

      [Header("Camera Clipping")]
      [SerializeField] private float cameraRadius = 0.1f;
      [SerializeField] private LayerMask collisionLayers;
      [SerializeField] private float eyeHeight = 1.8f;
      [SerializeField] private float clipLerpSpeed = 15f;

      private CharacterController _cc;
      private MainInput _input;
      private Vector2 _moveInput;
      private Vector2 _lookInput;
      private bool _sprintingHeld;
      private const float Gravity = -9.81f;

      private Vector3 _currentVelocity;
      private Vector3 _verticalVelocity;
      private float _xRotation;
      private Vector2 _smoothMouseDelta, _mouseDeltaVel;
      private float _bobTimer;
      private Vector3 _originalCamLocalPos;

      public bool MoveActive { get => moveActive; set => moveActive = value; }

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
         _cc = GetComponent<CharacterController>();

         if (cameraHolder) _originalCamLocalPos = cameraHolder.localPosition;

         CameraActive = true;
         Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false;
      }

      private void OnEnable()
      {
         _input = InputManager.Input;
         
         _input.Player.Enable();
         _input.Player.Move.performed += Move;
         _input.Player.Move.canceled += Move;
         _input.Player.Look.performed += Look;
         _input.Player.Look.canceled += Look;
         _input.Player.Sprint.performed += Sprint;
         _input.Player.Sprint.canceled += Sprint;
         _input.Player.ToggleLook.performed += ToggleLook;
      }

      private void OnDisable()
      {
         _input.Player.Move.performed -= Move;
         _input.Player.Move.canceled -= Move;
         _input.Player.Look.performed -= Look;
         _input.Player.Look.canceled -= Look;
         _input.Player.Sprint.performed -= Sprint;
         _input.Player.Sprint.canceled -= Sprint;
         _input.Player.ToggleLook.performed -= ToggleLook;
         _input.Player.Disable();
      }

      private void Update()
      {
         if (!cameraHolder) return;
         if (!_cc || !_cc.enabled) return;

         HandleMouseLook();
         HandleHeadBob();
         HandleMovement();
         HandleCameraClipping();
         ApplyGravity();
      }

      private void Move(InputAction.CallbackContext ctx)
      {
         _moveInput = ctx.ReadValue<Vector2>();
         if (_moveInput.sqrMagnitude > 1f) _moveInput.Normalize();
      }

      private void Look(InputAction.CallbackContext ctx)
      {
         _lookInput = ctx.ReadValue<Vector2>() * mouseSensitivity / 10f;
      }

      private void Sprint(InputAction.CallbackContext ctx) { _sprintingHeld = ctx.ReadValue<float>() > 0.5f; }

      private void ToggleLook(InputAction.CallbackContext ctx) { CameraActive = !CameraActive; }

      private void HandleMouseLook()
      {
         if (!cameraActive) return;

         _smoothMouseDelta = Vector2.SmoothDamp(_smoothMouseDelta, _lookInput, ref _mouseDeltaVel, mouseSmoothTime);

         transform.Rotate(Vector3.up * _smoothMouseDelta.x);

         _xRotation = Mathf.Clamp(_xRotation - _smoothMouseDelta.y, -maxLookAngle, maxLookAngle);
         cameraHolder.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

         _lookInput = Vector2.zero;
      }

      private void HandleMovement()
      {
         if (!moveActive)
         {
            _currentVelocity = Vector3.zero;
            MoveController(Vector3.zero);
            return;
         }

         Vector3 inputDir = new Vector3(_moveInput.x, 0f, _moveInput.y);
         Vector3 worldDir = transform.TransformDirection(inputDir);

         float targetSpeed = _sprintingHeld ? sprintSpeed : walkSpeed;
         Vector3 targetVel = worldDir * targetSpeed;

         float accel = acceleration;
         float decel = deceleration;

         bool hasInput = worldDir.sqrMagnitude > 0.001f;

         if (hasInput) { _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVel, accel * Time.deltaTime); }
         else
         {
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, decel * Time.deltaTime);
            if (_currentVelocity.magnitude < stopThreshold) _currentVelocity = Vector3.zero;
         }

         MoveController(_currentVelocity + _verticalVelocity);
      }

      private void HandleHeadBob()
      {
         if (_moveInput == Vector2.zero) return;

         float freq = _sprintingHeld ? sprintBobFrequency : bobFrequency;
         float amp = _sprintingHeld ? sprintBobAmplitude : bobAmplitude;
         float sway = _sprintingHeld ? sprintBobSway : bobSway;

         _bobTimer += Time.deltaTime * freq;

         float y = Mathf.Sin(_bobTimer) * amp;
         float x = Mathf.Cos(_bobTimer * 0.5f) * sway;

         cameraHolder.localPosition = _originalCamLocalPos + new Vector3(x, y, 0f);
      }
      private void ApplyGravity()
      {
         if (_cc.isGrounded && _verticalVelocity.y < 0f)
            _verticalVelocity.y = -0.05f;

         _verticalVelocity.y += Gravity * Time.deltaTime;
      }


      private void HandleCameraClipping()
      {
         if (!cameraHolder) return;

         Vector3 headPos = transform.position + Vector3.up * eyeHeight;
         Vector3 desiredCamPos = cameraHolder.position;
         Vector3 dir = desiredCamPos - headPos;
         float dist = dir.magnitude;

         if (dist > 0.0001f && Physics.SphereCast(
                headPos,
                cameraRadius,
                dir.normalized,
                out RaycastHit hit,
                dist,
                collisionLayers
             ))
         {
            Vector3 safePos = hit.point - dir.normalized * cameraRadius;
            cameraHolder.position = Vector3.Lerp(cameraHolder.position, safePos, clipLerpSpeed * Time.deltaTime);
         }
         else
         {
            Vector3 target = headPos + dir;
            cameraHolder.position = Vector3.Lerp(cameraHolder.position, target, clipLerpSpeed * Time.deltaTime);
         }
      }

      private void MoveController(Vector3 velocity)
      {
         if (_cc.enabled) _cc.Move(velocity * Time.deltaTime);
      }
   }
}