using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Puzzle.Wordle
{
   public class WordListManager : MonoBehaviour
   {
      [Header("Word Data")]
      public TextAsset wordList;

      [HideInInspector] public List<string> wordListData = new();
      [HideInInspector] public string targetWord;

      private void Awake()
      {
         if (wordList)
         {
            wordListData = wordList.text.Split('\n').Select(word => word.Trim().ToUpper())
                                   .Where(word => word.Length == 5).ToList();
         }
         else
         {
            wordListData = new()
            {
               "UNITY",
               "PHONE",
               "APPLE",
               "HOUSE",
               "WORDS"
            };
         }

         targetWord = wordListData[UnityEngine.Random.Range(0, wordListData.Count)];
         Debug.Log(targetWord);
      }
   }
}