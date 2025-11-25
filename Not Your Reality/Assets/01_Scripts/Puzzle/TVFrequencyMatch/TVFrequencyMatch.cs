using UnityEngine;

namespace Puzzle.TVFrequencyMatch
{
    public class TVFrequencyMatch : MonoBehaviour
    {
        private TVWordManager _tvWordManager;


        private void Start()
        {
            _tvWordManager = GetComponent<TVWordManager>();
        }
    }
}