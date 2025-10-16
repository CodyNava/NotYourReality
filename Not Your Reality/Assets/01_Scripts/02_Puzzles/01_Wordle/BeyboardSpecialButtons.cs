using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeyboardSpecialButtons : MonoBehaviour
{
    private string _action;

    public TextMeshProUGUI actionText;
    public WordlePuzzle wordlePuzzle;

    void Start()
    {
        _action = actionText.text;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        wordlePuzzle.OnSpecialKey(_action);
    }
}