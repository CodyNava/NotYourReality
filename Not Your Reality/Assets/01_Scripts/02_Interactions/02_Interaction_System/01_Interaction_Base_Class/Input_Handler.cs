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
        interactionInputData.InteractedClicked = Input.GetMouseButtonDown(0);
        interactionInputData.InteractedReleased = Input.GetMouseButtonUp(0);
    }
}