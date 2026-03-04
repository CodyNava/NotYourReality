using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace System.Audio
{
   public class FMODButtons : MonoBehaviour
   {
      [SerializeField] private EventReference buttonPress;
      private EventInstance _eventInstance;
      [SerializeField] private Button button;

      public void OnPress()
      {
         if (button.interactable) { RuntimeManager.PlayOneShot(buttonPress, transform.position); }
      }
   }
}