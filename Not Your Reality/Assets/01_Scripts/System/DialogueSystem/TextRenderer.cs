using System.Collections;
using System.DialogueSystem.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace System.DialogueSystem
{
   public class TextRenderer : MonoBehaviour
   {
      [Space(15f)]
      [Header("UI Refs")]
      [SerializeField] private TMP_Text subtitleTextUI;
      [SerializeField] private TMP_Text speakerNameUI;
      [SerializeField] private Image subtitleImageUI;

      [Space(15f)]
      [Header("Fade")]
      [SerializeField] private CanvasGroup rootGroup;
      [SerializeField] private float fadeInTime;
      [SerializeField] private float fadeOutTime;

      private Coroutine _fadeRoutine;
      private SubtitleSettingsData _currentSetting;

      private void OnEnable() { }

      private void Start()
      {
         if (SubtitleSettingsManager.Instance != null)
         {
            _currentSetting = SubtitleSettingsManager.Instance.currentSettings;
            ApplySettings(_currentSetting);
            SubtitleSettingsManager.Instance.OnSettingsChanged += ApplySettings;
         }
      }

      private void OnDisable() { SubtitleSettingsManager.Instance.OnSettingsChanged -= ApplySettings; }

      private void ApplySettings(SubtitleSettingsData settings)
      {
         if (!settings) return;

         _currentSetting = settings;

         subtitleTextUI.fontSize = settings.fontSizeMain;
         subtitleTextUI.color = settings.textDefaultColor;
         subtitleTextUI.alignment = settings.alignment;
         subtitleTextUI.outlineColor = settings.outlineColor;
         subtitleTextUI.outlineWidth = settings.outlineThickness;
         subtitleTextUI.fontStyle = settings.fontStyleMain;

         speakerNameUI.fontSize = settings.fontSizeName;
         speakerNameUI.color = settings.textDefaultColor;
         speakerNameUI.alignment = settings.alignment;
         speakerNameUI.outlineColor = settings.outlineColor;
         speakerNameUI.outlineWidth = settings.outlineThickness;
         speakerNameUI.fontStyle = settings.fontStyleName;

         subtitleImageUI.color = settings.backgroundColor;
      }

      public void ShowSubtitle(DialogueText line)
      {
         if (!line)
         {
            HideSubtitle();
            return;
         }

         subtitleTextUI.text = TruncateToMaxLines(line.subText, _currentSetting.maxLines);

         if (line.currentSpeaker)
         {
            subtitleTextUI.color = line.currentSpeaker.textColor;
            speakerNameUI.color = line.currentSpeaker.textColor;
            subtitleImageUI.enabled = true;

            if (line.currentSpeaker.characterID == 0 && !_currentSetting.displayFowlersName)
            {
               speakerNameUI.text = "";
               return;
            }

            speakerNameUI.text = line.currentSpeaker.name;
         }
         else
         {
            subtitleTextUI.color = _currentSetting.textDefaultColor;
            speakerNameUI.color = _currentSetting.textDefaultColor;
            speakerNameUI.text = "";
         }

         Fade(1f, fadeInTime);
      }

      private void Fade(float targetAlpha, float duration)
      {
         if (_fadeRoutine != null) { StopCoroutine(_fadeRoutine); }

         _fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
      }

      private IEnumerator ClearAfterFade()
      {
         yield return new WaitForSeconds(fadeOutTime);
         subtitleTextUI.text = "";
         speakerNameUI.text = "";
         subtitleImageUI.enabled = false;
      }

      private IEnumerator FadeRoutine(float target, float duration)
      {
         float start = rootGroup.alpha;
         float time = 0f;

         while (time < duration)
         {
            time += Time.deltaTime;
            float lerp = time / duration;
            rootGroup.alpha = Mathf.Lerp(start, target, lerp);
            yield return null;
         }

         rootGroup.alpha = target;
      }

      public void HideSubtitle()
      {
         Fade(0f, fadeOutTime);
         StartCoroutine(ClearAfterFade());
      }

      public void ClearInstant() { subtitleTextUI.text = ""; }

      private string TruncateToMaxLines(string text, int maxLines)
      {
         if (maxLines <= 0) return text;

         string[] lines = text.Split('\n');
         if (lines.Length <= maxLines) return text;

         string result = "";
         for (int i = 0; i < maxLines; i++)
         {
            result += lines[i];
            if (i < maxLines - 1) result += "\n";
         }

         return result;
      }
   }
}