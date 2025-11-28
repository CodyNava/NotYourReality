using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Wordle
{
    public class WordKnitterTile : LetterTile
    {
        [SerializeField] private WordlePuzzle wordlePuzzle;
        [SerializeField] private Image frame;

        public void HandleClick()
        {
            wordlePuzzle.SelectTile(this);
        }

        public void SetFrameColor(Color color)
        {
            frame.color = color;
            frame.enabled = true;
        }

        public void ClearFrame()
        {
            frame.color = Color.clear;
            frame.enabled = false;
        }
    }
}