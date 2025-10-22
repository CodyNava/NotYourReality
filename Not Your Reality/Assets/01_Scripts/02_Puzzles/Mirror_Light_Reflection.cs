using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mirror_Light_Reflection : MonoBehaviour
{ 
    [Space]
    [Tooltip("The length of the light beam for this puzzle")]
    [SerializeField] private float beamLength;

    [Tooltip("Allowed Reflections before the light stops")]
    [SerializeField] private int reflections;
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material targetWinMaterial;
    [SerializeField] private Material targetDefaultMaterial;
    [SerializeField] private Collider doorCollider;

    private LineRenderer _lineRenderer;
    private readonly List<Vector3> _reflectionPoints = new List<Vector3>();
    private bool _targetHit;

    private void Awake()
    {
        targetRenderer.material = targetDefaultMaterial;
        _lineRenderer = GetComponent<LineRenderer>();
        doorCollider.enabled = false;
    }

    private void Update()
    {
        _reflectionPoints.Clear();
        var currentPosition = transform.position;
        var currentDirection = transform.forward;
        _reflectionPoints.Add(currentPosition);
        for (var i = 0; i <= reflections; i++)
        {
            if (Physics.Raycast(currentPosition, currentDirection, out var hit, beamLength))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    currentPosition = hit.point + currentDirection * 0.01f;
                    i--; 
                    continue;
                }
                
                _reflectionPoints.Add(hit.point);
                
                if (hit.collider.CompareTag("Mirror"))
                {
                    currentPosition = hit.point;
                    currentDirection = Reflect(currentDirection, hit.normal);
                }
                else if (hit.collider.CompareTag("Goal"))
                {
                    _reflectionPoints.Add(hit.point);
                    _targetHit = true;
                    break;
                }
                else
                {
                    _targetHit = false;
                    break;
                }
            }
            else
            {
                _reflectionPoints.Add(currentPosition + currentDirection * beamLength);
                break;
            }
            CheckWin();
        }
        
        
        if (_reflectionPoints.Count > 1)
        {
            Vector3 lastPoint = _reflectionPoints[^1];
            Vector3 lastDir = (_reflectionPoints.Count >= 2)
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

    private void CheckWin()
    {
        targetRenderer.material = _targetHit? targetWinMaterial : targetDefaultMaterial;
        doorCollider.enabled = _targetHit;
    }

    private Vector3 Reflect(Vector3 direction, Vector3 normal)
    {
        return Vector3.Reflect(direction, normal);
    }
}