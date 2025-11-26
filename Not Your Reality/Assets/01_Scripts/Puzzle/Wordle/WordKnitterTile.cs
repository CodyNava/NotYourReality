using UnityEngine;

namespace Puzzle.Wordle
{
    public class WordKnitterTile : LetterTile
    {
        [SerializeField] private WordlePuzzle wordlePuzzle;

        public void HandleClick()
        {
            wordlePuzzle.SelectTile(this);
        }
        
    }
}