using System.DialogueSystem.SO;
using System.DialogueSystem;
using UnityEngine;

namespace System.DialogueSystem
{
   public class SubtitleSettingsManager : MonoBehaviour
   {
      public static SubtitleSettingsManager Instance { get; private set; }

      [Header("Default Settings Profile")]
      [SerializeField] private SubtitleSettingsData defaultSettings;
      
      public SubtitleSettingsData currentSettings;
      
      public event Action<SubtitleSettingsData> OnSettingsChanged;

      public void Awake()
      {
         if (Instance && Instance != this)
         {
            Destroy(gameObject);
            return;
         }
         
         SettingsInit();
      }
      private void SettingsInit()
      {
         Instance = this;
         currentSettings = defaultSettings;
      }
      
      private void ApplySettings(SubtitleSettingsData newSetting)
      {
         if (!newSetting) { return; }
         currentSettings = newSetting;
         OnSettingsChanged?.Invoke(currentSettings);
      }

      public void ResetToDefaultSettings()
      {
         ApplySettings(defaultSettings);
      }
   }
}