using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Wordle
{
    public class KnitterWordListManager : MonoBehaviour
    {
        [Header("Knitter Word List")]
        public TextAsset knitterBundleList;

        private readonly List<List<string>> _allBundles = new();
        public List<string> chosenBundle = new();

        private void Awake()
        {
            ParseBundles();

            if (_allBundles.Count == 0)
            {
                return;
            }

            chosenBundle = _allBundles[Random.Range(0, _allBundles.Count)];
        }

        private void ParseBundles()
        {
            if (knitterBundleList == null)
                return;


            string[] lines = knitterBundleList.text.Split('\n');

            List<string> currentBundle = new();

            foreach (string raw in lines)
            {
                string line = raw.Trim().ToUpper();

                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                if (line == "---")
                {
                    AddBundle(currentBundle);
                    currentBundle = new();
                    continue;
                }

                currentBundle.Add(line);
            }
        }

        private void AddBundle(List<string> bundle)
        {
            if (bundle.Count != 6)
            {
                return;
            }

            // Check for each Word has the correct length
            if (bundle[0].Length != 5 ||
                bundle[1].Length != 5 ||
                bundle[2].Length != 3 ||
                bundle[3].Length != 3 ||
                bundle[4].Length != 3 ||
                bundle[5].Length != 3)
            {
                return;
            }

            _allBundles.Add(new List<string>(bundle));
        }
    }
}