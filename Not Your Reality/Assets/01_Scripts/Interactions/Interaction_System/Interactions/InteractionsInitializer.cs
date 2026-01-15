using System.Collections;
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
         StartCoroutine(ReEnableScripts(false, 0.5f));
         StartCoroutine(ReEnableScripts(true,0.5f));
      }
      
      
      private IEnumerator ReEnableScripts(bool status, float time)
      {
         yield return new WaitForSeconds(time);
         foreach (var interactable in interactableObjects)
         {
            interactable.enabled = status;
         }
      }
   }
}