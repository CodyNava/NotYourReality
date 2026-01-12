using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactions.Interaction_System.Interactions.Door_Rework
{
    public class DoorHandle : InteractableBase
    {
        [Header("Door Reference")] [SerializeField]
        private Door door;

        [Header("Handle configurations")]
        [Tooltip("Is the handle on the side of the hinge or not")]
        [SerializeField] private bool isHingeSide;

        private bool _isHeld;
        private float DirectionMultiplier => isHingeSide ? -1 : 1;

        public override void OnInteract()
        {
            base.OnInteract();
            _isHeld = true;
            door.BeginInteraction();
        }

        public void Release()
        {
            _isHeld = false;
            door.EndInteraction();
        }

        private void Update()
        {
            if (!_isHeld) return;

            var mouseDeltaX = Mouse.current.delta.ReadValue().x;
            var signedInput = mouseDeltaX * DirectionMultiplier;

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