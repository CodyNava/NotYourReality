using System.Collections.Generic;
using UnityEngine;

public class Light_Switch : Interactable_Base
{
    [SerializeField] private List<GameObject> effectedObjects;
    private bool _interacted;
    
    public override void OnInteract()
    {
        base.OnInteract();
        //Current Use: Turns effected Objects off or on
        if (!_interacted)
        {
            ToggleOn();
            foreach (var teeHee in effectedObjects)
            {
                teeHee.SetActive(true);
            }
        }
        else
        {
            ToggleOff();
            foreach (var teeHee in effectedObjects)
            {
                teeHee.SetActive(false);
            }
        }
    }

    private void ToggleOn()
    {
        gameObject.transform.Rotate(-60,0,0);
        _interacted = true;
    }
    private void ToggleOff()
    {
        gameObject.transform.Rotate(60,0,0);
        _interacted = false;
    }
}