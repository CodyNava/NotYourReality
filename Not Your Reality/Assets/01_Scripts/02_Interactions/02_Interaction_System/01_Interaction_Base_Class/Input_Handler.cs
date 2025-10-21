using UnityEngine;

public class Input_Handler : MonoBehaviour
{
   private Interaction_Input_Data _interactionInputData;
   public void Intialize(Interaction_Input_Data runtimeInput) { _interactionInputData = runtimeInput; }
   private void Update() { GetInteractionInputData(); }

   private void GetInteractionInputData()
   {
      if (!_interactionInputData) return;
      _interactionInputData.InteractedClicked = Input.GetKeyDown(KeyCode.E);
      _interactionInputData.InteractedHold = Input.GetKey(KeyCode.E);
      _interactionInputData.InteractedReleased = Input.GetKeyUp(KeyCode.E);
   }
}