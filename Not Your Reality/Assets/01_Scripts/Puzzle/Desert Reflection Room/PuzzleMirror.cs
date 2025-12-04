using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Puzzle.Desert_Reflection_Room
{
    public class PuzzleMirror : InteractableBase
    {
        [Header("Mirror Settings")]
        [Tooltip("How sensitive the mirror reacts to the mouse movement")]
        [SerializeField] private float sensitivity = 4f;

        [Tooltip("The speed at which the mirror rotates")]
        [SerializeField] private float rotationSpeed = 360f;

        [Tooltip("<b>On</b>: Mirror rotates Up and Down \n" + "<b>Off</b>: Mirror rotates Left and Right")]
        [SerializeField] private bool zRotation;

        private bool _isHeld;
        private Rigidbody _rigidbody;

        private float _initialZRotation;
        private float _initialYRotation;
        private float _accumulatedAngle;

        private void Start()
        {
            TooltipMessage = "Hold E to Rotate";
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }

        public override void OnInteract()
        {
            base.OnInteract();
            _initialYRotation = transform.eulerAngles.y;
            _initialZRotation = transform.eulerAngles.z;
            _accumulatedAngle = 0f;
            _rigidbody.constraints = zRotation switch
            {
                true => RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationY,
                false => RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ
            };
            _isHeld = true;
        }

        private void FixedUpdate()
        {
            if (!_isHeld) return;
            RotateMirror();
        }

        private void RotateMirror()
        {
            _rigidbody.isKinematic = false;
            switch (zRotation)
            {
                case true:
                { 
                    var rawInput = InputManager.Input.Player.Look.ReadValue<Vector2>().y;
                    var delta = rawInput * sensitivity * Time.fixedDeltaTime;
                    _accumulatedAngle += delta;
                    var targetAngle = _initialZRotation + _accumulatedAngle;
                    var targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, targetAngle);
                    
                    var rotationStep = rotationSpeed * Time.fixedDeltaTime;
                    
                    var newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
                    _rigidbody.MoveRotation(newRotation);
                    break;
                }
                case false:
                {
                    var rawInput = InputManager.Input.Player.Look.ReadValue<Vector2>().x;
                    var delta = rawInput * sensitivity * Time.fixedDeltaTime;
                    _accumulatedAngle += delta;
                    var targetAngle = _initialYRotation + _accumulatedAngle;
                    var targetRotation = Quaternion.Euler(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
                    
                    var rotationStep = rotationSpeed * Time.fixedDeltaTime;
                    
                    var newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
                    _rigidbody.MoveRotation(newRotation);
                    break;
                }
            }
        }

        public void Release()
        {
            _isHeld = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            _rigidbody.isKinematic = true;
        }
    }
}