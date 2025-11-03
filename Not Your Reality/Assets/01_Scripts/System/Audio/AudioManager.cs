using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace System.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;

        [SerializeField] private EventReference mainMenu;

        private EventInstance _mainMenuInstance;

        private void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _mainMenuInstance = RuntimeManager.CreateInstance(mainMenu);
        }

        public void PlayMainMenu()
        {
            _mainMenuInstance.start();
        }

        public void StopMainMenu()
        {
            _mainMenuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}