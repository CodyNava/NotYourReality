using System;
using System.Collections.Generic;
using FMODUnity;
using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Puzzle.Wordle
{
    public class WordlePuzzle : MonoBehaviour
    {
        private readonly Dictionary<char, KeyboardButton> _keyboardButtons = new();

        [Header("References")]
        [SerializeField] private WordListManager wordList;

        [SerializeField] private Transform wordleBoard;
        [SerializeField] private int maxGuesses = 6;
        [SerializeField] private List<ItemLetterInteract> letterItems;
        [SerializeField] private int amountFakeLetters = 2;

        [Header("Audio References")]
        [SerializeField] private EventReference keyboardSound;

        [SerializeField] private EventReference allCorrectSound;
        [SerializeField] private EventReference coloringSound;


        [Header("Colors")]
        [SerializeField] private Color correctColor = Color.green;

        [SerializeField] private Color presentColor = Color.yellow;
        [SerializeField] private Color incorrectColor = Color.red;
        [SerializeField] private Color defaultColor = Color.white;

        [Header("Animation")]
        [SerializeField] private float revealDuration = 1.5f;

        [SerializeField] private DoorHandle door;

        private bool _isRevealing;
        private float _perLetterDelay;

        private int _currentGuess;
        private string _currentInput;
        private bool _isGameOver;

        private int _randomIntForChar;
        private char _randomChar;
        private string _randomLetter;
        private int _randomIndex;


        private readonly List<List<LetterTile>> _board = new();
        private readonly List<int> _randomItems = new List<int>();

        // ---------------------------
        // UNITY LIFECYCLE
        // ---------------------------
        private void Start()
        {
            ResetWordle();
            //door.IsInteractable = false;
            Debug.Log("word is :" + wordList.targetWord);
            foreach (char c in wordList.targetWord)
            {
                SetRandomCharToItem(c);
            }

            for (int i = 0; i < 2; i++)
            {
                _randomIntForChar = Random.Range('a', 'z');
                _randomChar = Convert.ToChar(_randomIntForChar);
                _randomLetter = Convert.ToString(_randomChar).ToUpper();

                _randomIndex = Random.Range(0, letterItems.Count);
                
                SetFakeCharToItem(_randomLetter, _randomIndex);
            }
        }


        private void SetRandomCharToItem(char c)
        {
            int random = Random.Range(0, letterItems.Count);
            if (!_randomItems.Contains(random))
            {
                letterItems[random].LetterChar = c;
                _randomItems.Add(random);
            }
            else
            {
                SetRandomCharToItem(c);
            }
        }

        private void SetFakeCharToItem(string randomLetter, int randomNumber)
        {
            Debug.Log("random char is :" + randomLetter);
            if (_randomItems.Contains(randomNumber))
            {
                randomNumber = Random.Range(0, letterItems.Count);
                SetFakeCharToItem(randomLetter, randomNumber);
            }
            else if (wordList.targetWord.Contains(randomLetter))
            {
                _randomIntForChar = Random.Range('a', 'z');
                _randomChar = Convert.ToChar(_randomIntForChar);
                randomLetter = Convert.ToString(_randomChar).ToUpper();
                SetFakeCharToItem(randomLetter, randomNumber);
            }
            else
            {
                letterItems[randomNumber].LetterChar = randomLetter[0];
                _randomItems.Add(randomNumber);
            }
        }

        // ---------------------------
        // RESET PUZZLE BOARD
        // ---------------------------
        private void ResetWordle()
        {
            _currentGuess = 0;
            _isGameOver = false;
            _currentInput = "";

            _board.Clear();

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

                _board.Add(rowTiles);
            }
        }

        // ---------------------------
        // KEYBOARD INPUT
        // ---------------------------
        public void OnKeyboardClick(char letter)
        {
            if (_isGameOver) return;

            if (_currentInput.Length < 5)
            {
                _currentInput += letter;
                UpdateCurrentRow();
            }

            RuntimeManager.PlayOneShot(keyboardSound, transform.position);
        }

        public void OnSpecialKey(string action)
        {
            if (_isGameOver) return;

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

            RuntimeManager.PlayOneShot(keyboardSound, transform.position);
        }

        private void UpdateCurrentRow()
        {
            for (var i = 0; i < 5; i++)
            {
                _board[_currentGuess][i].SetLetter(i < _currentInput.Length ? _currentInput[i] : ' ');
            }
        }

        // ---------------------------
        // WORDLE GUESS LOGIC
        // ---------------------------
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
                    _board[_currentGuess][i].SetColor(color);
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
                door.IsInteractable = true;

                return;
            }

            _currentGuess++;
            _currentInput = "";

            if (_currentGuess >= maxGuesses)
            {
                Debug.Log("You Lose");
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
        // REGISTER KEY TO PUZZLE
        // ---------------------------
        public void RegisterKeyboardKey(KeyboardButton key)
        {
            char letter = char.ToUpper(key.buttonLetter.text[0]);
            if (!_keyboardButtons.ContainsKey(letter))
                _keyboardButtons.Add(letter, key);
        }

        public void KeyEnable(char c)
        {
            _keyboardButtons[c].ActivateLetter();
        }
    }
}