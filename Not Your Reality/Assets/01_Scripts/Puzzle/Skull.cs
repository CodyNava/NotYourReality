using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    [ExecuteInEditMode]
    public class Skull : MonoBehaviour
    {
        private MirrorLightReflection _reflection;
        private List<LineRenderer> _lineRenderers;
        private bool _continueTracing;
        public bool Reached { get; set; }

        private void Awake()
        {
            foreach (var line in GetComponentsInChildren<LineRenderer>())
            {
                line.enabled = false;
            }
            _reflection = GetComponentInParent<MirrorLightReflection>();
        }

        private void Update()
        {
            foreach (var line in GetComponentsInChildren<LineRenderer>())
            {
                if (_reflection.Splitting && Reached)
                {
                    line.enabled = true;
                }
                else
                {
                    line.enabled = false;
                }
            }
        }

        public static bool HasReached(Quaternion current, Quaternion final)
        {
            return Quaternion.Angle(current, final) < 7;
        }
        
        public void Split(int remainingReflections)
        {
            if (!Reached) return;
            foreach (var line in GetComponentsInChildren<LineRenderer>())
            {
                var linePoints = line.GetComponent<ReflectionPoints>().points;
                linePoints.Clear();
                
                var currentPosition = transform.position;
                var currentDirection = line.transform.forward;
                linePoints.Add(currentPosition);
                
                _continueTracing = true;
                
                for (var i = 0; i < remainingReflections && _continueTracing; i++)
                {
                    if (Physics.Raycast(currentPosition, currentDirection, out var hit, _reflection.BeamLength))
                    {
                        linePoints.Add(hit.point);

                        switch (hit.collider.tag)
                        {
                            case "Mirror":
                                currentPosition = hit.point;
                                currentDirection = MirrorLightReflection.Reflect(currentDirection, hit.normal);
                                break;

                            case "Goal":
                                linePoints.Add(hit.point);
                                _reflection.TargetHit = true;
                                _continueTracing = false;
                                break;

                            case "Death Trap":
                                StartCoroutine(_reflection.ResetRiddle());
                                _continueTracing = false;
                                break;

                            case "Skull":
                                var otherSkull = hit.collider.GetComponent<Skull>();
                                if (otherSkull != null && otherSkull != this)
                                {
                                    otherSkull.transform.rotation = Quaternion.Slerp(otherSkull.transform.rotation, Quaternion.LookRotation(currentDirection), _reflection.RotationSpeed);
                                    otherSkull.Reached = HasReached(otherSkull.transform.rotation, Quaternion.LookRotation(currentDirection));
                                    otherSkull.Split(remainingReflections - i);
                                }
                                _continueTracing = false;
                                break;
                        }
                    }
                    else
                    {
                        linePoints.Add(currentPosition + currentDirection * _reflection.BeamLength);
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
                            linePoints.Add(lastPoint + lastDir * _reflection.BeamLength);
                        }
                }
                line.positionCount = linePoints.Count;
                line.SetPositions(linePoints.ToArray());
            }
        }
    }
}