using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    [ExecuteInEditMode]
    public class Skull : MonoBehaviour
    {
        private List<LineRenderer> _lineRenderers;
        [SerializeField] private MirrorLightReflection reflection;

        private void Awake()
        {
            foreach (var line in GetComponentsInChildren<LineRenderer>())
            {
                line.enabled = false;
            }
            //reflection = GetComponentInParent<MirrorLightReflection>();
        }

        private void Update()
        {
            foreach (var line in GetComponentsInChildren<LineRenderer>())
            {
                line.enabled = reflection.Splitting;
            }
        }
        public void Split()
        {
            foreach (var line in GetComponentsInChildren<LineRenderer>())
            {
                var linePoints = line.GetComponent<ReflectionPoints>().points;
                linePoints.Clear();
                var currentPosition = transform.position;
                var currentDirection = line.transform.forward;
                linePoints.Add(currentPosition);
                for (var i = 0; i < reflection.Reflections; i++)
                {
                    if (Physics.Raycast(currentPosition, currentDirection, out var hit, reflection.BeamLength))
                    {
                        linePoints.Add(hit.point);

                        if (hit.collider.CompareTag("Mirror"))
                        {
                            currentPosition = hit.point;
                            currentDirection = reflection.Reflect(currentDirection, hit.normal);
                        }
                        else if (hit.collider.CompareTag("Goal"))
                        {
                            linePoints.Add(hit.point);
                            reflection.TargetHit = true;
                            break;
                        }
                        else if (hit.collider.CompareTag("Death Trap"))
                        {
                            //TODO: GAME OVER and RESET
                            break;
                        }
                        else if (hit.collider.CompareTag("Skull"))
                        {
                            var otherSkull = hit.collider.GetComponent<Skull>();
                            if (otherSkull != null && otherSkull != this)
                            {
                                otherSkull.Split();
                            }
                            break;
                        }
                    }
                    else
                    {
                        linePoints.Add(currentPosition + currentDirection * reflection.BeamLength);
                        break;
                    }
                }
                if (linePoints.Count > 1)
                {
                        var lastPoint = linePoints[^1];
                        var lastDir = (linePoints.Count >= 2)
                            ? (linePoints[^1] - linePoints[^2]).normalized : currentDirection;
                        if (!Physics.Raycast(lastPoint, lastDir, out _))
                        {
                            linePoints.Add(lastPoint + lastDir * reflection.BeamLength);
                        }
                }
                line.enabled = true;
                line.positionCount = linePoints.Count;
                line.SetPositions(linePoints.ToArray());
            }
        }
    }
}