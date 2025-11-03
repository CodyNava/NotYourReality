using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Wordle
{
   public class KeyboardSpecialButtons : MonoBehaviour
   {
      private string _action;

      public TextMeshProUGUI actionText;
      public WordlePuzzle wordlePuzzle;

      private void Start()
      {
         _action = actionText.text;
         GetComponent<Button>().onClick.AddListener(OnClick);
      }

      private void OnClick() { wordlePuzzle.OnSpecialKey(_action); }
   }
}