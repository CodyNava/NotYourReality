using System.DialogueSystem.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace System.DialogueSystem
{
    public class TextRenderer : MonoBehaviour
    {
        [Header("Root visibility")]
        [SerializeField] private CanvasGroup subtitleGroup;   // <- das ist dein “SubBox CanvasGroup”
        [SerializeField] private bool toggleRaycast = true;

        [Header("UI Refs")]
        [SerializeField] private TMP_Text subtitleTextUI;
        [SerializeField] private TMP_Text speakerNameUI;
        [SerializeField] private Image subtitleImageUI;

        private SubtitleSettingsData _settings;

        
        private void Awake()
        {
            HideSubtitle();
        }

        private void OnEnable()
        {
            if (SubtitleSettingsManager.Instance != null)
            {
                SubtitleSettingsManager.Instance.OnSettingsChanged += ApplySettings;
                ApplySettings(SubtitleSettingsManager.Instance.currentSettings);
            }
        }

        private void OnDisable()
        {
            if (SubtitleSettingsManager.Instance != null)
                SubtitleSettingsManager.Instance.OnSettingsChanged -= ApplySettings;
        }

        public void ApplySettings(SubtitleSettingsData settings)
        {
            _settings = settings;
        }

        public void ShowSubtitle(DialogueText line)
        {
            if (!line) return;

            SetVisible(true);

            // Speaker
            if (speakerNameUI)
            {
                if (line.currentSpeaker)
                {
                    speakerNameUI.gameObject.SetActive(true);
                    speakerNameUI.text = line.currentSpeaker.characterName; 
                    speakerNameUI.color = line.currentSpeaker.textColor;
                }
                else
                {
                    speakerNameUI.gameObject.SetActive(false);
                }
            }

            // Text
            if (subtitleTextUI)
            {
                subtitleTextUI.gameObject.SetActive(true);
                subtitleTextUI.text = line.subText;

                if (line.currentSpeaker)
                    subtitleTextUI.color = line.currentSpeaker.textColor;
                else if (_settings)
                    subtitleTextUI.color = _settings.textDefaultColor;
            }

            // Background
            if (subtitleImageUI)
            {
                subtitleImageUI.gameObject.SetActive(true);

                if (_settings)
                    subtitleImageUI.color = _settings.backgroundColor;
            }
        }

        public void HideSubtitle()
        {
            SetVisible(false);
        }

        public void ClearInstant()
        {
            if (subtitleTextUI) subtitleTextUI.text = "";
        }

        private void SetVisible(bool visible)
        {
            if (!subtitleGroup)
            {
                if (subtitleTextUI) subtitleTextUI.gameObject.SetActive(visible);
                if (speakerNameUI) speakerNameUI.gameObject.SetActive(visible);
                if (subtitleImageUI) subtitleImageUI.gameObject.SetActive(visible);
                return;
            }

            subtitleGroup.alpha = visible ? 1f : 0f;

            if (toggleRaycast)
            {
                subtitleGroup.interactable = visible;
                subtitleGroup.blocksRaycasts = visible;
            }
        }
    }
}
