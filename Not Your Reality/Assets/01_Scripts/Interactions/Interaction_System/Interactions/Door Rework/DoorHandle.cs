using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

namespace Interactions.Interaction_System.Interactions.Door_Rework
{
    public class DoorHandle : InteractableBase
    {
        [Header("Door Reference")] [SerializeField]
        private Door door;

        private bool _isHeld;

        private float _directionMultiplier;

        private GameObject _player;
        private Vector3 _doorPosition;
        private Vector3 _doorForward;


        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _doorPosition = door.transform.position;
            _doorForward = door.transform.forward;
        }

        public override void OnInteract()
        {
            base.OnInteract();
            _isHeld = true;
            door.BeginInteraction();
        }

        private void CalculateDirectionMultiplier()
        {
            var playerPosition = _player.transform.position;

            var playerToDoor = playerPosition - _doorPosition;
            var playerToDoorDot = Vector3.Dot(_doorForward, playerToDoor);

            _directionMultiplier = playerToDoorDot >= 0f ? -1f : 1f;
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