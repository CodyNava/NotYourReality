using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using FMODUnity;
using Interactions.Interaction_System.Interactions.Door_Rework;

namespace Interactions.Interaction_System.Interactions
{
    public class InspectableItem : InteractableBase
    {
        [Tooltip("The speed at which the Item goes into focus")]
        [SerializeField] private float duration = 0.25f;

        [SerializeField] private float sensitivity;
        [SerializeField] private float clampMin;
        [SerializeField] private float clampMax;
        [SerializeField] private bool rotationClamping;
        [SerializeField] private BedroomUnlock bedroomUnlock;

        [Tooltip("The LayerMask used by the Player")]
        [SerializeField] private int playerLayerMask = 9;

        private float _horizontal;
        private float _vertical;
        private Volume _volume;
        private Camera _cam;
        private Transform _anchorTransform;
        private Quaternion _anchorRotation;
        private bool _isInspecting;
        private Vignette _vignette;
        private Vector3 _transform;
        private Quaternion _rotation;
        private Coroutine _inspect;

        private Quaternion _baseRotation;

        private Transform _pivot;
        private Transform _originalParent;
        private Vector3 _itemLocalPosInPivot;

        [Header("FMOD")]
        [SerializeField] private StudioEventEmitter emitter;

        [SerializeField] private RoomVoiceManager manager; 

        private void Awake()
        {
            StartCoroutine(WaitForLoadingCoreScripts());
        }

        private void OnEnable()
        {
            StartCoroutine(WaitForLoadingCoreScripts());
        }

        private IEnumerator WaitForLoadingCoreScripts()
        {
            yield return new WaitForSeconds(0.5f);
            if (InputManager.Input.Inspection.enabled)
            {
                InputManager.Input.Inspection.Disable();
            }
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

            if (emitter != null)
                emitter.Play(); 

            if (manager != null)
                manager.OnVoiceTriggered(gameObject); 

            Physics.IgnoreLayerCollision(gameObject.layer, playerLayerMask, true);
        }

        private void Update()
        {
            RotateItem();
            if (_isInspecting && InputManager.Input.Inspection.OnInteract.WasPressedThisFrame())
            {
                StartCoroutine(Release());
            }
        }

        private void RotateItem()
        {
            if (!_isInspecting) return;

            var rawMouse = InputManager.Input.Inspection.Look.ReadValue<Vector2>();
            rawMouse *= sensitivity;
            _horizontal += rawMouse.x;
            _vertical -= rawMouse.y;
            if (rotationClamping) _vertical = Mathf.Clamp(_vertical, clampMin, clampMax);

            if (_pivot)
            {
                var offset = Quaternion.Euler(0, _horizontal, -_vertical);
                _pivot.rotation = _baseRotation * offset;
            }
        }

        private IEnumerator Inspect()
        {
            TooltipMessage = "";
            _anchorTransform = _cam.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.CompareTag("Inspection Anchor"));
            _anchorRotation = _cam.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.CompareTag("Inspection Anchor"))!.rotation;
            InputManager.Input.Player.Disable();

            CreatePivotAtBoundsCenter();

            _vignette.intensity.value = 0.2f;
            var t = 0f;
            while (t < duration * 0.2f)
            {
                t += Time.deltaTime;
                float a = t / duration;
                if (_anchorTransform)
                    _pivot.position = Vector3.Lerp(_pivot.position, _anchorTransform.position, a);
                if (_pivot)
                    _pivot.rotation = Quaternion.Lerp(_pivot.rotation, _anchorRotation, a);
                yield return null;
            }

            _isInspecting = true;
            InputManager.Input.Inspection.Enable();
            _baseRotation = _pivot ? _pivot.rotation : transform.rotation;
            _horizontal = 0;
            _vertical = 0;
        }


        private IEnumerator Release()
        {
            _isInspecting = false;
            TooltipMessage = "Press E to Inspect";
            InputManager.Input.Player.Enable();
            InputManager.Input.Inspection.Disable();
            _vignette.intensity.value = 0f;

            if (_pivot)
            {
                var targetPivotRot = _rotation;
                var targetPivotPos = _transform - targetPivotRot * _itemLocalPosInPivot;
                var t = 0f;
                while (t < duration)
                {
                    t += Time.deltaTime;
                    float a = t / duration;
                    _pivot.position = Vector3.Lerp(_pivot.position, targetPivotPos, a);
                    _pivot.rotation = Quaternion.Lerp(_pivot.rotation, targetPivotRot, a);
                    yield return null;
                }

                if (bedroomUnlock != null)
                {
                    bedroomUnlock.AddItem(this);
                }
                DestroyPivot();
            }
            Physics.IgnoreLayerCollision(gameObject.layer, playerLayerMask, false);
        }

        private void CreatePivotAtBoundsCenter()
        {
            if (_pivot) return;

            _originalParent = transform.parent;

            Vector3 center = transform.position;
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers != null && renderers.Length > 0)
            {
                Bounds b = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    b.Encapsulate(renderers[i].bounds);
                center = b.center;
            }

            var go = new GameObject($"{name}_InspectPivot");
            _pivot = go.transform;

            _pivot.position = center;
            _pivot.rotation = transform.rotation;

            _pivot.SetParent(_originalParent, true);

            transform.SetParent(_pivot, true);

            _itemLocalPosInPivot = transform.localPosition;

            _baseRotation = _pivot.rotation;
        }

        private void DestroyPivot()
        {
            if (!_pivot) return;

            transform.SetParent(_originalParent, true);

            var pivotGo = _pivot.gameObject;
            _pivot = null;

            Destroy(pivotGo);
        }
    }
}
