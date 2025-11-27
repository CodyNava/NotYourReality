using UnityEngine;

namespace System.DialogueSystem.SO
{
   [CreateAssetMenu(menuName = "DialogueSystem/Character")]
   public class Character : ScriptableObject
   {
      [Space(15)]
      public int characterID;
      [Space(15)]
      public string characterName;
      [Space(15)]
      public Color textColor;
   }
}