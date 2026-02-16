using System.Collections.Generic;
using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;

namespace System.Tools
{
    public class InteractableChecker : MonoBehaviour
    {
        [SerializeField] private DoorHandle handle;
        [SerializeField] private List<GameObject> allDoorHandles;
        private bool _isWon;

        private void Start()
        {
            foreach (var handles in allDoorHandles)
            {
                handles.layer =  LayerMask.NameToLayer("Default");
            }
        }

        private void Update()
        {
            if (_isWon) return;
            if (!handle.IsInteractable) return;
            
            foreach (var handles in allDoorHandles)
            {
                handles.layer = LayerMask.NameToLayer("Interactable");
            }
            _isWon = true;
        }
    }
}