using System;
using System.Collections;
using Player.PlayerMovement.Movement;
using Puzzle.Bedroom;
using UnityEngine;

namespace Puzzle.Basement
{
    public class IntroSequence : MonoBehaviour
    {
        private static readonly string  WakeUpStateName = ("WakeUp");
        [SerializeField] private ScreenFader screenFader;
        [SerializeField] private GameObject crossHair;
        [SerializeField] private GameObject player;
        [SerializeField] private PlayerController pc;
        [SerializeField] private CharacterController characterController; 
        [SerializeField] private GameObject camRig;
        [SerializeField] private float timeBetweenBlinks;
        [SerializeField] private float blinkDelay;
        [SerializeField] private Animator faintAnimator;
        [Header("Intro Toggle")]
        [SerializeField] private bool skipIntro;


        private void Start()
        {
            if (skipIntro) return;
            StartCoroutine(IntroSequenceRoutine());
        }

        private IEnumerator IntroSequenceRoutine()
        {
            pc.MoveActive = false;
            pc.LookActive = false;
            if (crossHair) crossHair.SetActive(false);
            
            yield return new WaitForSeconds(1f);
            
            StartStandUpAnimation();
            yield return new WaitForSeconds(0.5f);
            if (screenFader) yield return screenFader.FadeTo(0f, 0.5f);
            
            yield return Blinking(0.2f);
            yield return Blinking(0.2f);
            yield return WaitForStateEnd(faintAnimator,  0);
            faintAnimator.enabled = false;
            pc.MoveActive = true;
            pc.LookActive = true;
            if (crossHair) crossHair.SetActive(true);
           
        }

        private void StartStandUpAnimation()
        {
            faintAnimator.enabled = true;
            faintAnimator.SetTrigger(WakeUpStateName);
            
        }

        private IEnumerator Blinking(float duration)
        {
            yield return new WaitForSeconds(blinkDelay);
            if (screenFader) yield return screenFader.FadeTo(1f, duration);
            yield return new WaitForSeconds(timeBetweenBlinks);
            if (screenFader) yield return screenFader.FadeTo(0f, duration);
        }
        
        private static IEnumerator WaitForStateEnd(Animator animator, int layer)
        {
            while (true)
            {
                var st = animator.GetCurrentAnimatorStateInfo(layer);
                if (st.normalizedTime >= 1f) yield break;
                yield return null;
            }
        }
    }
}
