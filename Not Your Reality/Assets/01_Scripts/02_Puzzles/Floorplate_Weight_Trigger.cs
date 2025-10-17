using System.Collections.Generic;
using UnityEngine;

public class Floorplate_Weight_Trigger : MonoBehaviour
{
    [Tooltip("The weight that needs to be reached for the plate to trigger")]
    [SerializeField] private float goalWeight;

    [Tooltip("The distance the platform will move once it is activated")]
    [SerializeField] private float platformMoveDistance;

    [Tooltip("The speed the platform will move at once it is activated")]
    [SerializeField] private float platformMoveSpeed;
    [SerializeField] private GameObject winTarget;
    
    [SerializeField]private List<Move_Object> _objects;
    private Rigidbody _rb;
    private bool _hasWon;

    private Vector3 _startPos;
    private Vector3 _targetPos;

    private void Awake()
    {
        _objects = new List<Move_Object>();
        _objects.Clear();
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        _targetPos = _startPos;
    }

    private void FixedUpdate()
    {
        var moveTo = Vector3.Lerp(transform.position, _targetPos, Time.fixedDeltaTime * platformMoveSpeed);
        _rb.MovePosition(moveTo);
    }

    private void CheckWin()
    {
        var currentWeight = 0f;
        foreach (var moveObject in _objects)
        {
            currentWeight += moveObject.Weight;
        }
        if (currentWeight >= goalWeight && !_hasWon)
        {
            _targetPos = _startPos + Vector3.down * platformMoveDistance;
            _hasWon = true;
            winTarget.SetActive(false);
            Debug.Log("You did it");
        }
        else if (currentWeight < goalWeight && _hasWon)
        {
            _hasWon = false;
            _targetPos = _startPos;
            winTarget.SetActive(true);
            Debug.Log("Not enough");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Weight")) return;
        var moveObject = other.GetComponent<Move_Object>();
        _objects.Add(moveObject);
        CheckWin();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Weight")) return;
        var moveObject = other.GetComponent<Move_Object>();
        _objects.Remove(moveObject);
        CheckWin();
    }
}