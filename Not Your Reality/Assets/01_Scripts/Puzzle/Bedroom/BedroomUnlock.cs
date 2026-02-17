using System.Collections.Generic;
using FMODUnity;
using Interactions.Interaction_System.Interactions;
using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;

namespace Puzzle.Bedroom
{
    public class BedroomUnlock : MonoBehaviour
    {
        [SerializeField] private List<InspectableItem> itemsToUnlock;

        [SerializeField] private EventReference unlockSound;

        [SerializeField] private DoorHandle doorHandle1;
        [SerializeField] private DoorHandle doorHandle2;
        [SerializeField] private DoorHandle doorHandle3;
        [SerializeField] private DoorHandle doorHandle4;

        [SerializeField] private List<GameObject> allDoorHandles;

        private int _loops;

        private void Awake()
        {
            itemsToUnlock = new List<InspectableItem>();
            foreach (var handle in allDoorHandles)
            {
                handle.layer = LayerMask.NameToLayer("Default");
                if (handle.TryGetComponent<DoorHandle>(out var doorHandle))
                {
                    doorHandle.IsInteractable = false;
                }
            }
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
                        for (var i = 0; i < 4; i++)
                        {
                            allDoorHandles[i].layer = LayerMask.NameToLayer("Interactable");
                            if (allDoorHandles[i].TryGetComponent<DoorHandle>(out var doorHandle))
                            {
                                doorHandle.IsInteractable = true;
                            }
                        }
                        RuntimeManager.PlayOneShot(unlockSound, doorHandle1.transform.position);
                        itemsToUnlock.Clear();
                        _loops++;
                    }

                    break;
                case 1:
                    if (itemsToUnlock.Count == 4)
                    {
                        for (var i = 4; i < 8; i++)
                        {
                            allDoorHandles[i].layer = LayerMask.NameToLayer("Interactable");
                            if (allDoorHandles[i].TryGetComponent<DoorHandle>(out var doorHandle))
                            {
                                doorHandle.IsInteractable = true;
                            }
                        }
                    
                        RuntimeManager.PlayOneShot(unlockSound, doorHandle2.transform.position);
                        itemsToUnlock.Clear();
                        _loops++;
                    }

                    break;
                case 2:
                    if (itemsToUnlock.Count == 4)
                    {
                        for (var i = 8; i < 12; i++)
                        {
                            allDoorHandles[i].layer = LayerMask.NameToLayer("Interactable");
                            if (allDoorHandles[i].TryGetComponent<DoorHandle>(out var doorHandle))
                            {
                                doorHandle.IsInteractable = true;
                            }
                        }
                        RuntimeManager.PlayOneShot(unlockSound, doorHandle3.transform.position);
                        itemsToUnlock.Clear();
                        _loops++;
                    }

                    break;
                case 3:
                    if (itemsToUnlock.Count == 3)
                    {
                        for (var i = 12; i < 16; i++)
                        {
                            allDoorHandles[i].layer = LayerMask.NameToLayer("Interactable");
                            if (allDoorHandles[i].TryGetComponent<DoorHandle>(out var doorHandle))
                            {
                                doorHandle.IsInteractable = true;
                            }
                        }
                        RuntimeManager.PlayOneShot(unlockSound, doorHandle4.transform.position);
                        itemsToUnlock.Clear();
                    }

                    break;
            }
        }
    }
}