using TMPro;
using UnityEngine;

namespace System.DialogueSystem.SO
{
   [CreateAssetMenu(menuName = "DialogueSystem/SubtitleSettingsData")]
   public class SubtitleSettingsData : ScriptableObject
   {
      [Space(15)]
      public int maxLines;
      public Color textDefaultColor;
      public TextAlignmentOptions alignment;

      [Space(15)]
      public float fontSizeMain;
      public FontStyles fontStyleMain;

      [Space(15)]
      public float fontSizeName;
      public FontStyles fontStyleName;
      public bool displayFowlersName;

      [Space(15)]
      public Color backgroundColor;

      [Space(15)]
      public float outlineThickness;
      public Color outlineColor;
   }
}