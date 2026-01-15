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
        [SerializeField] private float duration = 1.5f;

        [SerializeField] private bool mouseToggle;
        
        [SerializeField] private WordlePuzzle wordlePuzzle;

        public char LetterChar
        {
            private get => _letterChar;
            set => _letterChar = value;
        }

        private char _letterChar;
        private Volume _volume;
        private Camera _cam;
        private Transform _anchorTransform;
        private Quaternion _anchorRotation;
        private bool _isInspecting;
        private Vignette _vignette;
        private Vector3 _transform;
        private Quaternion _rotation;
        private Coroutine _inspect;
        private Transform _letterTransform;
        private TextMeshPro _letter;

        private void Awake()
        {
            _cam = Camera.main;
            _transform = transform.position;
            _rotation = transform.rotation;
            _letterTransform = transform.GetChild(0);
            _letter = GetComponentInChildren<TextMeshPro>();
            _volume = FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _vignette);
            TooltipMessage = "Press E to Inspect";
        }

        private void Start()
        {
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
            if (!_isInspecting) return;
            switch (mouseToggle)
            {
                case true:
                    if (InputManager.Input.UI.Click.IsPressed())
                    {
                        var rawMouse = InputManager.Input.Inspection.Look.ReadValue<Vector2>() * 0.05f;
                        gameObject.transform.Rotate(0, -rawMouse.x, rawMouse.y);
                    }

                    break;
                case false:
                    var rawMouse2 = InputManager.Input.Inspection.Look.ReadValue<Vector2>() * 0.05f;
                    gameObject.transform.Rotate(0, -rawMouse2.x, rawMouse2.y);
                    break;
            }

            if (FacingLetter() && _isInspecting && _letterChar != ' ')
            {
                wordlePuzzle.KeyEnable(_letterChar);
            }
        }

        private bool FacingLetter()
        {
            var toCamera = (_cam.transform.position - _letterTransform.position).normalized;
            return Vector3.Dot(transform.forward, toCamera) > 0.5f;
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
            InputManager.Input.Inspection.Enable();
            var t = 0f;
            _vignette.intensity.value = 0.2f;
            while (t < duration)
            {
                t += Time.deltaTime;
                if (_anchorTransform != null)
                    transform.position = Vector3.Lerp(transform.position, _anchorTransform.position, t / duration);
                transform.rotation = Quaternion.Lerp(transform.rotation, _anchorRotation, t / duration);
                yield return null;
            }
        }

        private IEnumerator Release()
        {
            _isInspecting = false;
            TooltipMessage = "Press E to Inspect";
            InputManager.Input.Player.Enable();
            InputManager.Input.Inspection.Disable();
            _vignette.intensity.value = 0f;
            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, _transform, t / duration);
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, t / duration);
                yield return null;
            }
        }
    }
}