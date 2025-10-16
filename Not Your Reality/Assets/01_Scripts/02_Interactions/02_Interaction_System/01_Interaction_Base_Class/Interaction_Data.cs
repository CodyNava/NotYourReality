using UnityEngine;

[CreateAssetMenu(fileName = "Interaction Data", menuName = "InteractionSystem/InteractionData")]
public class Interaction_Data : ScriptableObject
{
    private Interactable_Base _interactableBase;

    public Interactable_Base InteractableBase
    {
        get => _interactableBase;
        set => _interactableBase = value;
    }

    public void Interact()
    {
        _interactableBase.OnInteract();
        ResetData();
    }

    public bool IsSameInteractable(Interactable_Base newInteractable) => _interactableBase == newInteractable;
    public bool IsEmpty() => _interactableBase == null;
    public void ResetData() => _interactableBase = null;
}