using System.Collections.Generic;
using System.Linq;
using Interactions.Teleporter;
using UnityEngine;

namespace Puzzle.TVFrequencyMatch
{
   public class TVFrequencyMatch : MonoBehaviour
   {
      [SerializeField] private TeleportTransformSpawn teleportTransformSpawn;
      [SerializeField] private List<TVManager> tvManagers = new();
      private bool _alreadyWon;

      private void Start()
      {
         _alreadyWon = false;
         StartCoroutine(teleportTransformSpawn.MoveToDestination(true));
      }
      
      public void Status()
      {
         var winCounter = tvManagers.Count(tvManager => tvManager.Completed);
         if (winCounter != tvManagers.Count) return;
         if (!_alreadyWon) { StartCoroutine(teleportTransformSpawn.MoveToDestination(false)); }

         _alreadyWon = true;
      }
   }
}