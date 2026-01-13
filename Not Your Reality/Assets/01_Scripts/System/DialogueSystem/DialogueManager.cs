using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.DialogueSystem.SO;
using FMOD;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;

namespace System.DialogueSystem
{
   public class DialogueManager : MonoBehaviour
   {
      [Header("UI")]
      [SerializeField] private TextRenderer textRenderer;

      [Header("Flow")]
      [SerializeField] private float delayBetweenLines = 0f;

      private DialogueSequence _seq;
      private int _index;
      private bool _isPlaying;

      private StudioEventEmitter _emitter;
      private static StudioEventEmitter _currentlyPlayingEmitter;
      private EventInstance _inst;

      private EVENT_CALLBACK _cb;
      private GCHandle _handle;
      
      

      private volatile bool _markerQueued;
      private string _queuedMarkerName;
      private int _queuedMarkerPosMs;

      private volatile bool _stoppedQueued;

      private class CallbackState
      {
         public DialogueManager Owner;
      }

      public void PlaySequenceWithEmitter(DialogueSequence sequence, StudioEventEmitter emitter)
      {
         if (!sequence || !emitter) return;

         if (_currentlyPlayingEmitter && _currentlyPlayingEmitter != emitter)
         {
            _currentlyPlayingEmitter.Stop();
         }
         
        

         _currentlyPlayingEmitter = emitter;
         
         Stop();

         _seq = sequence;
         _index = 0;
         _isPlaying = true;

         _emitter = emitter;
         _inst = _emitter.EventInstance;

         if (!_inst.isValid())
         {
            Debug.LogWarning("Emitter EventInstance not valid");
            return;
         }

         _cb = FmodCallback;
         _handle = GCHandle.Alloc(new CallbackState { Owner = this }, GCHandleType.Normal);

         _inst.setUserData(GCHandle.ToIntPtr(_handle));
         _inst.setCallback(_cb, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.STOPPED);
         
         textRenderer?.HideSubtitle();
         TryShowCurrentIfNoMarker();
      }

      private void Update()
      {
         if (!_isPlaying) return;

         if (_stoppedQueued)
         {
            _stoppedQueued = false;
            Stop();
            return;
         }

         if (_markerQueued)
         {
            _markerQueued = false;
            HandleMarker(_queuedMarkerName, _queuedMarkerPosMs);
         }
      }

      private void HandleMarker(string markerName, int posMs)
      {
         

         if (_seq == null || _seq.dialogueTexts == null || _index >= _seq.dialogueTexts.Length) return;

         var line = _seq.dialogueTexts[_index];
         if (!line)
         {
            AdvanceLine();
            return;
         }
         
         if (!string.IsNullOrWhiteSpace(line.showOnMarker) && line.showOnMarker == markerName)
         {
            textRenderer?.ShowSubtitle(line);
         }
         
         if (!string.IsNullOrWhiteSpace(line.advanceOnMarker) && line.advanceOnMarker == markerName)
         {
            int nextIndex = _index + 1;

            if (_seq != null && _seq.dialogueTexts != null && _index < _seq.dialogueTexts.Length)
            {
               var next = _seq.dialogueTexts[nextIndex];
               
               if (next && !string.IsNullOrWhiteSpace(next.showOnMarker) && next.showOnMarker == markerName)
               {
                  _index = nextIndex;
                  textRenderer?.ShowSubtitle(next);
                  return;
               }
            }
            
            StartCoroutine(AdvanceRoutine());
         }
      }

      private void TryShowCurrentIfNoMarker()
      {
         if (_seq == null || _seq.dialogueTexts == null || _index >= _seq.dialogueTexts.Length) return;

         var line = _seq.dialogueTexts[_index];
         if (!line)
         {
            AdvanceLine();
            return;
         }

         if (string.IsNullOrWhiteSpace(line.showOnMarker))
         {
            textRenderer?.ShowSubtitle(line);

            if (string.IsNullOrWhiteSpace(line.advanceOnMarker) && line.duration > 0f)
               StartCoroutine(DurationFallback(line.duration));
         }
      }

      private IEnumerator AdvanceRoutine()
      {
            Debug.LogWarning("ClearInstantB");
         if (textRenderer)
         {
            Debug.LogWarning("ClearInstant");
            textRenderer.ClearInstant();
         }
         if (delayBetweenLines > 0f) yield return new WaitForSeconds(delayBetweenLines);
         
         AdvanceLine();
      }

      private void AdvanceLine()
      {
         _index++;

         if (_seq == null || _seq.dialogueTexts == null || _index >= _seq.dialogueTexts.Length)
         {
            Stop();
            return;
         }

         textRenderer?.HideSubtitle();
         TryShowCurrentIfNoMarker();
      }

      private IEnumerator DurationFallback(float seconds)
      {
         yield return new WaitForSeconds(seconds);
         yield return AdvanceRoutine();
      }

      public void Stop()
      {
         if (!_isPlaying && !_inst.isValid() && !_handle.IsAllocated)
         {
            textRenderer?.HideSubtitle();
            return;
         } 
         _isPlaying = false;
         _stoppedQueued = false;
         _markerQueued = false;
         
         if (_inst.isValid())
         {
            _inst.setCallback(null);
            _inst.setUserData(IntPtr.Zero);
         }

         if (_handle.IsAllocated) _handle.Free();

         _seq = null;
         _index = 0;
         _emitter = null;
         _inst.clearHandle();
         

         textRenderer?.HideSubtitle();
      }

      [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
      private static RESULT FmodCallback(EVENT_CALLBACK_TYPE type, IntPtr eventInstancePtr, IntPtr parameterPtr)
      {
         var inst = new EventInstance(eventInstancePtr);

         inst.getUserData(out var ud);
         if (ud == IntPtr.Zero) return RESULT.OK;

         var state = (CallbackState)GCHandle.FromIntPtr(ud).Target;
         var owner = state.Owner;

         if (type == EVENT_CALLBACK_TYPE.STOPPED)
         {
            owner._stoppedQueued = true;
            return RESULT.OK;
         }

         if (type == EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
         {
            var props = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(
               parameterPtr,
               typeof(TIMELINE_MARKER_PROPERTIES)
            );

            owner._queuedMarkerName = Marshal.PtrToStringAnsi(props.name);
            owner._queuedMarkerPosMs = props.position;
            owner._markerQueued = true;
         }

         return RESULT.OK;
      }
   }
}