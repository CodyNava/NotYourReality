using UnityEngine;

public class Interactable_Base : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")]
    [SerializeField] private float holdDuration;
    [SerializeField] private bool holdInteract;
    [SerializeField] private bool multipleUse;
    [SerializeField] private bool isInteractable;

    public float HoldDuration => holdDuration;
    public bool HoldInteract => holdInteract;
    public bool MultipleUse => multipleUse;
    public bool IsInteractable => isInteractable;

    public virtual void OnInteract()
    {
        Debug.Log("Interacted" + gameObject.name);
    }
}