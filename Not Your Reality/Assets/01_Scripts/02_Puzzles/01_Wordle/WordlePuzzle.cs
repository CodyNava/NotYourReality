using System.Collections.Generic;
using UnityEngine;

public class WordlePuzzle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WordListManager wordList;

    [SerializeField] private Transform wordleBoard;
    [SerializeField] private int maxGuesses = 6;

    [Header("Colors")]
    [SerializeField] private Color correctColor = Color.green;

    [SerializeField] private Color presentColor = Color.yellow;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;

    private int _currentGuess = 0;
    private string _currentInput = "";
    private bool _isGameOver = false;

    private List<List<LetterTile>> _board = new();

    private void Start()
    {
        foreach (Transform row in wordleBoard)
        {
            List<LetterTile> rowTiles = new();
            foreach (Transform tileobj in row)
            {
                var tile = tileobj.GetComponent<LetterTile>();
                if (tile != null)
                {
                    rowTiles.Add(tile);
                    tile.SetLetter(' ');
                    tile.SetColor(defaultColor);
                }
            }

            _board.Add(rowTiles);
        }
    }

    public void OnKeyboardClick(char letter)
    {
        Debug.Log("Key Got Clicked: " + letter + "");
        if (_isGameOver) return;
        if (_currentInput.Length < 5)
        {
            _currentInput += letter;
            UpdateCurrentRow();
        }
    }

    public void OnSpecialKey(string action)
    {
        if (_isGameOver) return;

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

    public void UpdateCurrentRow()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < _currentInput.Length)
            {
                _board[_currentGuess][i].SetLetter(_currentInput[i]);
            }
            else
            {
                _board[_currentGuess][i].SetLetter(' ');
            }
        }
    }

    public void SubmitGuess()
    {
        if (_currentInput.Length != 5)
        {
            Debug.Log("Guess must be 5 characters long");
            return;
        }

        LetterState[] feedback = CheckGuess(_currentInput, wordList.targetWord);

        for (int i = 0; i < 5; i++)
        {
            Color color = feedback[i] switch
            {
                LetterState.Correct => correctColor,
                LetterState.Present => presentColor,
                _ => incorrectColor
            };
            _board[_currentGuess][i].SetColor(color);
        }

        if (_currentInput.ToUpper() == wordList.targetWord)
        {
            Debug.Log("You Win");
            _isGameOver = true;
            return;
        }

        _currentGuess++;
        _currentInput = "";

        if (_currentGuess >= maxGuesses)
        {
            Debug.Log("You Lose");
            _isGameOver = true;
        }
    }

    private enum LetterState
    {
        Correct,
        Present,
        Absent
    }

    private static LetterState[] CheckGuess(string guess, string target)
    {
        guess = guess.ToUpper();
        target = target.ToUpper();

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
}