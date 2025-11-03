using System.Collections.Generic;
using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
   public class LightSwitch : InteractableBase
   {
      [Tooltip("All objects you want to be effected by the Switch")]
      [SerializeField] private List<GameObject> effectedObjects;
      private bool _interacted;

      public override void OnInteract()
      {
         base.OnInteract();
         //Current Use: Turns effected Objects off or on
         if (!_interacted)
         {
            ToggleOn();
            foreach (var effectedObject in effectedObjects) { effectedObject.SetActive(true); }
         }
         else
         {
            ToggleOff();
            foreach (var effectedObject in effectedObjects) { effectedObject.SetActive(false); }
         }
      }

      private void ToggleOn()
      {
         gameObject.transform.Rotate(-60, 0, 0);
         _interacted = true;
      }

      private void ToggleOff()
      {
         gameObject.transform.Rotate(60, 0, 0);
         _interacted = false;
      }
   }
}