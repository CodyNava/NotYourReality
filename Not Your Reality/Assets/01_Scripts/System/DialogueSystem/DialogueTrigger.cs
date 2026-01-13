using System.DialogueSystem.SO;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;

namespace System.DialogueSystem
{
   public class DialogueTrigger : MonoBehaviour
   {
      [SerializeField] private DialogueSequence sequence;
      [SerializeField] private DialogueManager dialogueManager;
      [SerializeField] private StudioEventEmitter eventEmitter;
      private bool _wasPlaying;

      private void Reset() { eventEmitter = GetComponent<StudioEventEmitter>(); }

      private void Update() { Trigger(); }

      private void Trigger()
      {
         if (!eventEmitter || !dialogueManager || !sequence) return;
         var inst = eventEmitter.EventInstance;

         if (!inst.isValid()) return;

         inst.getPlaybackState(out PLAYBACK_STATE playbackState);
         bool isPlayingOrStarting = playbackState == PLAYBACK_STATE.PLAYING || playbackState == PLAYBACK_STATE.STARTING;

         if (!_wasPlaying && isPlayingOrStarting)
         {
            _wasPlaying = true;
            dialogueManager.PlaySequenceWithEmitter(sequence, eventEmitter);
         }

         if (_wasPlaying && playbackState == PLAYBACK_STATE.STOPPED)
         {
            _wasPlaying = false;
            dialogueManager.Stop();
            //todo this.enabled = false;
         }
      }
   }
}