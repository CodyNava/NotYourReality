using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class MirrorLightReflection : MonoBehaviour
    {
        [Tooltip("The light source of this riddle")]
        [SerializeField] private LightSource lightSource;
        
        [Tooltip("The renderer component of the Goal")]
        [SerializeField] private Renderer targetRenderer;

        [Tooltip("The material the Goal should have when it is hit by the Light")]
        [SerializeField] private Material targetWinMaterial;

        [Tooltip("The material the Goal should have when it is not hit by the Light")]
        [SerializeField] private Material targetDefaultMaterial;

        [Tooltip("The door that is supposed to be activated once the Goal is hit")]
        [SerializeField] private Collider doorCollider;
        
        private readonly List<GameObject> _objectsToReset = new ();
        private readonly List<Vector3> _initialPosition = new ();
        private readonly List<Quaternion> _initialRotation = new ();

        private void Awake()
        {
            targetRenderer.material = targetDefaultMaterial;
            foreach (Transform child in transform)
            {
                _initialPosition.Add(child.position);
                _initialRotation.Add(child.rotation);
                _objectsToReset.Add(child.gameObject);
            }

            if (!doorCollider) return;
            doorCollider.enabled = false;
        }

        public void CheckWin()
        {
            targetRenderer.material = lightSource.TargetHit ? targetWinMaterial : targetDefaultMaterial;
            if (!doorCollider) return;
            doorCollider.enabled = lightSource.TargetHit;
        }

        public IEnumerator ResetRiddle()
        {
            for (var i = 0; i < _objectsToReset.Count; i++)
            {
                _objectsToReset[i].transform.position = _initialPosition[i];
                _objectsToReset[i].transform.rotation = _initialRotation[i];
                yield return null;
            }
        }
    }
}