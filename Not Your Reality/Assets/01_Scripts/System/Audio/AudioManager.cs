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
            _birthdayRoomInstance = RuntimeManager.CreateInstance(birthdayRoom);
            _tvRoomInstance = RuntimeManager.CreateInstance(tvRoom);
            _bathroomInstance = RuntimeManager.CreateInstance(bathroom);
            _pngRoomInstance = RuntimeManager.CreateInstance(pngRoom);
            _desertInstance = RuntimeManager.CreateInstance(desert);
            _bedroomR1Instance = RuntimeManager.CreateInstance(bedroomR1);
            _bedroomR2Instance = RuntimeManager.CreateInstance(bedroomR2);
            _bedroomR3Instance = RuntimeManager.CreateInstance(bedroomR3);
            _bedroomR4Instance = RuntimeManager.CreateInstance(bedroomR4);
            _basementInstance = RuntimeManager.CreateInstance(basement);
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

        public void PlayBirthdayRoom()
        {
            _birthdayRoomInstance.start();
        }

        public void stopBirthdayRoom()
        {
            _birthdayRoomInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayTVRoom()
        {
            _tvRoomInstance.start();
        }

        public void stopTVRoom()
        {
            _tvRoomInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayBathroom()
        {
            _bathroomInstance.start();
        }

        public void stopBathroom()
        {
            _bathroomInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayPngRoom()
        {
            _pngRoomInstance.start();
        }

        public void stopPngRoom()
        {
            _pngRoomInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayDesert()
        {
            _desertInstance.start();
        }

        public void stopDesert()
        {
            _desertInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayBedroomR1()
        {
            _bedroomR1Instance.start();
        }

        public void stopBedroomR1()
        {
            _bedroomR1Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            PlayBedroomR2();
        }

        public void PlayBedroomR2()
        {
            _bedroomR2Instance.start();
        }

        public void stopBedroomR2()
        {
            _bedroomR2Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            PlayBedroomR3();
        }

        public void PlayBedroomR3()
        {
            _bedroomR3Instance.start();
        }

        public void stopBedroomR3()
        {
            _bedroomR3Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            PlayBedroomR4();
        }

        public void PlayBedroomR4()
        {
            _bedroomR4Instance.start();
        }

        public void stopBedroomR4()
        {
            _bedroomR4Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void PlayBasement()
        {
            _basementInstance.start();
        }

        public void stopBasement()
        {
            _basementInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}