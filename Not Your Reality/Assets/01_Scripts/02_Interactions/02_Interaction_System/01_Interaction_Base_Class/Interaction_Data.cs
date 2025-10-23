using UnityEngine;

[CreateAssetMenu(fileName = "Interaction Data", menuName = "InteractionSystem/InteractionData")]
public class InteractionData : ScriptableObject
{
    public Interactable_Base InteractableBase { get; set; }

    public void Interact()
    {
        InteractableBase.OnInteract();
        ResetData();
    }

    public bool IsSameInteractable(Interactable_Base newInteractable) => InteractableBase == newInteractable;
    public bool IsEmpty() => InteractableBase == null;
    public void ResetData() => InteractableBase = null;
}