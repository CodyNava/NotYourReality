using System.DialogueSystem.SO;
using UnityEngine;

namespace System.DialogueSystem
{
   public class SubtitleSettingsManager : MonoBehaviour
   {
      public static SubtitleSettingsManager Instance { get; private set; }

      [Header("Default Settings Profile")]
      [SerializeField] private SubtitleSettingsData defaultSettings;
      
      public SubtitleSettingsData currentSettings;
      

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
   }
}