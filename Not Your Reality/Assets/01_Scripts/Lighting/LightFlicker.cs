using System.Collections;
using UnityEngine;

namespace Lighting
{
    public class LightFlicker : MonoBehaviour
    {
        [Space]
        [Tooltip("The minimum strength to which the light should drop ")]
        [SerializeField] private float strength;
        [Tooltip("How many times the light is supposed to flicker")]
        [SerializeField] private int frequency;
        [Tooltip("The time to complete the flickers")]
        [SerializeField] private float time;
        [Tooltip("The randomness of the flicker in %")]
        [Range(0f, 1f)]
        [SerializeField] private float randomness;
        [Tooltip("Light Bulb")]
        [SerializeField] private GameObject lightBulb;
       
        private Light _illuminatingObject;
        private float _timer;
        private float _originalIntensity;
        private float _delay;
        

        private void Awake()
        {
            StartCoroutine(InitializeLight());
            GenerateDelay();
        }

        private IEnumerator InitializeLight()
        {
            yield return new WaitForSeconds(0.5f);
            _illuminatingObject = GetComponent<Light>();
            _originalIntensity = _illuminatingObject.intensity;
        }

        private void Update()
        {
            Flicker();
        }

        private void Flicker()
        {
            _timer += Time.deltaTime;

            if (_timer >= _delay)
            {
                _illuminatingObject.intensity = _illuminatingObject.intensity == _originalIntensity ? strength : _originalIntensity;
                TurnOffBulb();
                _timer = 0;
                GenerateDelay();
            }
        }

        private void TurnOffBulb()
        {
            if (lightBulb == null) return;
            lightBulb.SetActive(_illuminatingObject.intensity == _originalIntensity);
        }

        private void GenerateDelay()
        {
            var averageTime = frequency / time;
            var variation = averageTime * randomness;
            _delay = Random.Range(averageTime - variation, averageTime + variation);
        }
    }
}
