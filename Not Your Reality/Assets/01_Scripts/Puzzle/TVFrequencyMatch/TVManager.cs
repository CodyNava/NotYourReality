using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.TVFrequencyMatch
{
    public class TVManager : MonoBehaviour
    {
        private TVWordManager _tvWordManager;
        [SerializeField] private TVFrequencyMatch tvFrequencyMatch;

        [SerializeField] private List<GameObject> tvLetters;
        private readonly List<GameObject> _actualLetters = new();

        [SerializeField] private List<Button> buttons;

        [SerializeField] private GameObject leftButton;
        [SerializeField] private GameObject rightButton;
        [SerializeField] private GameObject confirmButton;

        [SerializeField] private Vector3 rotationVector;

        [SerializeField] private EventReference leftSound;
        [SerializeField] private EventReference rightSound;
        [SerializeField] private EventReference confirmSound;

        private string _original;
        private string _scrambled;

        private int _letterA;
        private int _letterB = 1;

        private float _leftRotationX;
        private float _rightRotationX;

        public bool Completed { get; private set; }

        private void Awake()
        {
            _tvWordManager = FindFirstObjectByType<TVWordManager>();
        }

        private void Start()
        {
            _tvWordManager.SelectPair();
            _original = _tvWordManager.OriginalWord;
            _scrambled = _tvWordManager.ScrambledWord;
            SetUp();
        }

        private void SetUp()
        {
            for (var i = 0; i <= _scrambled.Length - 1; i++)
            {
                tvLetters[i].gameObject.SetActive(true);
            }

            _actualLetters.Clear();
            foreach (Transform child in transform)
            {
                if (child.CompareTag("TV Letter") && child.gameObject.activeSelf)
                {
                    _actualLetters.Add(child.gameObject);
                }
            }

            DisplayLetters();
        }

        private void DisplayLetters()
        {
            for (var i = 0; i <= _scrambled.Length - 1; i++)
            {
                var letter = _actualLetters[i].GetComponent<TextMeshProUGUI>();
                letter.text = _scrambled[i].ToString();
            }

            DisplaySelection();
        }

        private void DisplaySelection()
        {
            foreach (var letter in _actualLetters)
            {
                letter.GetComponent<TextMeshProUGUI>().color = Color.white;
            }

            _actualLetters[_letterA].GetComponent<TextMeshProUGUI>().color = Color.red;
            _actualLetters[_letterB].GetComponent<TextMeshProUGUI>().color = Color.red;
        }

        public void MoveSelectionLeft()
        {
            if (_letterA == 0)
            {
                return;
            }

            _leftRotationX -= 5f;
            leftButton.transform.localRotation =
                Quaternion.Euler(_leftRotationX, -90f, 90f);
            if (!leftSound.IsNull)
            {
                RuntimeManager.PlayOneShot(leftSound, leftButton.gameObject.transform.position);
            }
            _letterA--;
            _letterB--;
            DisplaySelection();
        }

        public void MoveSelectionRight()
        {
            if (_letterB == _scrambled.Length - 1)
            {
                return;
            }

            _rightRotationX += 5f;
            rightButton.transform.localRotation =
                Quaternion.Euler(_rightRotationX, -90f, 90f);
            
            if (!rightSound.IsNull)
            {
                RuntimeManager.PlayOneShot(rightSound, rightButton.gameObject.transform.position);
            }
            _letterA++;
            _letterB++;
            DisplaySelection();
        }

        public void SwapLetters()
        {
            StartCoroutine(PushButton());
        }

        private IEnumerator PushButton()
        {
            foreach (var button in buttons)
            {
                button.interactable = false;
            }
            var initialVector = confirmButton.transform.localPosition;
            var pressedVector = new Vector3(initialVector.x, initialVector.y, 0.2f);
            var duration = 0.1f;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                confirmButton.transform.localPosition = Vector3.Lerp(initialVector, pressedVector, elapsed / duration);
                yield return null;
            }
            
            confirmButton.transform.localPosition = pressedVector;
            
            var chars = _scrambled.ToCharArray();
            (chars[_letterB], chars[_letterA]) = (chars[_letterA], chars[_letterB]);
            _scrambled = new string(chars);
            if (!confirmSound.IsNull)
            {
                RuntimeManager.PlayOneShot(confirmSound, confirmButton.gameObject.transform.position);
            }
            DisplayLetters();
            CheckWin();
            
            yield return new WaitForSeconds(0.5f);
            
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                confirmButton.transform.localPosition = Vector3.Lerp(pressedVector, initialVector, elapsed / duration);
                yield return null;
            }
            
            confirmButton.transform.localPosition = initialVector;
            
            foreach (var button in buttons)
            {
                button.interactable = true;
            }
        }

        private void CheckWin()
        {
            if (_scrambled != _original) return;
            foreach (var button in buttons)
            {
                button.interactable = false;
            }

            foreach (var letter in _actualLetters)
            {
                letter.GetComponent<TextMeshProUGUI>().color = Color.green;
            }

            Completed = true;
            tvFrequencyMatch.Status();
        }
    }
}