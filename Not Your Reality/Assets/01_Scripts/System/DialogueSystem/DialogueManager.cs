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
      private EventInstance _inst;

      private EVENT_CALLBACK _cb;
      private GCHandle _handle;
      
      private int _part1StartMs = -1;
      private int _part2StartMs = -1;

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
         
         if (_part1StartMs >= 0 && _part2StartMs >= 0)
         {
            int part1Len = _part2StartMs - _part1StartMs;
            Debug.Log($"[Dialogue] Part1 Länge = {part1Len} ms");
         }

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
            _index++;

            if (_seq != null && _seq.dialogueTexts != null && _index < _seq.dialogueTexts.Length)
            {
               var next = _seq.dialogueTexts[_index];
               
               if (next && (!string.IsNullOrWhiteSpace(next.showOnMarker) && next.showOnMarker == markerName))
               {
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
         if (textRenderer) textRenderer.ClearInstant();
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
         _isPlaying = false;

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

         _part1StartMs = -1;
         _part2StartMs = -1;

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