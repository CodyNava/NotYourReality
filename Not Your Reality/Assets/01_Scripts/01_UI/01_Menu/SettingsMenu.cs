using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Space]
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
    
    /*[SerializeField] private Volume volume;
    [SerializeField] private Toggle motionBlurToggle;
    private MotionBlur _motionBlur;
    private int _motionBlurInt;*/
    
    //private const string MotionBlur = "Motion Blur";
    private const string Vsync = "Vsync";
    private const string Fullscreen = "Fullscreen";

    /*private void Awake()
    {
        volume.profile.TryGet(out _motionBlur);
    }*/

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
            {
                continue;
            }

            _resOptions.Add(_resOption);
            _uniqueRes.Add(_resolutions[i]);
            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                _resIndex = i;
            }
        }

        resDropDown.AddOptions(_resOptions);
        resDropDown.value = _resIndex;
        resDropDown.RefreshShownValue();

        LoadSettings();
    }

    #region Graphics
    public void SetResolution(int resIndex)
    {
        var resolution = _uniqueRes[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenMode: FullScreenMode.FullScreenWindow);
    }

    public void SetFullscreen()
    {
        _fullScreenInt = fullScreenToggle.isOn ? 1 : 0;
        Screen.fullScreen = fullScreenToggle.isOn;
        SaveSettings();
    }

    public void SetVsync()
    {
        _vSyncInt = vSyncToggle.isOn ? 1 : 0;
        QualitySettings.vSyncCount = _vSyncInt;
        SaveSettings();
    }

    /*public void SetMotionBlur()
    {
        _motionBlurInt = motionBlurToggle.isOn ? 1 : 0;
        _motionBlur.active = motionBlurToggle.isOn;
        SaveSettings();
    }*/
    #endregion
    
    private void SaveSettings()
    {
        //PlayerPrefs.SetInt(MotionBlur, _motionBlurInt);
        PlayerPrefs.SetInt(Vsync, _vSyncInt);
        PlayerPrefs.SetInt(Fullscreen, _fullScreenInt);
    }
    
    private void LoadSettings()
    {
        /*_motionBlurInt = PlayerPrefs.GetInt(MotionBlur, 1);
        motionBlurToggle.SetIsOnWithoutNotify(_motionBlurInt == 1);
        SetMotionBlur();*/
        
        _vSyncInt = PlayerPrefs.GetInt(Vsync, 1);
        vSyncToggle.SetIsOnWithoutNotify(_vSyncInt == 1);
        SetVsync();
        
        _fullScreenInt = PlayerPrefs.GetInt(Fullscreen, 1);
        fullScreenToggle.SetIsOnWithoutNotify(_fullScreenInt == 1);
        SetFullscreen();
    }
}