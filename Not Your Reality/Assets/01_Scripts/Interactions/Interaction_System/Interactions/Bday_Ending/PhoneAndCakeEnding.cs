using System.Collections;
using System.Collections.Generic;
using System.GlobalEventSystem;
using Player.PlayerMovement.Movement;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions.Bday_Ending
{
   public class PhoneAndCakeEnding : MonoBehaviour
   {
      [SerializeField] private GameObject phone;
      [SerializeField] private GameObject cake;
      [SerializeField] private Vector3 fallOffsetPos;
      [SerializeField] private Vector3 finalPositionAfterFalling;
      [SerializeField] private Quaternion finalRotationAfterFalling;
      [SerializeField] private Quaternion finalCamRotationAfterFalling;
      [SerializeField] private GameObject creditsScreen;
      [SerializeField] private GameObject crossHair;
      [SerializeField] private GameObject player;
      [SerializeField] private CanvasGroup screenFader;
      [SerializeField] private PlayerController ps;
      [SerializeField] private GameObject cameraHolder;
      [SerializeField] private float playerToTableTransformSpeed;
      [SerializeField] private float delayAfterCake;
      [SerializeField] private float delayAfterMonologue;
      [SerializeField] private float delayAfterPhone;
      [SerializeField] private float delayAfterFogHasGrown;
      [SerializeField] private float delayFogStartsGrowing;
      [SerializeField] private float fogTargetDensity;
      [SerializeField] private float fogGrowTime;
      [SerializeField] private float fallDuration;
      [SerializeField] private List<GameObject> fmodEmitter;
      private Transform _playerTransform;

      private void OnEnable()
      {
         GlobalEventManager.OnCake += CakeCake;
         GlobalEventManager.OnPhone += PhoneTouched;
      }

      private void OnDisable()
      {
         GlobalEventManager.OnCake -= CakeCake;
         GlobalEventManager.OnPhone -= PhoneTouched;
      }
      

      private void CakeCake()
      {
         cake.layer = LayerMask.NameToLayer("Default");
         fmodEmitter[0].SetActive(true);
         StartCoroutine(DelayAfterTouching(delayAfterCake, "cake"));
      }

      private void PhoneTouched()
      {
         phone.layer = LayerMask.NameToLayer("Default");
         fmodEmitter[1].SetActive(true);
         StartCoroutine(DelayAfterTouching(delayAfterPhone, "phone"));
      }

      private IEnumerator FadeToBlack(int alpha, float delay)
      {
         yield return new WaitForSeconds(delay);
         screenFader.alpha = alpha;
      }
      

      private void EndingSequence()
      {
         
         crossHair.gameObject.SetActive(false);
         ps.LookActive = false;
         StartCoroutine(EnableCredits());
         StartCoroutine(FallingThroughFloor(fallOffsetPos, fallDuration));
      }

      private IEnumerator DelayAfterPhone()
      {
         fmodEmitter[2].SetActive(true);
         yield return new WaitForSeconds(delayFogStartsGrowing);
         StartCoroutine(FogStartsGrowing());
      }
   
      private IEnumerator EnableCredits()
      {
         yield return new WaitForSeconds(fallDuration * 0.8f);
         creditsScreen.gameObject.SetActive(true);
      }

      private IEnumerator FallingThroughFloor(Vector3 offSet, float fallingTime)
      {
         StartCoroutine(FadeToBlack(1, 0.25f));
         var pT = player.transform;
         var startPos = pT.position;
         var endPos = startPos + offSet;

         float time = 0f;

         while (time < fallingTime)
         {
            time += Time.deltaTime;
            float a = time / fallingTime;
            pT.position = Vector3.Lerp(startPos, endPos, a);
            yield return null;
         }
         pT.position = endPos;
         player.transform.position = finalPositionAfterFalling;
         player.transform.rotation = finalRotationAfterFalling;
         cameraHolder.transform.localRotation = finalCamRotationAfterFalling;
         StartCoroutine(FadeToBlack(0, 0.2f));
         RenderSettings.fogDensity = 0;
      }
      
      private IEnumerator FogStartsGrowing()
      {
         float startDensity = RenderSettings.fogDensity;
         var t = 0f;

         while (t < fogGrowTime)
         {
            t += Time.deltaTime;
            float lerp = t / fogGrowTime;

            RenderSettings.fogDensity = Mathf.Lerp(startDensity, fogTargetDensity, lerp);

            yield return null;
         }

         RenderSettings.fogDensity = fogTargetDensity;
         StartCoroutine(DelayAfterTouching(delayAfterFogHasGrown, "EndingSequence"));
      }

      private IEnumerator DelayAfterTouching(float time, string obj)
      {
         yield return new WaitForSeconds(time);
         switch (obj)
         {
            case "cake":           phone.layer = LayerMask.NameToLayer("Interactable"); break;
            case "phone":          StartCoroutine(DelayAfterPhone()); break;
            case "EndingSequence": EndingSequence(); break;
         }
      }
   }
}