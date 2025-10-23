using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InteractionInputData _interactionInputData;

    public void Initialize(InteractionInputData runtimeInput)
    {
        Keybinds.MainInput.Interaction.Interact.started += OnInteractStarted;
        Keybinds.MainInput.Interaction.Interact.performed += WhileInteract;
        Keybinds.MainInput.Interaction.Interact.canceled += OnInteractReleased;
        _interactionInputData = runtimeInput;
    }

    private void OnDestroy()
    {
        Keybinds.MainInput.Interaction.Interact.started -= OnInteractStarted;
        Keybinds.MainInput.Interaction.Interact.performed -= WhileInteract;
        Keybinds.MainInput.Interaction.Interact.canceled -= OnInteractReleased;
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        _interactionInputData.InteractedClicked = true;
        _interactionInputData.InteractedHold = true;
    }

    private void WhileInteract(InputAction.CallbackContext context)
    {
        _interactionInputData.InteractedClicked = true;
        _interactionInputData.InteractedHold = true;
    }

    private void OnInteractReleased(InputAction.CallbackContext context)
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