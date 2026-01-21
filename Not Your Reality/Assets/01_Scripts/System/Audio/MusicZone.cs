using FMODUnity;
using UnityEngine;

namespace System.Audio
{
   public class MusicZone : MonoBehaviour
   {
      public EventReference music;

      private void OnTriggerEnter(Collider other)
      {
         if (!other.CompareTag("Player")) return;
         MusicManager.I.PlayMusic(music);
      }
   }
}