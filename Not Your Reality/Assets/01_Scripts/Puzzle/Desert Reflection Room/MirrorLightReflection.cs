using System.Collections.Generic;
using System.Linq;
using FMODUnity;
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
        [SerializeField] private Animator door;

        [Tooltip("The door unlock sound once the puzzle is finished")]
        [SerializeField] private EventReference unlockSound;

        [Tooltip("The Color the target is supposed to have when hit")]
        [SerializeField] private Color hitColor;
        [Tooltip("The intensity of the light")]
        [Range(50f, 100f)]
        [SerializeField] private float lightIntensity = 50f;

        private bool _puzzleCompleted;
        private readonly List<Renderer> _targetRenderer = new();
        public List<ReflectionGoal> goals = new();

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                if (!child.CompareTag("Goal")) continue;
                goals.Add(child.gameObject.GetComponent<ReflectionGoal>());
                foreach (Transform child2 in child)
                {
                    if (child2.TryGetComponent(out Renderer rend))
                    {
                        _targetRenderer.Add(rend);
                    }
                }
            }
        }


        public void CheckWin()
        {
            for (int i = 0; i < goals.Count; i++)
            {
                var hit = goals[i].BeenHit();
                var mat = _targetRenderer[i].materials;

                if (hit)
                {
                    mat[1].SetColor(EmissionColor, hitColor * lightIntensity);
                    goals[i].GetComponent<Animator>().enabled = true;
                }
                else
                {
                    mat[1].SetColor(EmissionColor, Color.black);
                }
            }

            if (!door) return;
            if (!AllHit() || _puzzleCompleted) return;
            _puzzleCompleted = true;
            door.enabled = true;

            RuntimeManager.PlayOneShot(unlockSound, transform.position);
        }

        private bool AllHit()
        {
            var winCounter = goals.Count(goal => goal.BeenHit());
            return winCounter == goals.Count;
        }

    }

}