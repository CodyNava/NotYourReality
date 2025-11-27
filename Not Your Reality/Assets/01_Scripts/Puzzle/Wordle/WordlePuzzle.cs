using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Wordle
{
    public class WordlePuzzle : MonoBehaviour
    {
        private Dictionary<char, KeyboardButton> _keyboardButtons = new();

        [Header("References")]
        [SerializeField] private WordListManager wordList;

        [SerializeField] private Transform wordleBoard;
        [SerializeField] private Transform wordKnitterBoard;
        [SerializeField] private int maxGuesses = 6;
        //[SerializeField] private Collider doorCollider;

        [Header("Colors")]
        [SerializeField] private Color correctColor = Color.green;

        [SerializeField] private Color presentColor = Color.yellow;
        [SerializeField] private Color incorrectColor = Color.red;
        [SerializeField] private Color defaultColor = Color.white;

        private bool _wordKnitterFocus;
        private bool _wordleFocus;
        private int _currentGuess;
        private string _currentInput;
        private bool _isGameOver;
        private WordKnitterTile _selectedTile;

        private readonly List<List<LetterTile>> _wordleBoard = new();
        private readonly List<List<LetterTile>> _wordKnitterBoard = new();

        private void Start()
        {
            /* if (doorCollider)
             {
                 doorCollider.enabled = false;
             }*/

            ResetWordl();
        }

        private void ResetWordl()
        {
            _currentGuess = 0;
            _isGameOver = false;
            _currentInput = "";
            foreach (Transform row in wordleBoard)
            {
                List<LetterTile> rowTiles = new();
                foreach (Transform tileobj in row)
                {
                    var tile = tileobj.GetComponent<LetterTile>();
                    if (tile)
                    {
                        rowTiles.Add(tile);
                        tile.SetLetter(' ');
                        tile.SetColor(defaultColor);
                    }
                }

                _wordleBoard.Add(rowTiles);
            }
        }

        public void SelectTile(WordKnitterTile tile)
        {
            if (_selectedTile == null)
            {
                _selectedTile = tile;
                _selectedTile.SetColor(Color.cyan);
            }

            if (_selectedTile != tile)
            {
                _selectedTile.SetColor(defaultColor);
                _selectedTile = tile;
                _selectedTile.SetColor(Color.cyan);
            }

            Debug.Log("Selected Tile:" + tile);
        }

        public void OnKeyboardClick(char letter)
        {
            if (_isGameOver) return;
            if (_wordleFocus)
            {
                if (_currentInput.Length < 5)
                {
                    _currentInput += letter;
                    UpdateCurrentRow();
                }
            }

            if (_wordKnitterFocus)
            {
                Debug.Log("Word Knitter Keyboard Clicked");
                if (_selectedTile == null) return;
                
                _selectedTile.SetLetter(letter);    
                
            }
        }

        public void OnSpecialKey(string action)
        {
            if (_isGameOver) return;

            if (_wordleFocus)
            {
                if (action == "BACK")
                {
                    if (_currentInput.Length > 0)
                    {
                        _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
                        UpdateCurrentRow();
                    }
                }
                else if (action == "ENTER")
                {
                    SubmitGuess();
                }
            }

            if (_wordKnitterFocus)
            {
                if (_selectedTile == null) return;

                if (action == "BACK")
                {
                    _selectedTile.SetLetter(' ');
                }
                else if (action == "ENTER")
                {
                    //TODO Submit für den WordKnitter bauen.
                }
            }
        }

        private void UpdateCurrentRow()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < _currentInput.Length)
                {
                    _wordleBoard[_currentGuess][i].SetLetter(_currentInput[i]);
                }
                else
                {
                    _wordleBoard[_currentGuess][i].SetLetter(' ');
                }
            }
        }

        private void SubmitGuess()
        {
            if (_currentInput.Length != 5)
            {
                Debug.Log("Guess must be 5 characters long");
                return;
            }

            LetterState[] feedback = CheckGuess(_currentInput, wordList.targetWord);

            for (int i = 0; i < 5; i++)
            {
                char guessedLetter = char.ToUpper(_currentInput[i]);
                if (_keyboardButtons.TryGetValue(guessedLetter, out KeyboardButton key))
                {
                    Color color = feedback[i] switch
                    {
                        LetterState.Correct => correctColor,
                        LetterState.Present => presentColor,
                        _ => incorrectColor
                    };
                    _wordleBoard[_currentGuess][i].SetColor(color);
                    if (color != correctColor && color != presentColor)
                    {
                        key.SetColor(Color.gray);
                    }
                }
            }

            if (_currentInput.ToUpper() == wordList.targetWord)
            {
                Debug.Log("You Win");
                _isGameOver = true;
                /*if (doorCollider != null)
                {
                    doorCollider.enabled = true;
                }*/

                return;
            }

            _currentGuess++;
            _currentInput = "";

            if (_currentGuess >= maxGuesses)
            {
                Debug.Log("You Lose");
                ResetWordl();
            }
        }

        private enum LetterState
        {
            Absent,
            Present,
            Correct
        }

        private static LetterState[] CheckGuess(string guess, string target)
        {
            guess = guess.ToUpper().Trim();
            target = target.ToUpper().Trim();

            Debug.Log(guess);
            Debug.Log(target);

            LetterState[] result = new LetterState[guess.Length];
            List<char> targetLetters = new List<char>(target);

            for (int i = 0; i < guess.Length; i++)
            {
                if (guess[i] == target[i])
                {
                    result[i] = LetterState.Correct;
                    targetLetters[i] = '_';
                }
            }

            for (int i = 0; i < guess.Length; i++)
            {
                if (result[i] == LetterState.Correct) continue;

                if (targetLetters.Contains(guess[i]))
                {
                    result[i] = LetterState.Present;
                    targetLetters[targetLetters.IndexOf(guess[i])] = '_';
                }
                else
                {
                    result[i] = LetterState.Absent;
                }
            }

            return result;
        }

        public void RegisterKeyboardKey(KeyboardButton key)
        {
            char letter = char.ToUpper(key.buttonLetter.text[0]);
            if (!_keyboardButtons.ContainsKey(letter)) _keyboardButtons.Add(letter, key);
        }

        public void WordKnitterFocus()
        {
            Debug.Log("Word Knitter Focus");
            _wordKnitterFocus = true;
            SwapFocus();
        }

        public void WordleFocus()
        {
            Debug.Log("Wordle Focus");
            _wordleFocus = true;
            SwapFocus();
        }

        private void SwapFocus()
        {
            if (_wordKnitterFocus)
            {
                _wordleFocus = false;
            }
            else if (_wordleFocus)
            {
                _wordKnitterFocus = false;
            }
        }
    }
}