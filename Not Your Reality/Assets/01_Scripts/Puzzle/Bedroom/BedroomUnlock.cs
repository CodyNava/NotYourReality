using System;
using System.Collections.Generic;
using FMODUnity;
using Interactions.Interaction_System.Interactions;
using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;

public class BedroomUnlock : MonoBehaviour
{
    [SerializeField] private List<InspectableItem> itemsToUnlock;

    [SerializeField] private EventReference unlockSound;
    
    [SerializeField] private DoorHandle doorHandle1;
    [SerializeField] private DoorHandle doorHandle2;
    [SerializeField] private DoorHandle doorHandle3;
    [SerializeField] private DoorHandle doorHandle4;

    private int _loops;

    private void Awake()
    {
        itemsToUnlock = new List<InspectableItem>();
    }

    public void AddItem(InspectableItem item)
    {
        if (itemsToUnlock.Contains(item)) return;
        
        itemsToUnlock.Add(item);
        CheckForAllItems();
    }

    private void CheckForAllItems()
    {
        switch (_loops)
        {
            case 0:
                if (itemsToUnlock.Count == 4)
                {
                    doorHandle1.IsInteractable = true;
                    RuntimeManager.PlayOneShot(unlockSound, doorHandle1.transform.position);
                    itemsToUnlock.Clear();
                    _loops++;
                }
                break;
            case 1:
                if (itemsToUnlock.Count == 4)
                {
                    doorHandle2.IsInteractable = true;
                    RuntimeManager.PlayOneShot(unlockSound, doorHandle2.transform.position);
                    itemsToUnlock.Clear();
                    _loops++;
                }
                break;
            case 2:
                if (itemsToUnlock.Count == 4)
                {
                    doorHandle3.IsInteractable = true;
                    RuntimeManager.PlayOneShot(unlockSound, doorHandle3.transform.position);
                    itemsToUnlock.Clear();
                    _loops++;
                }
                break;
            case 3:
                if (itemsToUnlock.Count == 3)
                {
                    doorHandle4.IsInteractable = true;
                    RuntimeManager.PlayOneShot(unlockSound, doorHandle4.transform.position);
                    itemsToUnlock.Clear();
                }
                break;
        }
    }
}
