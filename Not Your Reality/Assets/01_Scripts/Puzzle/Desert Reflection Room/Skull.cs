using System.Collections.Generic;
using UnityEngine;

namespace Puzzle.Desert_Reflection_Room
{
    public class Skull : MonoBehaviour
    {
        private static readonly List<Skull> RegisteredSkulls = new List<Skull>();
        [SerializeField] private LightSource lightSource;
        [SerializeField] private MirrorLightReflection mirrorLightReflection;

        private Skull _otherSkull;

        private List<LineRenderer> _lineRenderers;
        private bool _continueTracing;

        public bool Reached { get; set; }
        public bool Splitting { get; set; }

        private void Awake()
        {
            foreach (var line in GetComponentsInChildren<LineRenderer>())
            {
                line.enabled = false;
            }
        }

        private void Update()
        {
            foreach (var line in GetComponentsInChildren<LineRenderer>())
            {
                line.enabled = Splitting && Reached;
            }
        }

        public static void ClearAllSplits()
        {
            foreach (var skull in RegisteredSkulls)
            {
                skull.Splitting = false;
            }
        }

        private void OnEnable()
        {
            if (!RegisteredSkulls.Contains(this))
            {
                RegisteredSkulls.Add(this);
            }
        }

        private void OnDisable()
        {
            RegisteredSkulls.Remove(this);
        }

        public bool HasReached(Quaternion current, Quaternion final)
        {
            return Quaternion.Angle(current, final) < 3;
        }

        public void Split(int remainingRefs)
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

                for (var i = 0; i < remainingRefs && _continueTracing; i++)
                {
                    if (Physics.Raycast(currentPosition, currentDirection, out var hit, lightSource.BeamLength))
                    {
                        linePoints.Add(hit.point);

                        switch (hit.collider.tag)
                        {
                            case "Mirror":
                                currentPosition = hit.point;
                                currentDirection = LightSource.Reflect(currentDirection, hit.normal);
                                break;

                            case "Goal":
                                linePoints.Add(hit.point);
                                _continueTracing = false;
                                foreach (var goal in mirrorLightReflection.goals)
                                {
                                    if (hit.collider != goal.GetComponent<Collider>()) continue;
                                    if (goal.BeenHit()) break;
                                    goal.RegisterHit(hit.point);
                                }

                                break;

                            case "Skull":
                                _continueTracing = false;
                                _otherSkull = hit.collider.GetComponent<Skull>();
                                if (_otherSkull != null && _otherSkull != this)
                                {
                                    if (_otherSkull.Splitting) break;
                                    _otherSkull.Splitting = true;

                                    _otherSkull.Reached = false;
                                    _otherSkull.Reached = _otherSkull.HasReached(
                                        _otherSkull.transform.rotation,
                                        Quaternion.LookRotation(currentDirection)
                                    );
                                    if (_otherSkull != null && _otherSkull != this)
                                    {
                                        if (_otherSkull.Reached)
                                        {
                                            _otherSkull.transform.rotation = Quaternion.LookRotation(currentDirection);
                                        }
                                        else
                                        {
                                            _otherSkull.transform.rotation = Quaternion.Slerp(
                                                _otherSkull.transform.rotation,
                                                Quaternion.LookRotation(currentDirection),
                                                lightSource.RotationSpeed
                                            );
                                        }

                                        _otherSkull.Split(remainingRefs - i);
                                    }
                                }

                                break;
                        }
                    }
                    else
                    {
                        linePoints.Add(currentPosition + currentDirection * lightSource.BeamLength);
                        break;
                    }
                }

                mirrorLightReflection.CheckWin();
                if (linePoints.Count > 1)
                {
                    var lastPoint = linePoints[^1];
                    var lastDir = (linePoints.Count >= 2)
                        ? (linePoints[^1] - linePoints[^2]).normalized
                        : currentDirection;
                    if (!Physics.Raycast(lastPoint, lastDir, out _))
                    {
                        linePoints.Add(lastPoint + lastDir * lightSource.BeamLength);
                    }
                }

                line.positionCount = linePoints.Count;
                line.SetPositions(linePoints.ToArray());
            }
        }
    }
}