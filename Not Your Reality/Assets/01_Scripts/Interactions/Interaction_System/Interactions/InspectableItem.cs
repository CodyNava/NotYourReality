using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using FMODUnity;
using FMOD.Studio;
using Puzzle.Bedroom;

namespace Interactions.Interaction_System.Interactions
{
    public class InspectableItem : InteractableBase
    {
        [Header("Inspect Settings")]
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private float sensitivity = 3f;
        [SerializeField] private float clampMin = -80f;
        [SerializeField] private float clampMax = 80f;
        [SerializeField] private bool rotationClamping = true;

        [Header("Audio Lock")]
        [SerializeField] private bool lockPlayerUntilEmitterFinished;

        [Header("Vignette Settings")]
        [SerializeField] private bool useVignette = true;
        [SerializeField][Range(0f, 1f)] private float inspectVignetteIntensity = 0.25f;
        [SerializeField][Range(0f, 1f)] private float lockedVignetteIntensity = 0.4f;
        [SerializeField] private float vignetteLerpSpeed = 4f;

        [Header("References")]
        [SerializeField] private BedroomUnlock bedroomUnlock;
        [SerializeField] private int playerLayerMask = 9;

        [Header("FMOD")]
        [SerializeField] private StudioEventEmitter emitter;

        private float _horizontal;
        private float _vertical;

        private bool _isInspecting;
        private bool _audioFinished;

        private Volume _volume;
        private Vignette _vignette;
        private float _targetVignette;

        private Camera _cam;

        private Vector3 _startPos;
        private Quaternion _startRot;

        private Transform _pivot;
        private Transform _originalParent;
        private Vector3 _itemLocalPosInPivot;
        private Quaternion _baseRotation;

        private Coroutine _inspectRoutine;

        private void Awake()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitForSeconds(0.3f);

            _cam = Camera.main;
            _startPos = transform.position;
            _startRot = transform.rotation;

            _volume = FindFirstObjectByType<Volume>();

            if (_volume && _volume.profile.TryGet(out _vignette))
                _vignette.intensity.value = 0f;

            TooltipMessage = "Press E to Inspect";
        }

        public override void OnInteract()
        {
            base.OnInteract();

            if (_inspectRoutine != null)
                StopCoroutine(_inspectRoutine);

            _inspectRoutine = StartCoroutine(!_isInspecting ? Inspect() : TryRelease());

            if (emitter != null)
                emitter.Play();

            Physics.IgnoreLayerCollision(gameObject.layer, playerLayerMask, true);
        }

        private void Update()
        {
            RotateItem();

            if (_isInspecting && InputManager.Input.Inspection.OnInteract.WasPressedThisFrame())
            {
                StartCoroutine(TryRelease());
            }

            UpdateVignette();
        }

        private void RotateItem()
        {
            if (!_isInspecting) return;

            var rawMouse = InputManager.Input.Inspection.Look.ReadValue<Vector2>();
            rawMouse *= sensitivity;

            _horizontal += rawMouse.x;
            _vertical -= rawMouse.y;

            if (rotationClamping)
                _vertical = Mathf.Clamp(_vertical, clampMin, clampMax);

            if (_pivot)
            {
                var offset = Quaternion.Euler(0, _horizontal, -_vertical);
                _pivot.rotation = _baseRotation * offset;
            }
        }

        private IEnumerator Inspect()
        {
            TooltipMessage = "";
            _audioFinished = false;

            InputManager.Input.Player.Disable();
            InputManager.Input.Inspection.Enable();

            CreatePivot();

            Transform anchor = _cam.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.CompareTag("Inspection Anchor"));

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float a = t / duration;

                if (anchor)
                {
                    _pivot.position = Vector3.Lerp(_pivot.position, anchor.position, a);
                    _pivot.rotation = Quaternion.Lerp(_pivot.rotation, anchor.rotation, a);
                }

                yield return null;
            }

            _isInspecting = true;
            _baseRotation = _pivot.rotation;

            if (useVignette)
                _targetVignette = inspectVignetteIntensity;

            if (lockPlayerUntilEmitterFinished)
                StartCoroutine(WaitForEmitter());
        }

        private IEnumerator TryRelease()
        {
            if (lockPlayerUntilEmitterFinished && !_audioFinished)
                yield break;

            yield return StartCoroutine(Release());
        }

        private IEnumerator Release()
        {
            _isInspecting = false;
            TooltipMessage = "Press E to Inspect";

            InputManager.Input.Inspection.Disable();

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float a = t / duration;

                _pivot.position = Vector3.Lerp(_pivot.position,
                    _startPos - _startRot * _itemLocalPosInPivot, a);

                _pivot.rotation = Quaternion.Lerp(_pivot.rotation, _startRot, a);

                yield return null;
            }

            if (bedroomUnlock != null)
                bedroomUnlock.AddItem(this);

            DestroyPivot();

            InputManager.Input.Player.Enable();
            Physics.IgnoreLayerCollision(gameObject.layer, playerLayerMask, false);

            _targetVignette = 0f;
        }

        private IEnumerator WaitForEmitter()
        {
            if (emitter == null)
            {
                _audioFinished = true;
                yield break;
            }

            if (useVignette)
                _targetVignette = lockedVignetteIntensity;

            var instance = emitter.EventInstance;

            PLAYBACK_STATE state;
            instance.getPlaybackState(out state);

            while (state != PLAYBACK_STATE.STOPPED)
            {
                yield return null;
                instance.getPlaybackState(out state);
            }

            _audioFinished = true;

            if (useVignette)
                _targetVignette = inspectVignetteIntensity;
        }

        private void UpdateVignette()
        {
            if (!useVignette || _vignette == null) return;

            _vignette.intensity.value = Mathf.Lerp(
                _vignette.intensity.value,
                _targetVignette,
                Time.deltaTime * vignetteLerpSpeed
            );
        }

        private void CreatePivot()
        {
            if (_pivot) return;

            _originalParent = transform.parent;

            Bounds b = GetComponentsInChildren<Renderer>()[0].bounds;
            foreach (var r in GetComponentsInChildren<Renderer>())
                b.Encapsulate(r.bounds);

            _pivot = new GameObject($"{name}_InspectPivot").transform;
            _pivot.position = b.center;
            _pivot.rotation = transform.rotation;
            _pivot.SetParent(_originalParent, true);

            transform.SetParent(_pivot, true);
            _itemLocalPosInPivot = transform.localPosition;
        }

        private void DestroyPivot()
        {
            if (!_pivot) return;

            transform.SetParent(_originalParent, true);
            Destroy(_pivot.gameObject);
            _pivot = null;
        }
    }
}