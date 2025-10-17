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

    private LineRenderer _lineRenderer;
    private readonly List<Vector3> _reflectionPoints = new List<Vector3>();

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        _reflectionPoints.Clear();
        _reflectionPoints.Add(transform.position);
        var currentPosition = transform.position;
        var currentDirection = transform.forward;
        for (var i = 0; i <= reflections; i++)
        {
            if (Physics.Raycast(currentPosition, currentDirection, out var hit, beamLength))
            {
                if (hit.collider.CompareTag("Mirror"))
                {
                    _reflectionPoints.Add(hit.point);
                    currentPosition = hit.point;
                    currentDirection = Reflect(currentDirection, hit.normal);
                }
                else if (hit.collider.CompareTag("Goal"))
                {
                    Win();
                }
            }
            else
            {
                _reflectionPoints.Add(currentPosition + currentDirection * beamLength);
                break;
            }
        }

        _lineRenderer.positionCount = _reflectionPoints.Count;
        _lineRenderer.SetPositions(_reflectionPoints.ToArray());
    }

    private void Win()
    {
        Debug.Log("You Win");
    }

    private Vector3 Reflect(Vector3 direction, Vector3 normal)
    {
        return Vector3.Reflect(direction, normal);
    }
}