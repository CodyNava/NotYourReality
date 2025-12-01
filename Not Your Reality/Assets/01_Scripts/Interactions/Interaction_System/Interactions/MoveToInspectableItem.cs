using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Interactions.Interaction_System.Interactions
{
    public class MoveToInspectableItem : InteractableBase
    {
        [Tooltip("The speed at which the Item goes into focus")]
        [SerializeField] private float duration = 1f;
        
        private Volume _volume;
        private Camera _camera;
        private Vector3 _originalTransform;
        private Quaternion _originalRotation;
        private Transform _anchorTransform;
        private Quaternion _anchorRotation;
        private bool _isInspecting;
        private Vignette _vignette;
        private Coroutine _inspect;

        private void Awake()
        {
            _camera = Camera.main;
            _anchorTransform = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.CompareTag("Inspection Anchor"));
            _anchorRotation = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.CompareTag("Inspection Anchor"))!.rotation;
            _volume = FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _vignette);
            TooltipMessage = "Press E to Inspect";
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            if (_inspect != null) StopCoroutine(_inspect);
            _inspect = StartCoroutine(!_isInspecting ? Inspect() : Release());
        }
        
        private IEnumerator Inspect()
        {
            InputManager.Input.Player.Disable();
            InputManager.Input.UI.Disable();
            InputManager.Input.Interaction.Disable();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _isInspecting = true;
            TooltipMessage = "";
            _originalTransform = _camera.transform.position;
            _originalRotation = _camera.transform.rotation;
            var t = 0f;
            _vignette.intensity.value = 0.2f;
            while (t < duration)
            {
                t += Time.deltaTime;
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _anchorTransform.position, t / duration);
                _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, _anchorRotation, t/duration);
                yield return null;
            }

        }

        private IEnumerator Release()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _isInspecting = false;
            TooltipMessage = "Press E to Inspect";
            _vignette.intensity.value = 0f;
            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _originalTransform, t/duration);
                _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, _originalRotation, t/duration);
                yield return null;
            }
            InputManager.Input.Player.Enable();
            InputManager.Input.UI.Enable();
            InputManager.Input.Interaction.Enable();
        }
    }
}