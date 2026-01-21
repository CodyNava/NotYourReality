using System;
using System.GlobalEventSystem;
using FMODUnity;
using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
   public class InspectingTouch : InteractableBase
    {
        private int _index;

        [SerializeField] private StudioEventEmitter emitter;
        [SerializeField] private RoomVoiceManager manager;

        public override void OnInteract()
        {
            base.OnInteract();

            if (emitter != null)
            {
                emitter.Play();
            }

            if (manager != null)
            {
                manager.OnVoiceTriggered(gameObject);
            }

            if (gameObject.name.Equals("Phone")) { GlobalEventManager.OnPhoneTouched(); }

         if (gameObject.name.Equals("Cake")) { GlobalEventManager.OnCakeTouched(); }
      }

      private void Update() { TooltipMessage = IsInteractable ? "Hold E to Touch" : ""; }
   }
}