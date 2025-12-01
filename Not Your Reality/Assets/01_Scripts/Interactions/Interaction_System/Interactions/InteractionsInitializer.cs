using System.Collections.Generic;
using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
   public class InteractionsInitializer : MonoBehaviour
   {
      [SerializeField] private List<InteractableBase> interactableObjects;

      private void Start()
      {
         interactableObjects =
            new List<InteractableBase>(FindObjectsByType<InteractableBase>(sortMode: FindObjectsSortMode.None));
         ReEnableScripts(false);
         ReEnableScripts(true);
      }

      private void ReEnableScripts(bool status)
      {
         foreach (var interactable in interactableObjects)
         {
            interactable.enabled = status;
         }
      }
   }
}