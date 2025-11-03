using TMPro;
using UnityEngine;

namespace Interactions.Interaction_System.Interaction_Base_Class
{
   public class InteractionUIPanel : MonoBehaviour
   {
      [SerializeField] private TextMeshProUGUI tooltipText;

      public void SetTooltip(string tooltip) { tooltipText.SetText(tooltip); }

      public void Reset() { tooltipText.SetText(""); }
   }
}