using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace UI.Menu
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Graphics")] [SerializeField] private TMP_Dropdown resDropDown;
        private Resolution[] _resolutions;
        private List<Resolution> _uniqueRes;
        private List<string> _resOptions;
        private string _resOption;
        private int _resIndex;

        [SerializeField] private TMP_Dropdown qualityDropDown;
        [SerializeField] private List<RenderPipelineAsset> rpa;
        private List<string> _qualityOptions;
        private int _qualityIndex;

        [SerializeField] private Toggle fullScreenToggle;
        private int _fullScreenInt;

        [SerializeField] private Toggle vSyncToggle;
        private int _vSyncInt;

        [SerializeField] private Volume volume;
        [SerializeField] private Toggle motionBlurToggle;
        [SerializeField] private Slider gammaSlider;
        private LiftGammaGain _gamma;
        private MotionBlur _motionBlur;
        private int _motionBlurInt;

        [Header("Audio")]
        [SerializeField] private Slider masterSlider;
        private Bus _masterBus;

        [SerializeField] private Slider musicSlider;
        private Bus _musicBus;
    
        [SerializeField] private Slider sfxSlider;
        private Bus _sfxBus;

        [SerializeField] private Slider voiceSlider;
        private Bus _voiceBus;

        [SerializeField] private Slider ambientSlider;
        private Bus _ambientBus;

        private const string MotionBlur = "Motion Blur";
        private const string Vsync = "Vsync";
        private const string Fullscreen = "Fullscreen";
        private const string ResolutionIndex = "ResolutionIndex";
        private const string QualityIndex = "QualityIndex";
        private const string GammaValue = "GammaValue";
        private const string Master = "Master";
        private const string Music = "Music";
        private const string Sfx = "Sfx";
        private const string Voice = "Voice";
        private const string Ambience = "Ambience";

        private void Awake()
        {
            _masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            _musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
            _sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            _voiceBus = FMODUnity.RuntimeManager.GetBus("bus:/Voice");
            _ambientBus = FMODUnity.RuntimeManager.GetBus("bus:/Ambient");
            volume.profile.TryGet(out _motionBlur);
            volume.profile.TryGet(out _gamma);
        }

        private void Start()
        {
            _resOptions = new List<string>();
            _uniqueRes = new List<Resolution>();
            _resolutions = Screen.resolutions;
            _qualityOptions = new List<string>();
            resDropDown.ClearOptions();
            qualityDropDown.ClearOptions();

            for (var i = 0; i < _resolutions.Length; i++)
            {
                _resOption = $"{_resolutions[i].width} x {_resolutions[i].height}";
                if (_resOptions.Contains(_resOption))
                    continue;

                _resOptions.Add(_resOption);
                _uniqueRes.Add(_resolutions[i]);
            }

            resDropDown.AddOptions(_resOptions);

            foreach (var option in rpa)
            {
                _qualityOptions.Add(option.name);
            }
            
            qualityDropDown.AddOptions(_qualityOptions);

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

        public void SetQuality(int qualityIndex)
        {
            _qualityIndex = qualityIndex;
            QualitySettings.renderPipeline = rpa[_qualityIndex];
            PlayerPrefs.SetInt(QualityIndex, _qualityIndex);
        }

        public void SetFullscreen()
        {
            _fullScreenInt = fullScreenToggle.isOn ? 1 : 0;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
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

        public void AdjustGamma()
        {
            _gamma.gamma.value = new Vector4(1f, 1f, 1f, gammaSlider.value);
        }
        #endregion

        #region Sound
        public void MasterVolume()
        {
            ApplyVolume(_masterBus, masterSlider.value);
        }

        public void MusicVolume()
        {
            ApplyVolume(_musicBus, musicSlider.value);
        }

        public void SfxVolume()
        {
            ApplyVolume(_sfxBus, sfxSlider.value);
        }

        public void VoiceVolume()
        {
            ApplyVolume(_voiceBus, voiceSlider.value);
        }

        public void AmbienceVolume()
        {
            ApplyVolume(_ambientBus, ambientSlider.value);
        }

        private void ApplyVolume(Bus bus, float newValue)
        {
            bus.setVolume(newValue);
            Save();
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
            
            _qualityIndex = PlayerPrefs.GetInt(QualityIndex, 1);
            qualityDropDown.SetValueWithoutNotify(_qualityIndex);
            
            gammaSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(GammaValue, 0f));

            masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(Master, 0.5f));
            musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(Music, 0.5f));
            sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(Sfx, 0.5f));
            voiceSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(Voice, 0.5f));
            ambientSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(Ambience, 0.5f));
        }

        private void ApplySettings()
        {
            SetResolution(_resIndex);
            SetQuality(_qualityIndex);
            SetFullscreen();
            SetVsync();
            SetMotionBlur();
            AdjustGamma();
            MasterVolume();
            MusicVolume();
            SfxVolume();
            VoiceVolume();
            AmbienceVolume();
        }

        private void Save()
        {
            PlayerPrefs.SetFloat(Master, masterSlider.value);
            PlayerPrefs.SetFloat(Music, musicSlider.value);
            PlayerPrefs.SetFloat(Sfx, sfxSlider.value);
            PlayerPrefs.SetFloat(Voice, voiceSlider.value);
            PlayerPrefs.SetFloat(Ambience, ambientSlider.value);
        }
    }
}