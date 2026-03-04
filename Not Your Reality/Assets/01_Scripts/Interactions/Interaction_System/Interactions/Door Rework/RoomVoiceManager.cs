using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions.Door_Rework
{
    public class RoomVoiceManager : MonoBehaviour
    {
        [Header("Normal Voice Colliders")]
        [SerializeField] private List<GameObject> normalVoices;

        [Header("Use Final Voice?")]
        [SerializeField] private bool useFinalVoice = true;

        [Header("Final Voice Collider")]
        [SerializeField] private GameObject finalVoice;
        [SerializeField] private EventReference finalVoiceEvent;

        [Header("Door")]
        [SerializeField] private DoorHandle doorHandle;

        [Header("Door Unlock Delay")]
        [SerializeField] private float doorUnlockDelay = 0.5f;

        [Header("Audio Reference")]
        [SerializeField] private EventReference unlockSound;

        private readonly HashSet<GameObject> _playedVoices = new();

        private bool _finalPlaying;
        private bool _doorUnlocked ;

        private EventInstance _finalInstance;

        private void Awake()
        {
            if (useFinalVoice && finalVoice != null)
            {
                var col = finalVoice.GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;
            }
            
        }

        public void OnVoiceTriggered(GameObject voiceObject)
        {
            if (normalVoices.Contains(voiceObject))
            {
                if (_playedVoices.Contains(voiceObject))
                    return;

                _playedVoices.Add(voiceObject);

                var emitter = voiceObject.GetComponent<StudioEventEmitter>();
                if (emitter != null)
                    emitter.Play();

                if (_playedVoices.Count >= normalVoices.Count)
                {
                    if (useFinalVoice && finalVoice != null)
                    {
                        var col = finalVoice.GetComponent<Collider>();
                        if (col != null)
                            col.enabled = true;
                    }
                    else
                    {
                        UnlockDoor();
                    }
                }
            }
            else if (useFinalVoice && voiceObject == finalVoice && !_finalPlaying)
            {
                _finalPlaying = true;

                _finalInstance = RuntimeManager.CreateInstance(finalVoiceEvent);
                _finalInstance.start();
                _finalInstance.release();
            }
        }

        private void Update()
        {
            if (!useFinalVoice || !_finalPlaying || _doorUnlocked)
                return;

            _finalInstance.getPlaybackState(out var state);

            if (state == PLAYBACK_STATE.STOPPED)
            {
                _finalPlaying = false;
                UnlockDoor();
            }
        }

        private void UnlockDoor()
        {
            if (_doorUnlocked)
                return;

            _doorUnlocked = true;
            StartCoroutine(UnlockDoorDelayed());
        }

        private IEnumerator UnlockDoorDelayed()
        {
            yield return new WaitForSeconds(doorUnlockDelay);

            if (doorHandle != null)
                doorHandle.IsInteractable = true;

            RuntimeManager.PlayOneShot(unlockSound, transform.position);
        }
    }
}