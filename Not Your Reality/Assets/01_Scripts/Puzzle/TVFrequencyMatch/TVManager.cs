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

            for (var i = 0; i <= _scrambled.Length - 1; i++)
            {
                var letter = _actualLetters[i].GetComponent<TextMeshProUGUI>();
                letter.text = _scrambled[i].ToString();
            }
        }

        public void LetterSelection()
        {
            //TODO: Change the selected Letters
        }
        
        public void SwapLetters()
        {
            //TODO: Swap Selected Letters
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