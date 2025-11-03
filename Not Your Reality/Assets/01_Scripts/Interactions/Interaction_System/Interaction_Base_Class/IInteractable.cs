namespace Interactions.Interaction_System.Interaction_Base_Class
{
   public interface IInteractable
   {
      float HoldDuration { get; }
      bool HoldInteract { get; }
      bool IsHolding { get; }
      bool MultipleUse { get; }
      bool IsInteractable { get; }

      string TooltipMessage { get; }

      void OnInteract();
   }
}