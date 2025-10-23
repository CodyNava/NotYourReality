using UnityEngine;
using TMPro;

public class LetterCube : MonoBehaviour
{
    public TextMeshPro letterText;
    public Renderer cubeRenderer;

    public void SetLetter(char letter)
    {
        letterText.text = letter.ToString();
    }

    public void SetColor(Color color)
    {
        cubeRenderer.material.color = color;
    }
}