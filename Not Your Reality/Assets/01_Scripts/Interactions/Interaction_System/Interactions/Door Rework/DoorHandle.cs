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
        private Vector3 _playerPosition;
        private Vector3 _playerForward;
        private Vector3 _handleForward;
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
            _playerPosition = _player.transform.position;
            _playerForward = _player.transform.forward;
            _handleForward = transform.forward;

            var side = Vector3.Dot(_handleForward, _playerForward);

            var playerToDoor = _playerPosition - _doorPosition;
            var playerToDoorDot = Vector3.Dot(_doorForward, playerToDoor);

            switch (side)
            {
                case > 0 when playerToDoorDot > 0://Player on Door-forward looking along handle forward
                case < 0 when playerToDoorDot > 0://Player on Door-forward looking away from door handle forward
                    _directionMultiplier = -1f;
                    break;
                case < 0 when playerToDoorDot < 0://Player behind Door-forward looking away from door handle forward
                case > 0 when playerToDoorDot < 0://Player behind Door-forward looking along handle forward
                    _directionMultiplier = 1f;
                    break;
                
            }
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