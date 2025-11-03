using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
   public class PathOfTruthTiles : MonoBehaviour
   {
      private void OnTriggerEnter(Collider other)
      {
         switch (gameObject.tag)
         {
            case "Safe":
            case "Not Safe" when !other.gameObject.CompareTag("Player"): return;
            case "Not Safe" when other.gameObject.CompareTag("Player"): Debug.Log("Reset Hit"); break;
         }
      }
   }
}