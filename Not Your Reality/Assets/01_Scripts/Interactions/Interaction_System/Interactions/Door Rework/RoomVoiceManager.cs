using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using Interactions.Interaction_System.Interactions.Door_Rework;
using System.Collections;

public class RoomVoiceManager : MonoBehaviour
{
    [Header("Normal Voice Colliders (1–7)")]
    [SerializeField] private List<GameObject> normalVoices;

    [Header("Final Voice Collider (8)")]
    [SerializeField] private GameObject finalVoice;
    [SerializeField] private EventReference finalVoiceEvent;

    [Header("Door")]
    [SerializeField] private DoorHandle doorHandle;

    [Header("Door Unlock Delay")]
    [SerializeField] private float doorUnlockDelay = 0.5f;


    [Header("Audio Reference")]
    [Tooltip("The sound that is played when you unlock a door")]
    [SerializeField] private EventReference unlockSound;

    private HashSet<GameObject> playedVoices = new HashSet<GameObject>();
    private bool finalPlaying = false;
    private EventInstance finalInstance;

    private void Awake()
    {
        if (finalVoice != null)
        {
            var col = finalVoice.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        if (doorHandle != null)
            doorHandle.IsInteractable = false;
    }

    public void OnVoiceTriggered(GameObject voiceObject)
    {
        if (normalVoices.Contains(voiceObject))
        {
            if (!playedVoices.Contains(voiceObject))
            {
                var emitter = voiceObject.GetComponent<StudioEventEmitter>();
                if (emitter != null)
                    emitter.Play();

                playedVoices.Add(voiceObject);
            }

            if (playedVoices.Count >= normalVoices.Count && finalVoice != null)
            {
                var col = finalVoice.GetComponent<Collider>();
                if (col != null) col.enabled = true;
            }
        }
        else if (voiceObject == finalVoice && !finalPlaying)
        {
            finalPlaying = true;
            finalInstance = RuntimeManager.CreateInstance(finalVoiceEvent);
            finalInstance.start();
            finalInstance.release();
        }
    }

    private void Update()
    {
        if (!finalPlaying) return;

        finalInstance.getPlaybackState(out var state);
        if (state == PLAYBACK_STATE.STOPPED)
        {
            finalPlaying = false;
            StartCoroutine(UnlockDoorDelayed());
        }
    }


    private IEnumerator UnlockDoorDelayed()
    {
        yield return new WaitForSeconds(doorUnlockDelay);

        if (doorHandle != null)
            doorHandle.IsInteractable = true;

        RuntimeManager.PlayOneShot(unlockSound, transform.position);
    }

}
