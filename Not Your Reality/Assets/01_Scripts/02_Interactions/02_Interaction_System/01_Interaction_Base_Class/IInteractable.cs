public interface IInteractable
{
    float HoldDuration { get; }
    bool HoldInteract { get; }
    bool IsHolding { get;  }
    bool MultipleUse { get; }
    bool IsInteractable { get; }

    void OnInteract();
}