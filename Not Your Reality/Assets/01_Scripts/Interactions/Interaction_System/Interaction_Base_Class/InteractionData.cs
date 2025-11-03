using UnityEngine;

namespace Interactions.Interaction_System.Interaction_Base_Class
{
   [CreateAssetMenu(fileName = "Interaction Data", menuName = "InteractionSystem/InteractionData")]
   public class InteractionData : ScriptableObject
   {
      public InteractableBase InteractableBase { get; set; }

      public void Interact()
      {
         InteractableBase.OnInteract();
         ResetData();
      }

      public bool IsSameInteractable(InteractableBase newInteractable) => InteractableBase == newInteractable;
      public bool IsEmpty() => !InteractableBase;
      public void ResetData() => InteractableBase = null;
   }
}