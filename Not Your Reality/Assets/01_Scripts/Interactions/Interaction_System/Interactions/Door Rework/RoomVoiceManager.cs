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

        private HashSet<GameObject> playedVoices = new HashSet<GameObject>();

        private bool finalPlaying = false;
        private bool doorUnlocked = false;

        private EventInstance finalInstance;

        private void Awake()
        {
            if (useFinalVoice && finalVoice != null)
            {
                var col = finalVoice.GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;
            }

            if (doorHandle != null)
                doorHandle.IsInteractable = false;
        }

        public void OnVoiceTriggered(GameObject voiceObject)
        {
            if (normalVoices.Contains(voiceObject))
            {
                if (playedVoices.Contains(voiceObject))
                    return;

                playedVoices.Add(voiceObject);

                var emitter = voiceObject.GetComponent<StudioEventEmitter>();
                if (emitter != null)
                    emitter.Play();

                if (playedVoices.Count >= normalVoices.Count)
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
            else if (useFinalVoice && voiceObject == finalVoice && !finalPlaying)
            {
                finalPlaying = true;

                finalInstance = RuntimeManager.CreateInstance(finalVoiceEvent);
                finalInstance.start();
                finalInstance.release();
            }
        }

        private void Update()
        {
            if (!useFinalVoice || !finalPlaying || doorUnlocked)
                return;

            finalInstance.getPlaybackState(out var state);

            if (state == PLAYBACK_STATE.STOPPED)
            {
                finalPlaying = false;
                UnlockDoor();
            }
        }

        private void UnlockDoor()
        {
            if (doorUnlocked)
                return;

            doorUnlocked = true;
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