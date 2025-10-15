using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private Resolution[] _resolutions;
    private List<Resolution> _uniqueRes;
    private List<string> _resOptions;
    private string _resOption;
    private int _resIndex;
    [SerializeField] private TMP_Dropdown resDropDown;
    
    private void Start()
    {
        QualitySettings.vSyncCount = 1;
        
        _resOptions = new List<string>();
        _uniqueRes = new List<Resolution>();
        _resolutions = Screen.resolutions;
        resDropDown.ClearOptions();

        for (int i = 0; i < _resolutions.Length; i++)
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
    }

    public void SetResolution(int resIndex)
    {
        var resolution = _uniqueRes[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenMode: FullScreenMode.FullScreenWindow);
    }

    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetVsync(bool isVsync)
    {
        QualitySettings.vSyncCount = isVsync ? 1 : 0;
    }
}