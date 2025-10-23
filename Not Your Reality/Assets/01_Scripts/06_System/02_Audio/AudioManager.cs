using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private EventReference mainMenu;

    private EventInstance _mainMenuInstance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
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