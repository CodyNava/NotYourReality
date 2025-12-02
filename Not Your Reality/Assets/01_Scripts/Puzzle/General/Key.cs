using Interactions.Interaction_System.Interactions;
using UnityEngine;

namespace Puzzle.General
{
   public class Key : MonoBehaviour
   {
      private void OnCollisionEnter(Collision other)
      {
         if (!other.gameObject.CompareTag("Door")) return;
         var door = other.gameObject.GetComponent<OpenDoor>();
         door.UnlockDoor();
         Destroy(gameObject);
      }
   }
}
