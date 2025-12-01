using System.Collections.Generic;
using Interactions.Interaction_System.Interactions;
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

        [Header("Animation")]
        [SerializeField] private float revealDuration = 1.5f;
        [SerializeField] private OpenDoor door;

        private bool _isRevealing;
        private float _perLetterDelay;

        private bool _wordKnitterFocus;
        private bool _wordleFocus;
        private int _currentGuess;
        private string _currentInput;
        private bool _isGameOver;
        private WordKnitterTile _selectedTile;

        [SerializeField] private KnitterWordListManager knitterBundles;
        private List<string> _knitterSolutions;

        private List<List<int>> _knitterTilePositions = new()
        {
            new List<int> { 4, 5, 6, 7, 8 },
            new List<int> { 0, 2, 6, 10, 12 },
            new List<int> { 1, 2, 3 },
            new List<int> { 1, 5, 9 },
            new List<int> { 9, 10, 11 },
            new List<int> { 3, 7, 11 }
        };

        private List<List<int>> _knitterWordleTilePositions = new()
        {
            new List<int> { 0, 1, 2, 3, 4 },
            new List<int> { 0, 1, 2, 3, 4 },
            new List<int> { 1, 2, 3 },
            new List<int> { 1, 2, 3 },
            new List<int> { 1, 2, 3 },
            new List<int> { 1, 2, 3 }
        };

        private List<List<WordKnitterTile>> _knitterWordTiles;

        private readonly List<List<LetterTile>> _wordleBoardTiles = new();
        private readonly List<WordKnitterTile> _wordKnitterBoardTiles = new();

        // ---------------------------
        // UNITY LIFECYCLE
        // ---------------------------
        private void Start()
        {
            ResetWordl();
            ResetWordKnitter();
            BuildKnitterWordTiles();
            _knitterSolutions = knitterBundles.chosenBundle;
            door.IsInteractable = false;
            Debug.Log("[KNITTER] Knitter Solutions: " + _knitterSolutions.Count);
        }

        // ---------------------------
        // BUILD KNITTER WORD TILES
        // --------------------------- 
        private void BuildKnitterWordTiles()
        {
            _knitterWordTiles = new List<List<WordKnitterTile>>();

            for (int w = 0; w < _knitterTilePositions.Count; w++)
            {
                List<WordKnitterTile> wordTiles = new();

                foreach (int tileIndex in _knitterTilePositions[w])
                {
                    wordTiles.Add(_wordKnitterBoardTiles[tileIndex]);
                }

                _knitterWordTiles.Add(wordTiles);
            }
        }

        // ---------------------------
        // RESET PUZZLE BOARD
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

        private void ResetWordKnitter()
        {
            _wordleFocus = true;
            _wordKnitterFocus = false;
            _selectedTile = null;

            _wordKnitterBoardTiles.Clear();

            foreach (Transform tileObj in wordKnitterBoard)
            {
                var tile = tileObj.GetComponent<WordKnitterTile>();
                if (tile)
                {
                    _wordKnitterBoardTiles.Add(tile);
                    tile.SetLetter(' ');
                    tile.SetColor(defaultColor);
                    Debug.Log("[KNITTER] Added tile: " + tile);
                    Debug.Log("[KNITTER] Board size: " + _wordKnitterBoardTiles.Count);
                }
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
                    SubmitGuess();
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
            if (_wordleFocus)
            {
                if (_isRevealing) return;

                if (_currentInput.Length != 5)
                {
                    Debug.Log("Guess must be 5 characters long.");
                    return;
                }

                StartCoroutine(RevealGuess(_currentInput));
            }
            else if (_wordKnitterFocus)
            {
                SubmitWordKnitter();
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

        // ---------------------------
        // WORDKNITTER LOGIC
        // ---------------------------

        private void CompareGuessToKnitterSolution(
            string guess,
            int pos,
            List<WordKnitterTile> greenTiles,
            List<WordKnitterTile> yellowTiles,
            HashSet<int> greenWords,
            HashSet<int> yellowWords)
        {
            char g = char.ToUpper(guess[pos]);

            for (int w = 0; w < _knitterSolutions.Count; w++)
            {
                string solution = _knitterSolutions[w].ToUpper();
                List<int> slotMap = _knitterWordleTilePositions[w];
                List<WordKnitterTile> wordTiles = _knitterWordTiles[w];

                List<int> matchingTiles = new();
                for (int i = 0; i < solution.Length && i < slotMap.Count; i++)
                {
                    if (solution[i] == g)
                    {
                        matchingTiles.Add(i);
                    }
                }

                if (matchingTiles.Count == 0)
                {
                    continue;
                }

                bool anyGreen = false;

                foreach (int i in matchingTiles)
                {
                    if (slotMap[i] == pos)
                    {
                        greenTiles.Add(wordTiles[i]);
                        anyGreen = true;
                    }
                }

                if (anyGreen)
                {
                    greenWords.Add(w);
                }
                else
                {
                    foreach (int i in matchingTiles)
                    {
                        yellowTiles.Add(wordTiles[i]);
                    }

                    yellowWords.Add(w);
                }
            }
        }

        private void ResetKnitterColors()
        {
            foreach (WordKnitterTile tile in _wordKnitterBoardTiles)
            {
                tile.SetColor(defaultColor);
                tile.ClearFrame();
            }
        }

        private System.Collections.IEnumerator RevealGuess(string guess)
        {
            _isRevealing = true;

            ResetKnitterColors();

            string upperGuess = guess.ToUpper().Trim();
            _perLetterDelay = revealDuration / 5f;


            for (int pos = 0; pos < guess.Length; pos++)
            {
                char g = upperGuess[pos];

                List<WordKnitterTile> greenTiles = new();
                List<WordKnitterTile> yellowTiles = new();
                HashSet<int> greenwords = new();
                HashSet<int> yellowWords = new();

                CompareGuessToKnitterSolution(
                    guess.ToUpper().Trim(),
                    pos,
                    greenTiles,
                    yellowTiles,
                    greenwords,
                    yellowWords
                );

                LetterState state;
                if (greenTiles.Count > 0)
                {
                    state = LetterState.Correct;
                }
                else if (yellowTiles.Count > 0)
                {
                    state = LetterState.Present;
                }
                else
                {
                    state = LetterState.Absent;
                }

                if (_keyboardButtons.TryGetValue(g, out KeyboardButton key))
                {
                    Color color = state switch
                    {
                        LetterState.Correct => correctColor,
                        LetterState.Present => presentColor,
                        _ => incorrectColor
                    };

                    _wordleBoardTiles[_currentGuess][pos].SetColor(color);

                    if (color == incorrectColor)
                        key.SetColor(Color.gray);
                }


                /*
                foreach (WordKnitterTile tile in yellowTiles)
                {
                    tile.SetColor(presentColor);
                }

                foreach (WordKnitterTile tile in greenTiles)
                {
                    tile.SetColor(correctColor);
                }
                */

                foreach (int w in yellowWords)
                {
                    foreach (WordKnitterTile tile in _knitterWordTiles[w])
                    {
                        tile.SetFrameColor(presentColor);
                    }
                }

                foreach (int w in greenwords)
                {
                    foreach (WordKnitterTile tile in _knitterWordTiles[w])
                    {
                        tile.SetFrameColor(correctColor);
                    }
                }

                yield return new WaitForSeconds(_perLetterDelay);

                foreach (int w in yellowWords)
                {
                    foreach (WordKnitterTile tile in _knitterWordTiles[w])
                    {
                        tile.ClearFrame();
                    }
                }

                foreach (int w in greenwords)
                {
                    foreach (WordKnitterTile tile in _knitterWordTiles[w])
                    {
                        tile.ClearFrame();
                    }
                }
            }

            _isRevealing = false;

            _currentGuess++;
            _currentInput = "";

            if (_currentGuess >= maxGuesses)
            {
                ResetWordl();
            }
        }

        private void SubmitWordKnitter()
        {
            foreach (WordKnitterTile t in _wordKnitterBoardTiles)
            {
                t.ClearFrame();
            }

            bool anyEmpty = false;
            bool allWordsSolved = true;

            var tileBestState = new Dictionary<WordKnitterTile, LetterState>();

            bool[] wordSolved = new bool[_knitterSolutions.Count];


            for (int w = 0; w < _knitterSolutions.Count; w++)
            {
                string solution = _knitterSolutions[w].ToUpper().Trim();
                List<WordKnitterTile> wordTiles = _knitterWordTiles[w];

                int len = solution.Length;
                char[] guessChars = new char[len];

                bool thisWordHasEmptyTiles = false;

                for (int i = 0; i < len; i++)
                {
                    char c = (i < wordTiles.Count) ? wordTiles[i].GetLetter() : ' ';

                    if (c == ' ' || c == '\0')
                    {
                        anyEmpty = true;
                        thisWordHasEmptyTiles = true;
                        guessChars[i] = ' ';
                    }
                    else
                    {
                        guessChars[i] = char.ToUpper(c);
                    }
                }

                string currentGuess = new string(guessChars);

                LetterState[] states = CheckGuess(currentGuess, solution);

                bool thisWordSolved = !thisWordHasEmptyTiles;

                for (int i = 0; i < states.Length && i < wordTiles.Count; i++)
                {
                    WordKnitterTile tile = wordTiles[i];
                    LetterState state = states[i];

                    if (state != LetterState.Correct)
                    {
                        thisWordSolved = false;
                    }

                    if (!tileBestState.TryGetValue(tile, out LetterState existing))
                    {
                        tileBestState[tile] = state;
                    }
                    else
                    {
                        if ((int)state > (int)existing)
                        {
                            tileBestState[tile] = state;
                        }
                    }
                }

                wordSolved[w] = thisWordSolved;
                if (!thisWordSolved)
                {
                    allWordsSolved = false;
                }
            }

            foreach (WordKnitterTile tile in _wordKnitterBoardTiles)
            {
                if (!tileBestState.TryGetValue(tile, out LetterState state))
                {
                    char c = tile.GetLetter();
                    if (c != ' ' && c != '\0')
                    {
                        tile.SetColor(defaultColor);
                    }
                    else
                    {
                        tile.SetColor(incorrectColor);
                    }

                    continue;
                }

                switch (state)
                {
                    case LetterState.Correct:
                        tile.SetColor(correctColor);
                        break;
                    case LetterState.Present:
                        tile.SetColor(presentColor);
                        break;
                    case LetterState.Absent:
                    default:
                        tile.SetColor(incorrectColor);
                        break;
                }
            }

            if (!anyEmpty && allWordsSolved)
            {
                Debug.Log("Word Knitter: All tiles are correct.");
                _isGameOver = true;
                door.IsInteractable = true;
            }
            else if (!allWordsSolved)
            {
                Debug.Log("Word Knitter: Some tiles are incorrect.");
            }
            else if (anyEmpty)
            {
                Debug.Log("Word Knitter: Some tiles are empty.");
            }
        }


        // ---------------------------
        // REGISTER KEY TO PUZZLE
        // ---------------------------
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
            _wordKnitterFocus = true;
            _wordleFocus = false;
        }

        public void WordleFocusButton()
        {
            SetFocus(Focus.Wordle);
            _wordleFocus = true;
            _wordKnitterFocus = false;
        }
    }
}