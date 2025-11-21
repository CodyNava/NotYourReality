using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    [ExecuteInEditMode]
    public class MirrorLightReflection : MonoBehaviour
    {
        [Header("Beam Settings")]
        [Tooltip("The length of the Light beam for this puzzle")]
        [SerializeField] private float beamLength;

        public float BeamLength => beamLength;

        [Tooltip("Allowed reflections before the Light stops")]
        [SerializeField] private int reflections;

        [Header("Target Settings")] [Tooltip("The renderer component of the Goal")]
        [SerializeField] private Renderer targetRenderer;

        [Tooltip("The material the Goal should have when it is hit by the Light")]
        [SerializeField] private Material targetWinMaterial;

        [Tooltip("The material the Goal should have when it is not hit by the Light")]
        [SerializeField] private Material targetDefaultMaterial;

        [Tooltip("The door that is supposed to be activated once the Goal is hit")]
        [SerializeField] private Collider doorCollider;

        [Header("Misc Settings")]
        [Tooltip("The speed at which the Skull rotates to the desired position")]
        [SerializeField] private float rotationSpeed = 3f;

        private Skull _skull;
        private Coroutine _turning;

        private LineRenderer _lineRenderer;
        private readonly List<Vector3> _reflectionPoints = new List<Vector3>();
        private readonly List<Vector3> _initialPosition = new List<Vector3>();
        private readonly List<Quaternion> _initialRotation = new List<Quaternion>();
        private readonly List<GameObject> _objectsToReset = new List<GameObject>();

        public bool TargetHit { get; set; }
        public bool Splitting { get; private set; }
        private bool _continueTracing = true;

        private void Awake()
        {
            targetRenderer.material = targetDefaultMaterial;
            _lineRenderer = GetComponent<LineRenderer>();
            foreach (Transform child in transform)
            {
                _initialPosition.Add(child.position);
                _initialRotation.Add(child.rotation);
                _objectsToReset.Add(child.gameObject);
            }

            if (!doorCollider) return;
            doorCollider.enabled = false;
        }

        private void Update()
        {
            _continueTracing = true;
            TargetHit = false;
            _reflectionPoints.Clear();

            var currentPosition = transform.position;
            var currentDirection = transform.forward;
            _reflectionPoints.Add(currentPosition);

            for (var i = 0; i <= reflections && _continueTracing; i++)
            {
                if (Physics.Raycast(currentPosition, currentDirection, out var hit, beamLength))
                {
                    Splitting = hit.collider.CompareTag("Skull");
                    _reflectionPoints.Add(hit.point);

                    switch (hit.collider.tag)
                    {
                        case "Mirror":
                            currentPosition = hit.point;
                            currentDirection = Reflect(currentDirection, hit.normal);
                            break;

                        case "Goal":
                            _reflectionPoints.Add(hit.point);
                            TargetHit = true;
                            _continueTracing = false;
                            break;

                        case "Death Trap":
                            _continueTracing = false;
                            StartCoroutine(ResetRiddle());
                            break;

                        case "Skull":
                            StartCoroutine(TurnSkull(currentDirection, hit.collider));
                            _continueTracing = false;
                            _skull = hit.collider.GetComponent<Skull>();
                            _skull.Split(reflections - i);
                            break;
                    }
                }
                else
                {
                    _reflectionPoints.Add(currentPosition + currentDirection * beamLength);
                    break;
                }
            }

            CheckWin();
            if (_reflectionPoints.Count > 1)
            {
                var lastPoint = _reflectionPoints[^1];
                var lastDir = (_reflectionPoints.Count >= 2)
                    ? (_reflectionPoints[^1] - _reflectionPoints[^2]).normalized
                    : currentDirection;

                if (!Physics.Raycast(lastPoint, lastDir, out _))
                {
                    _reflectionPoints.Add(lastPoint + lastDir * beamLength);
                }
            }

            _lineRenderer.positionCount = _reflectionPoints.Count;
            _lineRenderer.SetPositions(_reflectionPoints.ToArray());
        }
       
        private IEnumerator TurnSkull(Vector3 forward, Collider other)
        {
            var t = 0f;
            while (t < rotationSpeed)
            {
                t += Time.deltaTime;
                other.transform.rotation = Quaternion.Lerp(other.transform.rotation, Quaternion.LookRotation(forward),t / rotationSpeed);
                yield return null;
            }
        }

        private void CheckWin()
        {
            targetRenderer.material = TargetHit ? targetWinMaterial : targetDefaultMaterial;
            if (!doorCollider) return;
            doorCollider.enabled = TargetHit;
        }

        public Vector3 Reflect(Vector3 direction, Vector3 normal)
        {
            return Vector3.Reflect(direction, normal);
        }

        public IEnumerator ResetRiddle()
        {
            Debug.Log("DEATH");
            for (var i = 0; i < _objectsToReset.Count; i++)
            {
                Debug.Log("RESET");
                _objectsToReset[i].transform.position = _initialPosition[i];
                _objectsToReset[i].transform.rotation = _initialRotation[i];
                yield return null;
            }
        }

        
    }
}