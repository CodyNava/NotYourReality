using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Puzzle.Wordle
{
    public class ItemLetterInteract : InteractableBase
    {
        [Tooltip("The speed at which the Item goes into focus")]
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private float sensitivity;
        [SerializeField] private float clampMin;
        [SerializeField] private float clampMax;
        [SerializeField] private bool rotationClamping;
        
        [SerializeField] private WordlePuzzle wordlePuzzle;

        public char LetterChar
        {
            private get => _letterChar;
            set => _letterChar = value;
        }

        private float _horizontal;
        private float _vertical;
        private char _letterChar = ' ';
        private bool _isInspecting;
        private Transform _anchorTransform;
        private Transform _letterTransform;
        private Transform _pivot;
        private Transform _originalParent;
        private Quaternion _anchorRotation;
        private Quaternion _rotation;
        private Quaternion _baseRotation;
        private Vector3 _transform;
        private Vector3 _itemLocalPosInPivot;
        private Volume _volume;
        private Camera _cam;
        private Coroutine _inspect;
        private Vignette _vignette;
        private TextMeshPro _letter;
        


        private void Awake()
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
            _letterTransform = transform.GetChild(0);
            _letter = GetComponentInChildren<TextMeshPro>();
            _volume = FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _vignette);
            TooltipMessage = "Press E to Inspect";
            _letter.text = LetterChar.ToString();
        }

        public override void OnInteract()
        {
            base.OnInteract();
            if (_inspect != null) StopCoroutine(_inspect);
            _inspect = StartCoroutine(!_isInspecting ? Inspect() : Release());
            Debug.Log(_isInspecting);
        }

        private void Update()
        {
            if (!_cam) return;
            RotateItem();
            
            
            if (FacingLetter() && _isInspecting && _letterChar != ' ')
            {
                wordlePuzzle.KeyEnable(_letterChar);
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

        private bool FacingLetter()
        {
            var toCamera = (_cam.transform.position - _letterTransform.position).normalized;
            return Vector3.Dot(transform.forward, toCamera) > 0.3f;
        }

        private IEnumerator Inspect()
        {
            _isInspecting = true;
            TooltipMessage = "";
            _anchorTransform = _cam.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.CompareTag("Inspection Anchor"));
            _anchorRotation = _cam.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.CompareTag("Inspection Anchor"))!.rotation;
            InputManager.Input.Player.Disable();
            
            CreatePivotAtBoundsCenter();
            
            var t = 0f;
            _vignette.intensity.value = 0.2f;
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

                DestroyPivot();
            }
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