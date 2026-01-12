using System;
using UnityEngine;

public class BedroomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] propSets;

    private int _counter = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (_counter >= propSets.Length) return;

        propSets[_counter].SetActive(false);
        _counter++;
        propSets[_counter].SetActive(true);
    }
}