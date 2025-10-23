using System.Collections.Generic;
using UnityEngine;

public class Path_Of_Truth : MonoBehaviour
{
    [Tooltip("Pull the tiles you want to be safe in here")]
    [SerializeField] private List<Collider> safeTiles;

    private void Awake()
    {
        foreach (Transform child in gameObject.transform)
        {
            var childCollider = child.GetComponent<Collider>();
            child.tag = safeTiles.Contains(childCollider) ? "Safe" : "Not Safe";
        }
    }
}