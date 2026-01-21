using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MusicManager : MonoBehaviour
{
   public static MusicManager I;

   [Header("Crossfade")]
   [SerializeField] private float fadeTime = 1.0f;

   private EventInstance _current;
   private Coroutine _fadeRoutine;

   private void Awake()
   {
      if (I) { Destroy(gameObject); return; }
      I = this;
      DontDestroyOnLoad(gameObject);
   }

   public void PlayMusic(EventReference music)
   {
      if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
      _fadeRoutine = StartCoroutine(CrossfadeTo(music));
   }

   private IEnumerator CrossfadeTo(EventReference music)
   {
      var next = RuntimeManager.CreateInstance(music);
      next.setVolume(0f);
      next.start();
      
      if (!_current.isValid())
      {
         yield return FadeVolume(next, 0f, 1f, fadeTime);
         _current = next;
         yield break;
      }
      
      float t = 0f;
      while (t < fadeTime)
      {
         t += Time.unscaledDeltaTime;
         float k = Mathf.Clamp01(t / fadeTime);

         _current.setVolume(1f - k);
         next.setVolume(k);

         yield return null;
      }

      _current.stop(STOP_MODE.IMMEDIATE);
      _current.release();

      _current = next;
      _current.setVolume(1f);
   }

   private IEnumerator FadeVolume(EventInstance inst, float from, float to, float time)
   {
      float t = 0f;
      while (t < time)
      {
         t += Time.unscaledDeltaTime;
         float k = Mathf.Clamp01(t / time);
         inst.setVolume(Mathf.Lerp(from, to, k));
         yield return null;
      }
      inst.setVolume(to);
   }
}