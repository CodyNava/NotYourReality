using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Interactions.Interaction_System.Interactions
{
    public class InspectableItem : InteractableBase
    {
        [Tooltip("The speed at which the Item goes into focus")]
        [SerializeField] private float duration = 1.5f;
        
        private Volume _volume;
        private Camera _cam;
        private Transform _anchorTransform;
        private Quaternion _anchorRotation;
        private bool _isInspecting;
        private Vignette _vignette;
        private Vector3 _transform;
        private Quaternion _rotation;
        private Coroutine _inspect;

        private void Awake()
        {
            _cam = Camera.main;
            _transform = transform.position;
            _rotation = transform.rotation;
            _volume = FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _vignette);
            TooltipMessage = "Press E to Inspect";
        }
        
        public override void OnInteract()
        {
            base.OnInteract();
            if (_inspect != null) StopCoroutine(_inspect);
            _inspect = StartCoroutine(!_isInspecting ? Inspect() : Release());
            Debug.Log(_isInspecting);
        }
        
        private IEnumerator Inspect()
        {
            _isInspecting = true;
            TooltipMessage = "";
            _anchorTransform = _cam.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.CompareTag("Inspection Anchor"));
            _anchorRotation = _cam.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.CompareTag("Inspection Anchor"))!.rotation;
            InputManager.Input.Player.Disable();
            var t = 0f;
            _vignette.intensity.value = 0.2f;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (_anchorTransform != null)
                    transform.position = Vector3.Lerp(transform.position, _anchorTransform.position, t / duration);
                transform.rotation = Quaternion.Lerp(transform.rotation, _anchorRotation, t/duration);
                yield return null;
            }
        }

        private IEnumerator Release()
        {
            _isInspecting = false;
            TooltipMessage = "Press E to Inspect";
            InputManager.Input.Player.Enable();
            _vignette.intensity.value = 0f;
            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, _transform, t/duration);
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, t/duration);
                yield return null;
            }
        }
    }
}