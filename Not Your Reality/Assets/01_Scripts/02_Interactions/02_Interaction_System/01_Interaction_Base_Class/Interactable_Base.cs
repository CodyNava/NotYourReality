using UnityEngine;

public class Interactable_Base : MonoBehaviour, IInteractable
{
    [Header("General Interactable Settings")]
    [Tooltip("If 'hold interact' is on:\n" +
             "<b>Value > 0:</b> Interaction will trigger after that amount (e.g. Crank)\n" +
             "<b>Value = 0:</b> Interaction will trigger as long as you hold it (e.g. Moving an Object)")]
    [SerializeField] private float holdDuration;
    [Tooltip("<b>On:</b> Hold Button to interact\n" +
             "<b>Off:</b> Press Button to interact")]
    [SerializeField] private bool holdInteract;
    [Tooltip("<b>IGNORE</b>, debug stat")]
    [SerializeField] private bool isHolding;
    [Tooltip("<b>IGNORE</b>, not implemented yet")]
    [SerializeField] private bool multipleUse;
    [Tooltip("<b>On:</b> The object can be interacted with\n" +
             "<b>Off:</b> The object cannot be interacted with")]
    [SerializeField] private bool isInteractable;
    [Tooltip("The Prompt that shows when hovering over an object")]
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