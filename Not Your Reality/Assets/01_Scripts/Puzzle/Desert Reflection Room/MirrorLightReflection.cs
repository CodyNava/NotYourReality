using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;

namespace Puzzle.Desert_Reflection_Room
{
    public class MirrorLightReflection : MonoBehaviour
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [Tooltip("The light source of this riddle")]
        [SerializeField] private LightSource lightSource;

        [Tooltip("The goal")]
        [SerializeField] private ReflectionGoal reflectionGoal;

        [Tooltip("The door that is supposed to be activated once the Goal is hit")]
        [SerializeField] private DoorHandle door;

        [Tooltip("The door unlock sound once the puzzle is finished")]
        [SerializeField] private EventReference unlockSound;

        [Tooltip("The Color the target is supposed to have when hit")]
        [SerializeField] private Color hitColor;
        [Tooltip("The intensity of the light")]
        [Range(50f, 100f)]
        [SerializeField] private float lightIntensity = 50f;


        [SerializeField] private List<GameObject> allDoorHandles;

        private bool _puzzleCompleted;
        private readonly List<GameObject> _objectsToReset = new();
        private readonly List<Vector3> _initialPosition = new();
        private readonly List<Quaternion> _initialRotation = new();
        private readonly List<Renderer> _targetRenderer = new();
        public List<ReflectionGoal> goals = new();

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                _initialPosition.Add(child.position);
                _initialRotation.Add(child.rotation);
                _objectsToReset.Add(child.gameObject);
                if (!child.CompareTag("Goal")) continue;
                goals.Add(child.gameObject.GetComponent<ReflectionGoal>());
                _targetRenderer.Add(child.gameObject.GetComponent<Renderer>());
            }

            if (!door) return;
            foreach (var handle in allDoorHandles)
            {
                handle.layer = LayerMask.NameToLayer("Default");
                if (handle.TryGetComponent<DoorHandle>(out var doorHandle))
                {
                    doorHandle.IsInteractable = false;
                }
            }
        }


        public void CheckWin()
        {
            for (int i = 0; i < goals.Count; i++)
            {
                var hit = goals[i].BeenHit();
                var mat = _targetRenderer[i].material;

                if (hit)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor(EmissionColor, hitColor * lightIntensity);
                }
                else
                {
                    mat.SetColor(EmissionColor, Color.black);
                }
            }

            if (!door) return;

            //todo: more feedback for winning

            if (!AllHit() || _puzzleCompleted) return;
            _puzzleCompleted = true;
            foreach (var handle in allDoorHandles)
            {
                handle.layer = LayerMask.NameToLayer("Interactable");
                if (handle.TryGetComponent<DoorHandle>(out var doorHandle))
                {
                    doorHandle.IsInteractable = true;
                }
            }

            RuntimeManager.PlayOneShot(unlockSound, transform.position);
        }

        private bool AllHit()
        {
            var winCounter = goals.Count(goal => goal.BeenHit());
            return winCounter == goals.Count;
        }

        public IEnumerator ResetRiddle()
        {
            for (var i = 0; i < _objectsToReset.Count; i++)
            {
                _objectsToReset[i].transform.position = _initialPosition[i];
                _objectsToReset[i].transform.rotation = _initialRotation[i];
                yield return null;
            }

            reflectionGoal.ClearHits();
        }
    }

}