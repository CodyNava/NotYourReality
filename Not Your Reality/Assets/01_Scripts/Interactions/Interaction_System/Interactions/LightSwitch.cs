using System.Collections.Generic;
using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
    public class LightSwitch : InteractableBase
    {
        private List<GameObject> _effectedObjects;
        private bool _interacted;

        private void Awake()
        {
            foreach (Transform child in transform)
            {
                _effectedObjects.Add(child.gameObject);
            }
        }

        public override void OnInteract()
        {
            base.OnInteract();
            if (!_interacted)
            {
                ToggleOn();
                foreach (var effectedObject in _effectedObjects)
                {
                    effectedObject.SetActive(true);
                }
            }
            else
            {
                ToggleOff();
                foreach (var effectedObject in _effectedObjects)
                {
                    effectedObject.SetActive(false);
                }
            }
        }

        private void ToggleOn()
        {
            gameObject.transform.Rotate(-60, 0, 0);
            _interacted = true;
        }

        private void ToggleOff()
        {
            gameObject.transform.Rotate(60, 0, 0);
            _interacted = false;
        }
    }
}