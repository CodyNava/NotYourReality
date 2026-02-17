using System.Collections;
using Player.PlayerMovement.Movement;
using UnityEngine;

namespace Puzzle.Bedroom
{
   public class JesterTrigger : MonoBehaviour
   {
      [Header("Refs")]
      [SerializeField] private GameObject player;
      [SerializeField] private PlayerController pc;
      [SerializeField] private Camera cam;
      [SerializeField] private Transform desertTransform;
      [SerializeField] private ScreenFader screenFader;
      [SerializeField] private GameObject crossHair;

      [Header("Jester")]
      [SerializeField] private Transform jesterTransform;
      [SerializeField] private Animator jesterAnimation;

      [Tooltip("Wo der Jester erscheinen soll")]
      [SerializeField] private Transform jesterAppearPoint;

      [Tooltip("Wo der Jester hin soll wenn er weg ist (oder leave empty = disable)")]
      [SerializeField] private Transform jesterDisappearPoint;

      [Tooltip("Nach wie vielen Sekunden (innerhalb waitBeforeFaint) der Jester erscheint")]
      [SerializeField] private float jesterAppearDelay = 0.5f;

      [Tooltip("Nach wie vielen Sekunden (innerhalb waitBeforeFaint) der Crack Trigger kommt")]
      [SerializeField] private float jesterCrackDelay = 2.0f;

      [Tooltip("Wie schnell der Player zum Jester hinschaut")]
      [SerializeField] private float lookAtSpeed = 8f;

      [Header("Sequence Timing")]
      [SerializeField] private float waitBeforeFaint = 5f;

      [Header("Fall Movement")]
      [SerializeField] private float fallDownDistance = 0.35f;
      [SerializeField] private float fallSpeed = 1.2f;
      [SerializeField] private Vector3 driftWorld = Vector3.zero;

      [Header("Rotation")]
      [SerializeField] private bool rotateWhileFainting = true;
      [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 0f, 35f);

      [Header("FOV")]
      [SerializeField] private bool fovPunch = true;
      [SerializeField] private float fovTarget = 55f;

      [Header("Fog (0 -> 1 -> 0)")]
      [SerializeField] private float fogInTime = 1.2f;
      [SerializeField] private float fogOutTime = 1.0f;
      [SerializeField, Range(0f, 1f)] private float fogMaxDensity = 1f;

      [Header("Fade")]
      [SerializeField] private float fadeToBlackTime = 0.8f;
      [SerializeField] private float blackHoldTime = 1.5f;
      [SerializeField] private float fadeFromBlackTime = 0.8f;

      [Header("Stand Up")]
      [SerializeField] private float standUpDelay = 0.8f;
      [SerializeField] private float standUpDuration = 0.6f;

      private bool _busy;

      private void OnTriggerEnter(Collider other)
      {
         if (_busy) return;
         if (other.gameObject != player) return;

         StartCoroutine(RunSequence());
      }

      private IEnumerator RunSequence()
      {
         _busy = true;

         pc.MoveActive = false;
         pc.LookActive = false;
         crossHair.SetActive(false);

         float originalFog = RenderSettings.fogDensity;
         bool originalFogEnabled = RenderSettings.fog;

         Quaternion uprightRot = player.transform.rotation;
         float baseFov = cam ? cam.fieldOfView : 0f;

         float timer = 0f;
         bool jesterSpawned = false;
         bool crackTriggered = false;

         while (timer < waitBeforeFaint)
         {
            timer += Time.deltaTime;

            // Spawn Jester
            if (!jesterSpawned && timer >= jesterAppearDelay)
            {
               TeleportJesterIn();
               jesterSpawned = true;
            }

            if (jesterSpawned && !crackTriggered && timer >= jesterCrackDelay)
            {
               if (jesterAnimation) { jesterAnimation.SetTrigger("Crack"); }

               crackTriggered = true;
            }

            if (jesterSpawned) ForceLookAtJester();

            yield return null;
         }



         // Fog UP
         RenderSettings.fog = true;
         yield return FogTo(fogMaxDensity, fogInTime);
         
         //Faint
         yield return FaintingRoutine();
         
         //Fade to black
         if (screenFader) yield return screenFader.FadeTo(1f, fadeToBlackTime);
         cam.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
         
         // Hold black
         yield return new WaitForSeconds(blackHoldTime);

         // Teleport player while black
         Vector3 desertStandPos = desertTransform ? desertTransform.position : player.transform.position;
         TeleportToDesertLying(desertStandPos);

         // Jester disappears 
         TeleportJesterOut();
         
         //Fog to original density
         yield return FogTo(originalFog, fogOutTime);

         // restore fog
         RenderSettings.fog = originalFogEnabled;

         // Fade back in
         if (screenFader) yield return screenFader.FadeTo(0f, fadeFromBlackTime);

         // Stay down
         yield return new WaitForSeconds(standUpDelay);

         // Stand up 
         yield return StandUpRoutine(uprightRot, baseFov, desertStandPos);

         pc.MoveActive = true;
         pc.LookActive = true;
         crossHair.SetActive(true);

         _busy = false;
      }

      private void ForceLookAtJester()
      {
         if (!jesterTransform || !cam) return;

         Vector3 target = jesterTransform.position + new Vector3(0f, 1.5f, 0f);
         
         Vector3 flatDir = target - player.transform.position;
         flatDir.y = 0f;

         if (flatDir.sqrMagnitude > 0.0001f)
         {
            Quaternion targetYaw = Quaternion.LookRotation(flatDir.normalized, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(
               player.transform.rotation,
               targetYaw,
               Time.deltaTime * lookAtSpeed
            );
         }
         
         Vector3 camDir = target - cam.transform.position;
         if (camDir.sqrMagnitude > 0.0001f)
         {
            Quaternion targetCamRot = Quaternion.LookRotation(camDir.normalized, Vector3.up);
            cam.transform.rotation = Quaternion.Slerp(
               cam.transform.rotation,
               targetCamRot,
               Time.deltaTime * lookAtSpeed
            );
         }
      }

      private void TeleportJesterIn()
      {
         if (!jesterTransform) return;

         if (jesterAppearPoint)
         {
            jesterTransform.position = jesterAppearPoint.position;
            jesterTransform.rotation = jesterAppearPoint.rotation;
            jesterTransform.gameObject.SetActive(true);
         }
         else
         {
            jesterTransform.gameObject.SetActive(true);
         }
      }

      private void TeleportJesterOut()
      {
         if (!jesterTransform) return;

         if (jesterDisappearPoint)
         {
            jesterTransform.position = jesterDisappearPoint.position;
            jesterTransform.rotation = jesterDisappearPoint.rotation;
            jesterTransform.gameObject.SetActive(true);
         }
         else
         {
            jesterTransform.gameObject.SetActive(false);
         }
      }

      private IEnumerator FaintingRoutine()
      {
         Vector3 posA = player.transform.position;
         Vector3 posB = posA + Vector3.down * fallDownDistance + driftWorld;

         Quaternion rotA = player.transform.rotation;
         Quaternion rotB = rotA * Quaternion.Euler(rotationOffset);

         float fovA = cam ? cam.fieldOfView : 0f;

         float duration = Mathf.Max(0.01f, fallDownDistance / Mathf.Max(0.01f, fallSpeed));

         float t = 0f;
         while (t < 1f)
         {
            t += Time.deltaTime / duration;
            float eased = t * t * (3f - 2f * t);

            player.transform.position = Vector3.Lerp(posA, posB, eased);

            if (rotateWhileFainting) player.transform.rotation = Quaternion.Slerp(rotA, rotB, eased);

            if (cam && fovPunch) cam.fieldOfView = Mathf.Lerp(fovA, fovTarget, eased);

            yield return null;
         }
      }

      private IEnumerator StandUpRoutine(Quaternion uprightRot, float baseFov, Vector3 standPos)
      {
         Vector3 downPos = standPos + Vector3.down * fallDownDistance;

         Vector3 posA = player.transform.position;
         Vector3 posB = standPos;

         Quaternion rotA = player.transform.rotation;
         Quaternion rotB = uprightRot;

         float fovA = cam ? cam.fieldOfView : 0f;

         float t = 0f;
         while (t < 1f)
         {
            t += Time.deltaTime / Mathf.Max(0.01f, standUpDuration);
            float eased = t * t * (3f - 2f * t);

            player.transform.position = Vector3.Lerp(posA, posB, eased);

            player.transform.rotation = Quaternion.Slerp(rotA, rotB, eased);

            if (cam && fovPunch) cam.fieldOfView = Mathf.Lerp(fovA, baseFov, eased);

            yield return null;
         }

         player.transform.position = standPos;
         player.transform.rotation = uprightRot;
         if (cam && fovPunch) cam.fieldOfView = baseFov;
      }

      private IEnumerator FogTo(float targetDensity, float time)
      {
         float start = RenderSettings.fogDensity;
         float t = 0f;

         while (t < 1f)
         {
            t += Time.deltaTime / Mathf.Max(0.01f, time);
            float eased = t * t * (3f - 2f * t);

            RenderSettings.fogDensity = Mathf.Lerp(start, targetDensity, eased);
            yield return null;
         }

         RenderSettings.fogDensity = targetDensity;
      }

      private void TeleportToDesertLying(Vector3 desertStandPos)
      {
         player.transform.position = desertStandPos + Vector3.down * fallDownDistance;
      }
   }
}