using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace System.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField] private EventReference mainMenu;
        [SerializeField] private EventReference credits;
        [SerializeField] private EventReference birthdayRoom;
        [SerializeField] private EventReference tvRoom;
        [SerializeField] private EventReference bathroom;
        [SerializeField] private EventReference pngRoom;
        [SerializeField] private EventReference desert;
        [SerializeField] private EventReference bedroomR1;
        [SerializeField] private EventReference bedroomR2;
        [SerializeField] private EventReference bedroomR3;
        [SerializeField] private EventReference bedroomR4;
        [SerializeField] private EventReference basement;

        private EventInstance _mainMenuInstance;
        private EventInstance _creditsInstance;
        private EventInstance _birthdayRoomInstance;
        private EventInstance _tvRoomInstance;
        private EventInstance _bathroomInstance;
        private EventInstance _pngRoomInstance;
        private EventInstance _desertInstance;
        private EventInstance _bedroomR1Instance;
        private EventInstance _bedroomR2Instance;
        private EventInstance _bedroomR3Instance;
        private EventInstance _bedroomR4Instance;
        private EventInstance _basementInstance;

        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            _mainMenuInstance = RuntimeManager.CreateInstance(mainMenu);
            _creditsInstance = RuntimeManager.CreateInstance(credits);
        }

        public void PlayMainMenu()
        {
            _mainMenuInstance.start();
        }

        public void StopMainMenu()
        {
            _mainMenuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayCredits()
        {
            _creditsInstance.start();
        }

        public void stopCredits()
        {
            _creditsInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayBasement()
        {
            _basementInstance.start();
        }
    }
}