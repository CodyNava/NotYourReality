using UnityEngine;

namespace Interactions.Interaction_System.Interaction_Base_Class
{
   [CreateAssetMenu(fileName = "InteractionInputData", menuName = "InteractionSystem/InputData")]
   public class InteractionInputData : ScriptableObject
   {
      public bool InteractedClicked { get; set; }

      public bool InteractedHold { get; set; }

      public bool InteractedReleased { get; set; }

      public void Reset()
      {
         InteractedClicked = false;
         InteractedReleased = false;
         InteractedHold = false;
      }
   }
}