using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Puzzle.TVFrequencyMatch
{
    public class TVFrequencyMatch : MonoBehaviour
    {
        [SerializeField] private Collider doorCollider;
        private readonly List<TVManager> _tvManagers = new ();

        private void Start()
        {
            foreach (Transform child in transform)
            {
                foreach (Transform grandChild in child)
                {
                    if (!grandChild.CompareTag("TV")) continue;
                    _tvManagers.Add(grandChild.GetComponent<TVManager>());
                }
            }
        }

        public void Status()
        {
            var winCounter = _tvManagers.Count(tvManager => tvManager.Completed);
            if (winCounter != _tvManagers.Count) return;
            if (doorCollider) doorCollider.enabled = true;
        }
    }
}