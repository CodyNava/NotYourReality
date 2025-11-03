using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Wordle
{
   public class KeyboardButton : MonoBehaviour
   {
      private char _letter;
      private Image _background;

      public TextMeshProUGUI buttonLetter;
      public WordlePuzzle wordlePuzzle;

      private void Start()
      {
         _letter = buttonLetter.text[0];
         GetComponent<Button>().onClick.AddListener(OnClick);
         wordlePuzzle.RegisterKeyboardKey(this);
         _background = GetComponent<Image>();
      }

      private void OnClick() { wordlePuzzle.OnKeyboardClick(_letter); }

      public void SetColor(Color color) { _background.color = color; }
   }
}