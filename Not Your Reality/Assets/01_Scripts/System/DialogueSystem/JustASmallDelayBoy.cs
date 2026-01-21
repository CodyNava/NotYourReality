using System.Collections;
using UnityEngine;

namespace System.DialogueSystem
{
    public class JustASmallDelayBoy : MonoBehaviour
    {
        [SerializeField] private float waitTime;
        [SerializeField] private GameObject waitingObject;

        private void OnEnable()
        {
            StartCoroutine(SmallDelay());
        }
        private IEnumerator SmallDelay()
        {
            yield return new WaitForSeconds(waitTime);
            waitingObject.gameObject.SetActive(true);
        }
    }
}
