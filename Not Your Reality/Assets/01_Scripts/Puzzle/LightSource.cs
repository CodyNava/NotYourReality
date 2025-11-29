using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
   public class LightSource : MonoBehaviour
   {
      [Tooltip("The length of the light beam")]
      [SerializeField] private float beamLength;
      public float BeamLength => beamLength;

      [Tooltip("Allowed reflections before the light stops")]
      [SerializeField] private int reflections;

      [Tooltip("The speed at which the skull rotates to the desired position")]
      [Range(0f, 0.01f)]
      [SerializeField] private float rotationSpeed = 0.005f;
      public float RotationSpeed => rotationSpeed;

      [SerializeField] private MirrorLightReflection mirrorLightReflection;

      private Skull _skull;

      private LineRenderer _lineRenderer;
      private readonly List<Vector3> _reflectionPoints = new();

      private bool _tracing = true;

      private void Awake() { _lineRenderer = GetComponent<LineRenderer>(); }

      private void Update()
      {
         Skull.ClearAllSplits();
         foreach (var goal in mirrorLightReflection.goals) { goal.ClearHits(); }

         _tracing = true;
         _reflectionPoints.Clear();

         var currentPosition = transform.position;
         var currentDirection = transform.forward;
         _reflectionPoints.Add(currentPosition);

         for (var i = 0; i <= reflections && _tracing; i++)
         {
            if (Physics.Raycast(currentPosition, currentDirection, out var hit, beamLength))
            {
               _reflectionPoints.Add(hit.point);

               switch (hit.collider.tag)
               {
                  case "Mirror":
                     currentPosition = hit.point;
                     currentDirection = Reflect(currentDirection, hit.normal);
                  break;

                  case "Goal":
                     _reflectionPoints.Add(hit.point);

                     foreach (var goal in mirrorLightReflection.goals)
                     {
                        if (hit.collider !=  goal.GetComponent<Collider>()) continue;
                        if (goal.BeenHit()) break;
                        goal.RegisterHit(hit.point);
                     }

                     _tracing = false;
                  break;

                  case "Death Trap":
                     _tracing = false;
                     StartCoroutine(mirrorLightReflection.ResetRiddle());
                  break;

                  case "Skull":
                     _tracing = false;
                     _skull = hit.collider.GetComponent<Skull>();
                     _skull.Splitting = true;
                     _skull.Reached = false;
                     _skull.Reached = _skull.HasReached(
                        _skull.transform.rotation,
                        Quaternion.LookRotation(currentDirection)
                     );

                     if (_skull.Reached) { _skull.transform.rotation = Quaternion.LookRotation(currentDirection); }
                     else
                     {
                        _skull.transform.rotation = Quaternion.Slerp(
                           _skull.transform.rotation,
                           Quaternion.LookRotation(currentDirection),
                           rotationSpeed
                        );
                     }

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

         mirrorLightReflection.CheckWin();
         if (_reflectionPoints.Count > 1)
         {
            var lastPoint = _reflectionPoints[^1];
            var lastDir = (_reflectionPoints.Count >= 2) ? (_reflectionPoints[^1] - _reflectionPoints[^2]).normalized :
               currentDirection;

            if (!Physics.Raycast(lastPoint, lastDir, out _))
            {
               _reflectionPoints.Add(lastPoint + lastDir * beamLength);
            }
         }

         _lineRenderer.positionCount = _reflectionPoints.Count;
         _lineRenderer.SetPositions(_reflectionPoints.ToArray());
      }

      public static Vector3 Reflect(Vector3 direction, Vector3 normal) { return Vector3.Reflect(direction, normal); }
   }
}