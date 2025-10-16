using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardButton : MonoBehaviour
{
    private char _letter;
    
    public TextMeshProUGUI buttonLetter;
    public WordlePuzzle wordlePuzzle;

    void Start()
    {
        _letter = buttonLetter.text[0];
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        wordlePuzzle.OnKeyboardClick(_letter);
    }
}
