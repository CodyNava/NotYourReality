using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.TVFrequencyMatch
{
    public class TVWordManager : MonoBehaviour
    {
        [Header("TW Word Manager")] [Tooltip("The .txt file with the words for this riddle")] [SerializeField]
        private TextAsset words;

        private readonly List<(string original, string scrambled)> _wordPairs = new();
        public string OriginalWord { get; private set; }

        public string ScrambledWord { get; private set; }

        private void Awake()
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
            var rand = Random.Range(0, _wordPairs.Count);
            OriginalWord = _wordPairs[rand].original;
            ScrambledWord = _wordPairs[rand].scrambled;
            
            Debug.Log(OriginalWord + ScrambledWord);
        }
    }
}