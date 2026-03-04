using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Wordle
{
    public class WordKnitterTile : LetterTile
    {
        [SerializeField] private WordlePuzzle wordlePuzzle;
        [SerializeField] private Image frame;
    }
}