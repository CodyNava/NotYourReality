using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class LampSounds : MonoBehaviour
{
    [SerializeField] private EventReference lampBuzz;

    private EventInstance eventInstance;

    private void Start()
    {
        Buzz();
    }

    public void Buzz()
    {
        eventInstance = RuntimeManager.CreateInstance(lampBuzz.Guid);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        eventInstance.start();
    }
}