using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown resDropDown;
    private Resolution[] _resolutions;
    private List<Resolution> _uniqueRes;
    private List<string> _resOptions;
    private string _resOption;
    private int _resIndex;

    [SerializeField] private Toggle fullScreenToggle;
    private int _fullScreenInt;

    [SerializeField] private Toggle vSyncToggle;
    private int _vSyncInt;

    [SerializeField] private Volume volume;
    [SerializeField] private Toggle motionBlurToggle;
    private MotionBlur _motionBlur;
    private int _motionBlurInt;

    [Header("Audio")]
    [SerializeField] private Slider masterSlider;
    private Bus _masterBus; 

    private const string MotionBlur = "Motion Blur";
    private const string Vsync = "Vsync";
    private const string Fullscreen = "Fullscreen";
    private const string ResolutionIndex = "ResolutionIndex";
    private const string Master = "Master";

    private void Awake()
    {
        _masterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        volume.profile.TryGet(out _motionBlur);
    }

    private void Start()
    {
        _resOptions = new List<string>();
        _uniqueRes = new List<Resolution>();
        _resolutions = Screen.resolutions;
        resDropDown.ClearOptions();

        for (var i = 0; i < _resolutions.Length; i++)
        {
            _resOption = $"{_resolutions[i].width} x {_resolutions[i].height}";
            if (_resOptions.Contains(_resOption))
                continue;

            _resOptions.Add(_resOption);
            _uniqueRes.Add(_resolutions[i]);
        }

        resDropDown.AddOptions(_resOptions);
        
        resDropDown.value = _resIndex; resDropDown.RefreshShownValue();

        LoadSettings();
        ApplySettings();
    }

    #region Graphics
    public void SetResolution(int resIndex)
    {
        _resIndex = resIndex;
        var resolution = _uniqueRes[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, _fullScreenInt == 1);
        PlayerPrefs.SetInt(ResolutionIndex, _resIndex);
    }

    public void SetFullscreen()
    {
        _fullScreenInt = fullScreenToggle.isOn ? 1 : 0;
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.fullScreen = fullScreenToggle.isOn;
        PlayerPrefs.SetInt(Fullscreen, _fullScreenInt);
    }

    public void SetVsync()
    {
        _vSyncInt = vSyncToggle.isOn ? 1 : 0;
        QualitySettings.vSyncCount = _vSyncInt;
        PlayerPrefs.SetInt(Vsync, _vSyncInt);
    }

    public void SetMotionBlur()
    {
        _motionBlurInt = motionBlurToggle.isOn ? 1 : 0;
        _motionBlur.active = motionBlurToggle.isOn;
        PlayerPrefs.SetInt(MotionBlur, _motionBlurInt);
    }
    #endregion

    #region Sound
    public void MasterVolume()
    {
        ApplyVolume(_masterBus, masterSlider.value);
    }

    private void ApplyVolume(Bus bus, float newValue)
    {
        bus.setVolume(newValue);
        PlayerPrefs.SetFloat(Master, newValue);
    }
    #endregion

    private void LoadSettings()
    {
        _motionBlurInt = PlayerPrefs.GetInt(MotionBlur, 1);
        motionBlurToggle.SetIsOnWithoutNotify(_motionBlurInt == 1);

        _vSyncInt = PlayerPrefs.GetInt(Vsync, 1);
        vSyncToggle.SetIsOnWithoutNotify(_vSyncInt == 1);

        _fullScreenInt = PlayerPrefs.GetInt(Fullscreen, 1);
        fullScreenToggle.SetIsOnWithoutNotify(_fullScreenInt == 1);

        _resIndex = PlayerPrefs.GetInt(ResolutionIndex, _uniqueRes.Count - 1);
        resDropDown.SetValueWithoutNotify(_resIndex);

        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(Master, 0.5f));
    }

    private void ApplySettings()
    {
        SetResolution(_resIndex);
        SetFullscreen();
        SetVsync();
        SetMotionBlur();
        MasterVolume();
    }
}
