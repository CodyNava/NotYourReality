using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
   [ExecuteInEditMode]
   public class MirrorLightReflection : MonoBehaviour
   {
      [Space]
      [Tooltip("The length of the light beam for this puzzle")]
      [SerializeField] private float beamLength;
      public float BeamLength => beamLength;

      [Tooltip("Allowed Reflections before the light stops")]
      [SerializeField] private int reflections;
      
      [SerializeField] private Renderer targetRenderer;
      [SerializeField] private Material targetWinMaterial;
      [SerializeField] private Material targetDefaultMaterial;
      [SerializeField] private Collider doorCollider;

      private Skull _skull;

      private LineRenderer _lineRenderer;
      private readonly List<Vector3> _reflectionPoints = new List<Vector3>();
      
      public bool TargetHit { get; set; }
      private bool _continueTracing = true;

      private void Awake()
      {
         targetRenderer.material = targetDefaultMaterial;
         _lineRenderer = GetComponent<LineRenderer>();
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
                         // TODO: GAME OVER and RESET
                         _continueTracing = false;
                         break;

                     case "Skull":
                         _skull = hit.collider.GetComponent<Skull>();
                         _skull.Split(reflections - i);
                         _continueTracing = false;
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
            Vector3 lastPoint = _reflectionPoints[^1];
            Vector3 lastDir = (_reflectionPoints.Count >= 2) ?
               (_reflectionPoints[^1] - _reflectionPoints[^2]).normalized : currentDirection;

            if (!Physics.Raycast(lastPoint, lastDir, out _))
            {
               _reflectionPoints.Add(lastPoint + lastDir * beamLength);
            }
         }

         _lineRenderer.positionCount = _reflectionPoints.Count;
         _lineRenderer.SetPositions(_reflectionPoints.ToArray());
      }

      private void CheckWin()
      {
         targetRenderer.material = TargetHit ? targetWinMaterial : targetDefaultMaterial;
         doorCollider.enabled = TargetHit;
      }

      public Vector3 Reflect(Vector3 direction, Vector3 normal) { return Vector3.Reflect(direction, normal); }
   }
}