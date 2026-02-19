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
      [SerializeField] private CharacterController characterController; 
      [SerializeField] private GameObject camRig;
      [SerializeField] private Transform desertTransform;
      [SerializeField] private ScreenFader screenFader;
      [SerializeField] private GameObject crossHair;

      [Header("Camera Faint Animator (on FaintRig)")]
      [SerializeField] private Animator faintAnimator; 
      [SerializeField] private string faintTriggerName = "Faint";
      [SerializeField] private string standUpTriggerName = "StandUp";
      [SerializeField] private string faintStateName = "Faint_Fall";     
      [SerializeField] private string standUpStateName = "Faint_StandUp"; 
      [SerializeField] private bool useFallbackDurations;
      [SerializeField] private float faintAnimFallbackDuration = 1.0f;
      [SerializeField] private float standUpAnimFallbackDuration = 1.2f;

      [Header("Jester")]
      [SerializeField] private Transform jesterTransform;
      [SerializeField] private Animator jesterAnimation;
      [SerializeField] private Transform jesterAppearPoint;
      [SerializeField] private Transform jesterDisappearPoint;

      [Tooltip("Nach wie vielen Sekunden der Jester erscheint")]
      [SerializeField] private float jesterAppearDelay = 0.5f;

      [Tooltip("Nach wie vielen Sekunden der Crack Trigger kommt")]
      [SerializeField] private float jesterCrackDelay = 2.0f;

      [Tooltip("Wie schnell der Player zum Jester hinschaut")]
      [SerializeField] private float lookAtSpeed = 8f;

      [Header("Sequence Timing")]
      [SerializeField] private float waitBeforeFaint = 5f;

      [Header("Fog (0 -> X -> 0)")]
      [SerializeField] private float fogInTime = 1.2f;
      [SerializeField] private float fogOutTime = 1.0f;
      [SerializeField, Range(0f, 1f)] private float fogMaxDensity = 1f;

      [Header("Fade")]
      [SerializeField] private float fadeToBlackTime = 0.8f;
      [SerializeField] private float blackHoldTime = 1.5f;
      [SerializeField] private float fadeFromBlackTime = 0.8f;

      [Header("Stand Up")]
      [SerializeField] private float standUpDelay = 0.8f;

      private bool _busy;

      private void Reset()
      {
         if (!characterController && player) characterController = player.GetComponent<CharacterController>();
      }

      private void OnTriggerEnter(Collider other)
      {
         if (_busy) return;
         if (other.gameObject != player) return;

         StartCoroutine(RunSequence());
      }

      private IEnumerator RunSequence()
      {
         _busy = true;

         // Lock player input
         pc.MoveActive = false;
         pc.LookActive = false;
         if (crossHair) crossHair.SetActive(false);
         
         float originalFog = RenderSettings.fogDensity;
         bool originalFogEnabled = RenderSettings.fog;
         //float baseFov = cam ? cam.fieldOfView : 0f;
         
         float timer = 0f;
         bool jesterSpawned = false;
         bool crackTriggered = false;

         while (timer < waitBeforeFaint)
         {
            timer += Time.deltaTime;

            if (!jesterSpawned && timer >= jesterAppearDelay)
            {
               TeleportJesterIn();
               jesterSpawned = true;
            }

            if (jesterSpawned && !crackTriggered && timer >= jesterCrackDelay)
            {
               if (jesterAnimation) jesterAnimation.SetTrigger("Crack");
               crackTriggered = true;
            }

            if (jesterSpawned) ForceLookAtJester();
            yield return null;
         }

         // Fog UP
         RenderSettings.fog = true;
         yield return FogTo(fogMaxDensity, fogInTime);

         if (screenFader) yield return screenFader.FadeTo(1f, 2f);
         // Faint Animation
         yield return PlayFaintAnimation();

         // Fade to black
         if (screenFader) yield return screenFader.FadeTo(1f, 1f);

         // Reset FOV
         //if (cam) cam.fieldOfView = baseFov;

         // Hold black
         yield return new WaitForSeconds(blackHoldTime);

         // Teleport player
         TeleportToDesert(desertTransform);


         // Fog back
         yield return FogTo(originalFog, fogOutTime);
         RenderSettings.fog = originalFogEnabled;
         
         // Stay down a bit
         yield return new WaitForSeconds(standUpDelay);

         // Stand up Animation
         yield return PlayStandUpAnimation();

         // Unlock player input
         faintAnimator.enabled = false;
         pc.SyncLookToCurrentTransform();
         pc.MoveActive = true;
         pc.LookActive = true;
         if (crossHair) crossHair.SetActive(true);

         _busy = false;
      }

     

      private IEnumerator PlayFaintAnimation()
      {
         if (!faintAnimator)
         {
            yield break;
         }

         faintAnimator.enabled = true;
         faintAnimator.ResetTrigger(standUpTriggerName);
         faintAnimator.SetTrigger(faintTriggerName);
         
         
         TeleportJesterOut();
         if (screenFader) yield return screenFader.FadeTo(0f, 1f);
         
         if (useFallbackDurations)
         {
            yield return new WaitForSeconds(faintAnimFallbackDuration);
            yield break;
         }
         
         yield return WaitForStateStart(faintAnimator, faintStateName, 0);
         yield return WaitForStateEnd(faintAnimator, faintStateName, 0);
      }

      private IEnumerator PlayStandUpAnimation()
      {
         if (!faintAnimator)
         {
            yield break;
         }

         faintAnimator.ResetTrigger(faintTriggerName);
         faintAnimator.SetTrigger(standUpTriggerName);
         if (screenFader) yield return screenFader.FadeTo(0f, 2f);
         if (useFallbackDurations)
         {
            yield return new WaitForSeconds(standUpAnimFallbackDuration);
            yield break;
         }

         yield return WaitForStateStart(faintAnimator, standUpStateName, 0);
         yield return WaitForStateEnd(faintAnimator, standUpStateName, 0);
      }

      private static IEnumerator WaitForStateStart(Animator animator, string stateName, int layer)
      {
         while (true)
         {
            var st = animator.GetCurrentAnimatorStateInfo(layer);
            if (st.IsName(stateName)) yield break;
            yield return null;
         }
      }

      private static IEnumerator WaitForStateEnd(Animator animator, string stateName, int layer)
      {
         while (true)
         {
            var st = animator.GetCurrentAnimatorStateInfo(layer);
            if (!st.IsName(stateName)) yield break;
            if (st.normalizedTime >= 1f) yield break;
            yield return null;
         }
      }
      
      private void ForceLookAtJester()
      {
         if (!jesterTransform || !camRig) return;

         Vector3 target = jesterTransform.position + new Vector3(0f, 1.5f, 0f);

         Vector3 flatDir = target - player.transform.position;
         flatDir.y = 0f;

         if (flatDir.sqrMagnitude > 0.0001f)
         {
            Quaternion targetYaw = Quaternion.LookRotation(flatDir.normalized, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetYaw, Time.deltaTime * lookAtSpeed);
         }

         Vector3 camDir = target - camRig.transform.position;
         if (camDir.sqrMagnitude > 0.0001f)
         {
            Quaternion targetCamRot = Quaternion.LookRotation(camDir.normalized, Vector3.up);
            camRig.transform.rotation = Quaternion.Slerp(camRig.transform.rotation, targetCamRot, Time.deltaTime * lookAtSpeed);
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

      private void TeleportToDesert(Transform standPos)
      {
         if (!characterController && player) characterController = player.GetComponent<CharacterController>();

         if (characterController) characterController.enabled = false;
         player.transform.position = standPos.transform.position;
         if (characterController) characterController.enabled = true;
      }
   }
}
