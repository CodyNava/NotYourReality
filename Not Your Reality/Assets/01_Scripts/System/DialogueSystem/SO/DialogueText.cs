using UnityEngine;

namespace System.DialogueSystem.SO
{
   [CreateAssetMenu(menuName = "DialogueSystem/Text")]
   public class DialogueText : ScriptableObject
   {
      [Space(15)]
      public Character currentSpeaker;
      [TextArea(2, 5)]
      public string subText;
      [Space(15)]
      public float duration;
      [Space(15)]
      public string showOnMarker;
      [Space(15)]
      public string advanceOnMarker;

      //todo Here Audio//
   }
}