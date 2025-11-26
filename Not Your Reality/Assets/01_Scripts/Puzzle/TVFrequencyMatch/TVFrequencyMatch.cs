using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Puzzle.TVFrequencyMatch
{
    public class TVFrequencyMatch : MonoBehaviour
    {
        private TVWordManager _tvWordManager;
        [SerializeField] private List<GameObject> tvLetters;
        private readonly List<GameObject> _actualLetters = new();

        private string _original;
        private string _scrambled;

        private bool _canFunction;

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

            for (var i = 0; i <= _scrambled.Length - 1; i++)
            {
                var letter = _actualLetters[i].GetComponent<TextMeshProUGUI>();
                letter.text = _scrambled[i].ToString();
            }
        }
    }
}