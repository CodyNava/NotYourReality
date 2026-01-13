using System.DialogueSystem.SO;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace System.DialogueSystem
{
   public class DialogueTrigger : MonoBehaviour
   {
      [Header("Dialogue")]
      [SerializeField] private DialogueSequence sequence;
      [SerializeField] private DialogueManager dialogueManager;

      [Header("FMOD")]
      [SerializeField] private StudioEventEmitter eventEmitter;

      private PLAYBACK_STATE _lastState = PLAYBACK_STATE.STOPPED;
      private bool _hookedForThisPlay = false;

      private void Reset()
      {
         eventEmitter = GetComponent<StudioEventEmitter>();
      }
      
      private void Update()
      {
         Trigger();
      }

      private void Trigger()
      {
         if (!sequence || !dialogueManager || !eventEmitter) return;

         var inst = eventEmitter.EventInstance;
         
         if (!inst.isValid())
         {
            _lastState = PLAYBACK_STATE.STOPPED;
            _hookedForThisPlay = false;
            return;
         }

         inst.getPlaybackState(out var state);

         bool isStartingOrPlaying = state == PLAYBACK_STATE.STARTING || state == PLAYBACK_STATE.PLAYING;
         bool wasStopped = _lastState == PLAYBACK_STATE.STOPPED;
         
         if (wasStopped && isStartingOrPlaying && !_hookedForThisPlay)
         {
            _hookedForThisPlay = true;
            dialogueManager.PlaySequenceWithEmitter(sequence, eventEmitter);
         }
         
         if (state == PLAYBACK_STATE.STOPPED)
         {
            if (_hookedForThisPlay)
            {
               dialogueManager.Stop();           // hide subs + detach callback
               _hookedForThisPlay = false;       // allow next play to hook again
            }
         }

         _lastState = state;
      }
      
   }
}