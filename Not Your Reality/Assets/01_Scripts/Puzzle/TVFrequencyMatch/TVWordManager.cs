using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Puzzle.TVFrequencyMatch
{
    public class TVWordManager : MonoBehaviour
    {
        [Header("TW Word Manager")]
        [Tooltip("The .txt file with the words for this riddle \n"
                 + "<b>PLEASE, I BEG YOU, MAKE SURE THERE ARE MORE WORDS THAN TV'S OR THE GAME BREAKS</b>")]
        [SerializeField]
        private TextAsset words;

        private readonly List<(string original, string scrambled)> _wordPairs = new();
        private readonly List<(string original, string scrambled)> _usedPairs = new();

        public string OriginalWord { get; private set; }
        public string ScrambledWord { get; private set; }

        private void Awake()
        {
            ScrambleWords();
        }

        private void ScrambleWords()
        {
            var pairs = words.text.Split('\n');
            foreach (var pair in pairs)
            {
                var line = pair.Trim();
                var parts = line.Split('|');
                var original = parts[0].Trim().ToUpper();
                var scrambled = parts[1].Trim().ToUpper();
                _wordPairs.Add((original, scrambled));
            }
        }

        public void SelectPair()
        {
            Debug.Log(_usedPairs.Count);
            Debug.Log(_wordPairs.Count);
            
            var unusedIndices = new List<int>(_wordPairs.Count);
            for (var i = 0; i < _wordPairs.Count; i++)
            {
                if (!_usedPairs.Contains(_wordPairs[i]))
                {
                    unusedIndices.Add(i);
                }
            }

            if (unusedIndices.Count == 0)
            {
                Debug.Log("All Words Used");
                OriginalWord = ScrambledWord = string.Empty;
                return;
            }
            var randIndex = unusedIndices[Random.Range(0, unusedIndices.Count)];

            OriginalWord = _wordPairs[randIndex].original;
            ScrambledWord = _wordPairs[randIndex].scrambled;

            _usedPairs.Add(_wordPairs[randIndex]);

            Debug.Log(OriginalWord + ScrambledWord);
        }
    }
}