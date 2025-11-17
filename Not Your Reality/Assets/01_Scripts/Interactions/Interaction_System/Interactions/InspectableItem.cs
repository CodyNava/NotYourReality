using Interactions.Interaction_System.Interaction_Base_Class;
using Player.PlayerMovement.Movement;
using UnityEditor;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
    public class InspectableItem : InteractableBase
    {
        private PlayerController _playerController;
        private bool _isInspecting;

        private void Start()
        {
            _playerController = FindFirstObjectByType<PlayerController>();
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            if (!_isInspecting)
            {
                Inspect();
            }
            else
            {
                Release();
            }
        }
        
        private void Inspect()
        {
            _isInspecting = true;
            _playerController.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            //TODO: Blur Background/Add Vignette, Display Object in Center, Possibly Add Object Rotation (Via Click and Drag or 'Q' and 'E'
        }

        private void Release()
        {
            _isInspecting = false;
            _playerController.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}