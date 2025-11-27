using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Puzzle.TVFrequencyMatch
{
    public class TVManager : MonoBehaviour
    {
        private TVWordManager _tvWordManager;
        private TVFrequencyMatch _tvFrequencyMatch;

        [SerializeField] private List<GameObject> tvLetters;
        private readonly List<GameObject> _actualLetters = new();

        private string _original;
        private string _scrambled;

        private int _letterA;
        private int _letterB = 1;

        public bool Completed { get; private set; }

        private void Awake()
        {
            _tvFrequencyMatch = GetComponentInParent<TVFrequencyMatch>();
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
            _letterA++;
            _letterB++;
            DisplaySelection();
        }

        public void SwapLetters()
        {
            var chars = _scrambled.ToCharArray();
            (chars[_letterB], chars[_letterA]) = (chars[_letterA], chars[_letterB]);
            _scrambled = new string(chars);
            DisplayLetters();
            CheckWin();
        }

        private void CheckWin()
        {
            if (_scrambled != _original) return;
            Completed = true;
            _tvFrequencyMatch.Status();
        }
    }
}