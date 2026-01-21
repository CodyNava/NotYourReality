using System.Collections;
using UnityEngine;

namespace Puzzle.Bedroom
{
   public class ScreenFader : MonoBehaviour
   {
      [SerializeField] private CanvasGroup canvasGroup;

      public IEnumerator FadeTo(float targetAlpha, float duration)
      {
         if (!canvasGroup) yield break;

         float start = canvasGroup.alpha;
         float t = 0f;

         while (t < 1f)
         {
            t += Time.deltaTime / Mathf.Max(0.01f, duration);
            float eased = t * t * (3f - 2f * t);

            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, eased);
            yield return null;
         }

         canvasGroup.alpha = targetAlpha;
      }
   }
}