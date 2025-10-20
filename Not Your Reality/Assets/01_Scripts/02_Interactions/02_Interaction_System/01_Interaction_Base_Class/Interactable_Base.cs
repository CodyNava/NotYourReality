using UnityEngine;

public class Interactable_Base : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")]
    [SerializeField] private float holdDuration;
    [SerializeField] private bool holdInteract;
    [SerializeField] private bool isHolding;
    [SerializeField] private bool multipleUse;
    [SerializeField] private bool isInteractable;
    [SerializeField] private string tooltipMessage = "Press E to Interact";

    public float HoldDuration => holdDuration;
    public bool HoldInteract => holdInteract;
    public bool IsHolding => isHolding;
    public bool MultipleUse => multipleUse;
    public bool IsInteractable => isInteractable;

    public string TooltipMessage => tooltipMessage;

    public virtual void OnInteract()
    {
        Debug.Log("Interacted" + gameObject.name);
    }
}