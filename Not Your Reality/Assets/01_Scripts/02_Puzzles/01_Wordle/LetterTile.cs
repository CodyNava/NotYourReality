using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LetterTile : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI letterText;

    public void SetLetter(char letter)
    {
        letterText.text = letter.ToString().ToUpper();
    }

    public void SetColor(Color color)
    {
        background.color = color;
    }
}