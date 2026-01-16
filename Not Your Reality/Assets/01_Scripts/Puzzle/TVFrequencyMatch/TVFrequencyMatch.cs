using System.Collections.Generic;
using System.Linq;
using Interactions.Interaction_System.Interactions;
using Interactions.Teleporter;
using UnityEngine;

namespace Puzzle.TVFrequencyMatch
{
   public class TVFrequencyMatch : MonoBehaviour
   {
      //[SerializeField] private OpenDoor door;
      [SerializeField] private TeleportTransformSpawn teleportTransformSpawn;
      [SerializeField] private List<TVManager> tvManagers = new();
      [SerializeField] private bool testWin; //todo remove by release
      private bool _alreadyWon;

      private void Start()
      {
         _alreadyWon = false;
         StartCoroutine(teleportTransformSpawn.MoveToDestination(true));

         /*foreach (Transform child in transform)
         {
             foreach (Transform grandChild in child)
             {
                 if (!grandChild.CompareTag("TV")) continue;
                 _tvManagers.Add(grandChild.GetComponent<TVManager>());
             }
         }*/
      }

      public void Update() //todo remove by release
      {
         if (!testWin) return;
         StartCoroutine(teleportTransformSpawn.MoveToDestination(false));
         testWin = false;
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