using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardButton : MonoBehaviour
{
    private char _letter;
    private Image background;
    
    public TextMeshProUGUI buttonLetter;
    public WordlePuzzle wordlePuzzle;

    void Start()
    {
        _letter = buttonLetter.text[0];
        GetComponent<Button>().onClick.AddListener(OnClick);
        wordlePuzzle.RegisterKeyboardKey(this);
        background = GetComponent<Image>();
    }

    void OnClick()
    {
        wordlePuzzle.OnKeyboardClick(_letter);
    }
    
    public void SetColor(Color color)
    {
        background.color = color;
    }
}
