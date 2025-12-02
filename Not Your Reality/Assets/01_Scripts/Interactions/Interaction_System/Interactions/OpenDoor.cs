using System.Collections;
using FMODUnity;
using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
    public class OpenDoor : InteractableBase
    {
        [Space]
        [Tooltip("The drag of the door")]
        [SerializeField] private float drag = 5;

        [Tooltip("The speed at which the door follows the mouse")]
        [SerializeField] private float pullStrength = 8;

        [Header("Close and Lock Mechanism")]
        [Tooltip("The maximum velocity the door is allowed to have to close and stay shut if the angle is low enough")]
        [SerializeField] private float lockThreshold = 0.3f;

        [Tooltip("The angle at which the door closes and stays shut if the velocity is low enough")]
        [SerializeField] private float lockAngle = 5;

        [Tooltip("The handle of the door")]
        [SerializeField] private Transform handleTransform;

        [Header("Audio Reference")]
        [Tooltip("The sound that is played when you unlock a door")]
        [SerializeField] private EventReference unlockSound;

        private bool _isHeld;
        private Rigidbody _rb;
        private Quaternion _lockRotation;
        private Vector3 _torque;
        private HingeJoint _joint;
        private Camera _camera;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _joint = GetComponent<HingeJoint>();
            _lockRotation = _rb.transform.rotation;
            TooltipMessage = "Hold E to Interact";
        }

        private IEnumerator Start()
        {
            while (!_camera)
            {
                _camera = Camera.main;
                yield return null;
            }
        }

        public override void OnInteract()
        {
            base.OnInteract();
            _isHeld = true;
        }


        // Optional Edit if you want the text to change based on status
        private void Update()
        {
            TooltipMessage = IsInteractable ? "Hold E to Interact" : "";
        }

        private void FixedUpdate()
        {
            if (!IsInteractable)
            {
                _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                  RigidbodyConstraints.FreezeRotationZ;
            }
            else
            {
                _rb.constraints = RigidbodyConstraints.None;
            }

            if (!_isHeld)
            {
                LockDoor();
                return;
            }

            RotateDoor();
        }

        private void RotateDoor()
        {
            var hingeToHandle = (handleTransform != null)
                ? (handleTransform.position - transform.position).normalized
                : transform.forward;
            hingeToHandle.y = 0;
            hingeToHandle.Normalize();

            var playerToHinge = transform.position - _camera.transform.position;
            playerToHinge.y = 0;
            playerToHinge.Normalize();

            var dot = Vector3.Dot(playerToHinge, hingeToHandle);
            var side = (Mathf.Abs(dot) < 0.0001f) ? 1f : Mathf.Sign(dot);
            var lookInput = InputManager.Input.Player.Look.ReadValue<Vector2>();
            var lookX = lookInput.x;
            var moveAngle = lookX * side;

            _torque = Vector3.up * (moveAngle * pullStrength);
            _torque -= _rb.angularVelocity * drag;
            const float maxTorque = 10000f;
            if (_torque.sqrMagnitude > maxTorque * maxTorque) _torque = _torque.normalized * maxTorque;

            _rb.AddTorque(_torque, ForceMode.Acceleration);
        }

        private void LockDoor()
        {
            var hingeVelocity = Vector3.Project(_rb.angularVelocity, Vector3.up).magnitude;
            var angle = Quaternion.Angle(_rb.rotation, _lockRotation);

            if (hingeVelocity < lockThreshold && angle < lockAngle)
            {
                _joint.useLimits = false;
                _rb.angularVelocity = Vector3.zero;
                _rb.rotation = _lockRotation;
            }
            else
            {
                _joint.useLimits = true;
            }
        }

        public void UnlockDoor()
        {
            IsInteractable = true;
            RuntimeManager.PlayOneShot(unlockSound, transform.position);
        }

        public void Release()
        {
            _isHeld = false;
        }
    }
}