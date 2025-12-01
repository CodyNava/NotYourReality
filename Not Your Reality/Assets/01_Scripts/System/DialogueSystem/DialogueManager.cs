using System.Collections;
using System.DialogueSystem.SO;
using UnityEngine;

namespace System.DialogueSystem
{
   public class DialogueManager : MonoBehaviour
   {
      [Header("Refs")]
      //todo audio
      [SerializeField] private TextRenderer textRenderer;
      [SerializeField] private DialogueSequence sequenceData;
      [SerializeField] private float delayBetweenLines;

      [SerializeField] private bool playOnStart;

      private DialogueSequence _currentSequence;
      private int _currentSequenceIndex;
      private bool _isPlaying;
      private Coroutine _runningCoroutine;

      public void Start()
      {
         if (playOnStart && sequenceData) { PlaySequence(sequenceData); }
      }

      public void PlaySequence(DialogueSequence sequence)
      {
         if (!sequence) { return; }

         if (_runningCoroutine != null)
         {
            StopCoroutine(_runningCoroutine);
            _runningCoroutine = null;
         }

         _currentSequence = sequence;
         _currentSequenceIndex = 0;
         _isPlaying = true;

         PlayCurrentLine();
      }

      private void Stop()
      {
         _isPlaying = false;
         if (_runningCoroutine != null)
         {
            StopCoroutine(_runningCoroutine);
            _runningCoroutine = null;
         }

         //todo sound stop here

         if (textRenderer) { textRenderer.HideSubtitle(); }
      }

      private void PlayCurrentLine()
      {
         if (!_isPlaying || !_currentSequence || _currentSequenceIndex >= _currentSequence.dialogueTexts.Length)
         {
            Stop();
            return;
         }

         var line = _currentSequence.dialogueTexts[_currentSequenceIndex];
         if (!line)
         {
            _currentSequenceIndex++;
            PlayCurrentLine();
            return;
         }

         if (textRenderer) textRenderer.ShowSubtitle(line);

         float duration = CalculateLineDuration(line);

         //todo sound here

         if (_runningCoroutine != null) { StopCoroutine(_runningCoroutine); }

         _runningCoroutine = StartCoroutine(LineRoutine(duration));
      }

      private float CalculateLineDuration(DialogueText line) { return Mathf.Max(line.duration, 0.01f); }

      private IEnumerator LineRoutine(float duration)
      {
         yield return new WaitForSeconds(duration);
         
         if (textRenderer) textRenderer.ClearInstant();
         if (delayBetweenLines > 0) yield return new WaitForSeconds(delayBetweenLines);
         
         _currentSequenceIndex++;
         PlayCurrentLine();
      }
   }
}