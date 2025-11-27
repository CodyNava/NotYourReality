using UnityEngine;

namespace System.DialogueSystem.SO
{
   [CreateAssetMenu(menuName = "DialogueSystem/Sequences")]
   public class DialogueSequence : ScriptableObject
   {
      [Space(15)]
      public DialogueText[] dialogueTexts;
   }
}