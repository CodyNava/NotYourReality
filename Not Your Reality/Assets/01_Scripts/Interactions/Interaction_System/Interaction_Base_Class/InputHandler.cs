using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactions.Interaction_System.Interaction_Base_Class
{
   public class InputHandler : MonoBehaviour
   {
      private InteractionInputData _interactionInputData;

      public void Initialize(InteractionInputData runtimeInput)
      {
         InputManager.Input.Interaction.OnInteract.started += OnInteractStarted;
         InputManager.Input.Interaction.OnInteract.performed += OnInteractPerformed;
         InputManager.Input.Interaction.OnInteract.canceled += OnInteractCanceled;
         _interactionInputData = runtimeInput;
      }

      private void OnDestroy()
      {
         InputManager.Input.Interaction.OnInteract.started -= OnInteractStarted;
         InputManager.Input.Interaction.OnInteract.performed -= OnInteractPerformed;
         InputManager.Input.Interaction.OnInteract.canceled -= OnInteractCanceled;
      }

      private void OnInteractStarted(InputAction.CallbackContext context)
      {
         _interactionInputData.InteractedClicked = true;
         _interactionInputData.InteractedHold = true;
      }

      private void OnInteractPerformed(InputAction.CallbackContext context)
      {
         _interactionInputData.InteractedClicked = true;
         _interactionInputData.InteractedHold = true;
      }

      private void OnInteractCanceled(InputAction.CallbackContext context)
      {
         _interactionInputData.InteractedHold = false;
         _interactionInputData.InteractedReleased = true;
      }

      private void LateUpdate()
      {
         if (!_interactionInputData) return;
         _interactionInputData.InteractedClicked = false;
         _interactionInputData.InteractedReleased = false;
      }
   }
}