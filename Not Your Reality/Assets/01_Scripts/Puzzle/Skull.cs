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
                line.enabled = _reflection.Splitting;
            } 
        }
        
        public void Split(int remainingReflections)
        {
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
                                currentDirection = _reflection.Reflect(currentDirection, hit.normal);
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