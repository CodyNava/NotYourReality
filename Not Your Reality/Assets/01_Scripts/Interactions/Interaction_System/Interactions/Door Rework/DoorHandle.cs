using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactions.Interaction_System.Interactions.Door_Rework
{
    public class DoorHandle : InteractableBase
    {
        [Header("Door Reference")]
        [SerializeField] private Door door;

        [SerializeField] private HingeJoint hinge;
        
        private bool _isHeld;
        private float _directionMultiplier;
        private GameObject _player;
        

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        public override void OnInteract()
        {
            base.OnInteract();
            _isHeld = true;
            door.BeginInteraction();
        }

        private void CalculateDirectionMultiplier()
        {
            var hingePos = door.transform.TransformPoint(hinge.anchor);
            var hingeAxis = door.transform.TransformDirection(hinge.axis);
            
            var toPlayer = _player.transform.position - hingePos;
            toPlayer -= Vector3.Dot(toPlayer, hingeAxis) * hingeAxis;
            toPlayer.Normalize();
            
            var doorForward = door.transform.forward;
            doorForward -= Vector3.Dot(doorForward, hingeAxis) * hingeAxis;
            doorForward.Normalize();
            
            var side = Vector3.Dot(Vector3.Cross(doorForward, toPlayer), hingeAxis);
            _directionMultiplier = side >= 0f ? 1f : -1f;
        }

        public void Release()
        {
            _isHeld = false;
            door.EndInteraction();
        }

        private void Update()
        {
            if (!IsInteractable)
            {
                TooltipMessage = "";
                return;
            }

            TooltipMessage = "Hold E to Interact";
            if (!_isHeld) return;

            CalculateDirectionMultiplier();
            var mouseDeltaX = Mouse.current.delta.ReadValue().x;
            var signedInput = mouseDeltaX * _directionMultiplier;

            door.ReceiveInput(signedInput);
        }

        private void OnDisable()
        {
            if (_isHeld)
            {
                _isHeld = false;
                door.EndInteraction();
            }
        }
    }
}