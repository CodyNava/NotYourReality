using UnityEngine;

public class Input_Handler : MonoBehaviour
{
    [SerializeField] private Interaction_Input_Data interactionInputData;

    private void Start()
    {
        interactionInputData.Reset();
    }

    private void Update()
    {
        GetInteractionInputData();
    }

    private void GetInteractionInputData()
    {
        interactionInputData.InteractedClicked = Input.GetKeyDown(KeyCode.E);
        interactionInputData.InteractedHold = Input.GetKey(KeyCode.E);
        interactionInputData.InteractedReleased = Input.GetKeyUp(KeyCode.E);
    }
}