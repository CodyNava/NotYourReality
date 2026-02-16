using System.Collections.Generic;
using System.GlobalEventSystem;
using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;

namespace Puzzle.General
{
   public class Key : MonoBehaviour
   {
      [SerializeField] private List<GameObject> allDoorHandles;
      
      private void OnCollisionEnter(Collision other)
      {
         if (!other.gameObject.CompareTag("Door")) return;
         foreach (var handle in allDoorHandles)
         {
             handle.layer = LayerMask.NameToLayer("Interactable");
             if (handle.TryGetComponent<DoorHandle>(out var doorHandle))
             {
                 doorHandle.IsInteractable = true;
             }
         }
         GlobalEventManager.OnKeyUsed();
         Destroy(gameObject);
      }
   }
}
