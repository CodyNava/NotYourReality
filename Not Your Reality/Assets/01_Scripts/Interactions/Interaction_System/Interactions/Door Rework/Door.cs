using FMODUnity;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions.Door_Rework
{
    public class Door : MonoBehaviour
    {
        [Header("Door Physics")] [SerializeField]
        private float pullStrength = 3f;

        [SerializeField] private float drag = 2f;
        [SerializeField] private float maxVelocity = 5f;

        [Header("Lock Mechanism")] [SerializeField]
        private float lockThreshold = 0.3f;

        [SerializeField] private float lockAngle = 5f;

        [Header("Audio")] [SerializeField] private EventReference unlockSound;
        [SerializeField] private EventReference voiceLine;

        private Rigidbody _rb;

        private Quaternion _lockRotation;
        private float _input;
        private bool _isHeld;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _lockRotation = transform.rotation;
        }

        public void BeginInteraction()
        {
            _isHeld = true;
        }

        public void EndInteraction()
        {
            _isHeld = false;
            _input = 0f;
        }

        public void ReceiveInput(float input)
        {
            if (!_isHeld) return;
            _input = input;
        }

        private void FixedUpdate()
        {
            if (_isHeld)
            {
                ApplyDoorTorque();
            }
            else
            {
                LockDoor();
            }

            ClampVelocity();
            _input = 0f;
        }

        private void ApplyDoorTorque()
        {
            var torque = Vector3.up * (_input * pullStrength);
            torque -= _rb.angularVelocity * drag;

            _rb.AddTorque(torque, ForceMode.Acceleration);
        }

        private void ClampVelocity()
        {
            if (_rb.angularVelocity.magnitude > maxVelocity)
            {
                _rb.angularVelocity = _rb.angularVelocity.normalized * maxVelocity;
            }
        }
        private void LockDoor()
        {
            var hingeVelocity = Vector3.Project(_rb.angularVelocity, Vector3.up).magnitude;
            
            var angle = Quaternion.Angle(_rb.rotation, _lockRotation);

            if (hingeVelocity < lockThreshold && angle < lockAngle)
            {
                _rb.angularVelocity = Vector3.zero;
                _rb.rotation = _lockRotation;
            }
            else
            {
            }
        }

        public void UnlockDoor()
        {
            RuntimeManager.PlayOneShot(unlockSound, transform.position);
            RuntimeManager.PlayOneShot(voiceLine, transform.position);
        }
    }
}