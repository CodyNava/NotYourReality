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

        private readonly List<List<LetterTile>> _wordleBoardTiles = new();
        private readonly List<List<LetterTile>> _wordKnitterBoardTiles = new();

        // ---------------------------
        // UNITY LIFECYCLE
        // ---------------------------
        private void Start()
        {
            ResetWordl();
        }

        // ---------------------------
        // RESET WORDLE BOARD
        // ---------------------------
        private void ResetWordl()
        {
            _currentGuess = 0;
            _isGameOver = false;
            _currentInput = "";

            _wordleBoardTiles.Clear();

            foreach (Transform row in wordleBoard)
            {
                List<LetterTile> rowTiles = new();

                foreach (Transform tileObj in row)
                {
                    var tile = tileObj.GetComponent<LetterTile>();
                    if (tile)
                    {
                        rowTiles.Add(tile);
                        tile.SetLetter(' ');
                        tile.SetColor(defaultColor);
                    }
                }

                _wordleBoardTiles.Add(rowTiles);
            }
        }

        // ---------------------------
        // TILE SELECTION (WORDKNITTER)
        // ---------------------------
        public void SelectTile(WordKnitterTile tile)
        {
            if (_selectedTile != null)
                _selectedTile.SetColor(defaultColor);

            _selectedTile = tile;
            _selectedTile.SetColor(Color.cyan);

            Debug.Log("[KNITTER] Tile Selected: " + tile);

            WordKnitterFocusButton();
        }

        // ---------------------------
        // KEYBOARD INPUT
        // ---------------------------
        public void OnKeyboardClick(char letter)
        {
            if (_isGameOver) return;

            // WORDLE MODE
            if (_wordleFocus)
            {
                if (_currentInput.Length < 5)
                {
                    _currentInput += letter;
                    UpdateCurrentRow();
                }
            }

            // KNITTER MODE
            if (_wordKnitterFocus)
            {
                Debug.Log("[KNITTER] Keyboard letter pressed: " + letter);
                if (_selectedTile == null) return;

                _selectedTile.SetLetter(letter);
            }
        }

        public void OnSpecialKey(string action)
        {
            if (_isGameOver) return;

            // WORDLE
            if (_wordleFocus)
            {
                if (action == "BACK")
                {
                    if (_currentInput.Length > 0)
                    {
                        _currentInput = _currentInput[..^1];
                        UpdateCurrentRow();
                    }
                }
                else if (action == "ENTER")
                {
                    SubmitGuess();
                }
            }

            // KNITTER
            if (_wordKnitterFocus)
            {
                if (_selectedTile == null) return;

                if (action == "BACK")
                {
                    _selectedTile.SetLetter(' ');
                }
                else if (action == "ENTER")
                {
                    // TODO: Submit logic
                }
            }
        }

        private void UpdateCurrentRow()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < _currentInput.Length)
                    _wordleBoardTiles[_currentGuess][i].SetLetter(_currentInput[i]);
                else
                    _wordleBoardTiles[_currentGuess][i].SetLetter(' ');
            }
        }

        // ---------------------------
        // WORDLE GUESS LOGIC
        // ---------------------------
        private void SubmitGuess()
        {
            if (_currentInput.Length != 5)
            {
                Debug.Log("Guess must be 5 characters long.");
                return;
            }

            var feedback = CheckGuess(_currentInput, wordList.targetWord);

            for (int i = 0; i < 5; i++)
            {
                char guessedLetter = char.ToUpper(_currentInput[i]);

                if (_keyboardButtons.TryGetValue(guessedLetter, out KeyboardButton key))
                {
                    Color col = feedback[i] switch
                    {
                        LetterState.Correct => correctColor,
                        LetterState.Present => presentColor,
                        _ => incorrectColor
                    };

                    _wordleBoardTiles[_currentGuess][i].SetColor(col);

                    if (col == incorrectColor)
                        key.SetColor(Color.gray);
                }
            }

            if (_currentInput.ToUpper() == wordList.targetWord)
            {
                Debug.Log("[WORDLE] You win!");
                _isGameOver = true;
                return;
            }

            _currentGuess++;
            _currentInput = "";

            if (_currentGuess >= maxGuesses)
            {
                Debug.Log("[WORDLE] You lose.");
                ResetWordl();
            }
        }

        private enum LetterState { Absent, Present, Correct }

        private static LetterState[] CheckGuess(string guess, string target)
        {
            guess = guess.ToUpper().Trim();
            target = target.ToUpper().Trim();

            LetterState[] result = new LetterState[guess.Length];
            List<char> targetLetters = new(target);

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
            if (!_keyboardButtons.ContainsKey(letter))
                _keyboardButtons.Add(letter, key);
        }

        // ---------------------------
        // FOCUS SYSTEM
        // ---------------------------
        public enum Focus
        {
            None,
            Wordle,
            WordKnitter
        }

        private Focus _focus = Focus.None;

        public void SetFocus(Focus f)
        {
            _focus = f;
            _wordleFocus = (f == Focus.Wordle);
            _wordKnitterFocus = (f == Focus.WordKnitter);

            Debug.Log("[FOCUS] Switched to: " + _focus);
        }

        public void WordKnitterFocusButton()
        {
            SetFocus(Focus.WordKnitter);
        }

        public void WordleFocusButton()
        {
            SetFocus(Focus.Wordle);
        }
    }
}
