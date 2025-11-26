using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Puzzle.TVFrequencyMatch
{
    public class TVFrequencyMatch : MonoBehaviour
    {
        [SerializeField] private Collider doorCollider;
        private List<TVManager> _tvManagers;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                _tvManagers.Add(child.GetComponent<TVManager>());
            }
        }

        public void Status()
        {
            var winCounter = _tvManagers.Count(tvManager => tvManager.Completed);
            if (winCounter == _tvManagers.Count)
            {
                if (doorCollider) doorCollider.enabled = true;
            }
        }
    }
}