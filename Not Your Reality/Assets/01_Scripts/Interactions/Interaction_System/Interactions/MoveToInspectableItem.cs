using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using Player.PlayerMovement.Movement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Interactions.Interaction_System.Interactions
{
    public class MoveToInspectableItem : InteractableBase
    {
        [Tooltip("The speed at which the Item goes into focus")]
        [SerializeField] private float duration = 1f;
        
        [Tooltip("The camera for this inspectable item")]
        [SerializeField] private GameObject inspectCamera;
        
        private PlayerController _playerController;

        private Canvas _crosshairCanvas;
        
        private Volume _volume;
        private bool _isInspecting;
        private Vignette _vignette;
        private Coroutine _inspect;
        
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            _volume = FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _vignette);
            TooltipMessage = "Press E to Inspect";
        }

        private void Start()
        {
            _playerController = FindFirstObjectByType<PlayerController>();
            var crosshair = GameObject.FindWithTag("Crosshair");
            _crosshairCanvas = crosshair.GetComponentInChildren<Canvas>();
        }

        public override void OnInteract()
        {
            base.OnInteract();
            if (_isInspecting)
            {
                Release();
            }
            else
            {
                Inspect();
            }
        }
        
        private void Inspect()
        {
            canvasGroup.blocksRaycasts = true;
            InputManager.Input.Player.Disable();
            InputManager.Input.UI.Disable();
            _crosshairCanvas.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _isInspecting = true;
            TooltipMessage = "";
            _vignette.intensity.value = 0.2f;

            _playerController.CameraActive = false;
            inspectCamera.SetActive(true);
        }

        private void Release()
        {
            canvasGroup.blocksRaycasts = false;
            inspectCamera.SetActive(false);
            _playerController.CameraActive = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _crosshairCanvas.enabled = true;
            _isInspecting = false;
            TooltipMessage = "Press E to Inspect";
            _vignette.intensity.value = 0f;
            InputManager.Input.Player.Enable();
            InputManager.Input.UI.Enable();
        }
    }
}