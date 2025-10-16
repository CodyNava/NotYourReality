using UnityEngine;

[CreateAssetMenu(fileName = "InteractionInputData", menuName = "InteractionSystem/InputData")]
public class Interaction_Input_Data : ScriptableObject
{
    private bool _interactedClicked;
    private bool _interactedHold;
    private bool _interactedReleased;

    public bool InteractedClicked
    {
        get => _interactedClicked;
        set => _interactedClicked = value;
    }

    public bool InteractedHold
    {
        get => _interactedHold;
        set => _interactedHold = value;
    }

    public bool InteractedReleased
    {
        get => _interactedReleased;
        set => _interactedReleased = value;
    }

    public void Reset()
    {
        _interactedClicked = false;
        _interactedReleased = false;
        _interactedHold = false;
    }
}