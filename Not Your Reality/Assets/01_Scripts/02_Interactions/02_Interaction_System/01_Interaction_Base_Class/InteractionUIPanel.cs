using TMPro;
using UnityEngine;

public class InteractionUIPanel : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI tooltipText;

   public void SetTooltip(string tooltip)
   {
      tooltipText.SetText(tooltip);
   }

   public void Reset()
   {
      tooltipText.SetText("");
   }
}