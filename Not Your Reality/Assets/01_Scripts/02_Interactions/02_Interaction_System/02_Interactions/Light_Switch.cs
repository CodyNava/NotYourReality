using System.Collections.Generic;
using UnityEngine;

public class Light_Switch : Interactable_Base
{
    private bool _interacted;
    [SerializeField] private List<GameObject> effectedObjects;
    
    public override void OnInteract()
    {
        base.OnInteract();
        //Current Use: Turns effected Objects off or on
        if (!_interacted)
        {
            gameObject.transform.Rotate(-60,0,0);
            _interacted = true;
           
            foreach (var teeHee in effectedObjects)
            {
                teeHee.SetActive(true);
            }
        }
        else
        {
            gameObject.transform.Rotate(60,0,0);
            _interacted = false;
            foreach (var teeHee in effectedObjects)
            {
                teeHee.SetActive(false);
            }
        }
    }
}