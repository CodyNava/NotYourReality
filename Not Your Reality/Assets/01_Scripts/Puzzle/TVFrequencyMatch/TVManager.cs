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
                Debug.Log("Left End Reached");
                return;
            }
            leftButton.gameObject.transform.Rotate(-rotationVector, Space.Self);
            RuntimeManager.PlayOneShot(leftSound, leftButton.gameObject.transform.position);
            _letterA--;
            _letterB--;
            DisplaySelection();
        }

        public void MoveSelectionRight()
        {
            if (_letterB == _scrambled.Length - 1)
            {
                Debug.Log("Right End Reached");
                return;
            }
            rightButton.gameObject.transform.Rotate(rotationVector, Space.Self);
            RuntimeManager.PlayOneShot(rightSound, rightButton.gameObject.transform.position);
            _letterA++;
            _letterB++;
            DisplaySelection();
        }

        public void SwapLetters()
        {
            var chars = _scrambled.ToCharArray();
            (chars[_letterB], chars[_letterA]) = (chars[_letterA], chars[_letterB]);
            _scrambled = new string(chars);
            RuntimeManager.PlayOneShot(confirmSound, confirmButton.gameObject.transform.position);
            DisplayLetters();
            CheckWin();
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