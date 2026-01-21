using System;
using System.Collections.Generic;
using UnityEngine;

public class EnablePNGRoom : MonoBehaviour
{
    [Header("Refernces of PNG Props")]
    [SerializeField] private List<GameObject> pngProps;

    private void OnTriggerEnter(Collider other)
    {
        if (pngProps == null) return;

        foreach (GameObject prop in pngProps)
        {
            prop.SetActive(true);
        }
    }
}