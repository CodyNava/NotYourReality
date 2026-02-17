using System.GlobalEventSystem;
using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;

namespace Puzzle.General
{
   public class Key : MonoBehaviour
   {
      private void OnCollisionEnter(Collision other)
      {
         if (!other.gameObject.CompareTag("Door")) return;
         var door = other.gameObject.GetComponentInChildren<DoorHandle>();
         door.IsInteractable = true;
         GlobalEventManager.OnKeyUsed();
         Destroy(gameObject);
      }
   }
}
