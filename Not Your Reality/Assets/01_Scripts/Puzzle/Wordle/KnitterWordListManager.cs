using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Wordle
{
    public class KnitterWordListManager : MonoBehaviour
    {
        [Header("Knitter Word List")]
        public TextAsset knitterBundleList;

        public List<List<string>> allBundles = new();
        public List<string> chosenBundle = new();

        private void Awake()
        {
            ParseBundles();

            if (allBundles.Count == 0)
            {
                Debug.LogError("No Bundles Found");
                return;
            }

            chosenBundle = allBundles[Random.Range(0, allBundles.Count)];

            Debug.Log("[KNITTER] Chosen Bundle: " + string.Join(", ", chosenBundle));
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
                Debug.LogWarning("[KNITTER] Ignored bundle with wrong size: " + bundle.Count);
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
                Debug.LogWarning("[KNITTER] Ignored bundle with wrong word length: " + bundle);
                return;
            }

            allBundles.Add(new List<string>(bundle));
        }
    }
}