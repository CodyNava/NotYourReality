using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Desert_Reflection_Room
{
    public class ReflectionGoal : MonoBehaviour
    {
        public List<Vector3> hits = new();
        public bool BeenHit() => hits.Count > 0;
        public void ClearHits() => hits.Clear();

        public void RegisterHit(Vector3 hit)
        {
            hits.Add(hit);
        }
    }       
}