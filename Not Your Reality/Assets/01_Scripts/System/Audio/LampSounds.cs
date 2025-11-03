using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace System.Audio
{
    public class LampSounds : MonoBehaviour
    {
        [SerializeField] private EventReference lampBuzz;

        private EventInstance _eventInstance;

        private void Start()
        {
            Buzz();
        }

        public void Buzz()
        {
            _eventInstance = RuntimeManager.CreateInstance(lampBuzz.Guid);
            _eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            _eventInstance.start();
        }
    }
}