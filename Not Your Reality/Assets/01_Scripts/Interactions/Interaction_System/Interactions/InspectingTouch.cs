using System;
using System.GlobalEventSystem;
using FMODUnity;
using Interactions.Interaction_System.Interaction_Base_Class;
using Interactions.Interaction_System.Interactions.Door_Rework;
using Puzzle.Bedroom;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
    public class InspectingTouch : InteractableBase
    {
        private int _index;

        [SerializeField] private StudioEventEmitter emitter;
        [SerializeField] private RoomVoiceManager manager;

        [SerializeField] private BedroomUnlock bedroomUnlock;

        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Sprite newMaterial;

        private Material _instancedMaterial;
        private Texture _originalTexture;
        private bool _initialized;

        private void Awake()
        {
            if (targetRenderer == null) return;

            _instancedMaterial = targetRenderer.material;
            _originalTexture = _instancedMaterial.mainTexture;
            _initialized = true;
        }

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

            if (bedroomUnlock != null)
                bedroomUnlock.AddItem(this);

            if (_initialized && newMaterial != null)
            {
                _instancedMaterial.mainTexture = newMaterial.texture;
            }

            if (gameObject.name.Equals("Phone"))
            {
                GlobalEventManager.OnPhoneTouched();
            }

            if (gameObject.name.Equals("Cake"))
            {
                GlobalEventManager.OnCakeTouched();
            }
        }

        private void Update()
        {
            TooltipMessage = IsInteractable ? "Press E to Touch" : "";
        }

        private void OnDestroy()
        {
            RestoreOriginalTexture();
        }

        private void OnDisable()
        {
            RestoreOriginalTexture();
        }

        private void RestoreOriginalTexture()
        {
            if (!_initialized || _instancedMaterial == null)
                return;

            _instancedMaterial.mainTexture = _originalTexture;
        }
    }
}