using Interactions.Interaction_System.Interaction_Base_Class;
using Puzzle;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
   public class SequenceSwitch : InteractableBase
   {
      [Tooltip("The target of the Switch")]
      [SerializeField] private SwitchSequence switchSequence;

      private bool Interacted { get; set; }

      public override void OnInteract()
      {
         if (Interacted) return;
         base.OnInteract();
         ToggleOn();
      }

      private void ToggleOn()
      {
         gameObject.transform.Rotate(-60, 0, 0);
         switchSequence.AddSwitch(this);
         Interacted = true;
      }

      public void ToggleOff()
      {
         gameObject.transform.Rotate(60, 0, 0);
         Interacted = false;
      }
   }
}