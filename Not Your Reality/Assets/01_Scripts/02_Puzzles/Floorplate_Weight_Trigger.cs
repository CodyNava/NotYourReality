using System.Collections.Generic;
using UnityEngine;

public class Floorplate_Weight_Trigger : MonoBehaviour
{
    [Tooltip("The weight that needs to be reached for the plate to trigger")]
    [SerializeField] private float goalWeight;
    
    private List<Move_Object> _objects;

    private void Awake()
    {
        _objects = new List<Move_Object>();
        _objects.Clear();
    }

    private void CheckWin()
    {
        var currentWeight = 0f;
        foreach (var moveObject in _objects)
        {
            currentWeight += moveObject.Weight;
        }
        if (currentWeight >= goalWeight)
        {
            Debug.Log("You did it");
        }
        else
        {
            Debug.Log("Not enough");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        var moveObject = other.GetComponent<Move_Object>();
        _objects.Add(moveObject);
        CheckWin();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;
        var moveObject = other.GetComponent<Move_Object>();
        _objects.Remove(moveObject);
        CheckWin();
    }
}
