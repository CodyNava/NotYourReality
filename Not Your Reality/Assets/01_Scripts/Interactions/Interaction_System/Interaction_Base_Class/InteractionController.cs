using System.Collections;
using Interactions.Interaction_System.Interactions;
using Interactions.Interaction_System.Interactions.Door_Rework;
using Puzzle.Desert_Reflection_Room;
using UnityEngine;
namespace Interactions.Interaction_System.Interaction_Base_Class
{
    public class InteractionController : MonoBehaviour
    {
        [Header("Data")] [SerializeField] private InputHandler inputHandler;

        [Header("UI")] [SerializeField] private InteractionUIPanel interactionUIPanel;

        [Header("Spherecast Settings")] [SerializeField]
        private float rayDistance;

        [Header("Crosshair")] [SerializeField] private RectTransform crosshair;

        [SerializeField] private float raySphereRadius;
        [SerializeField] private LayerMask interactableLayer;

        private Camera _camera;
        private bool _interacting;
        private float _holdTimer;

        private InteractionInputData _interactionInputData;
        private InteractionData _interactionData;

        private InteractableBase _hoveredObject;
        private InteractableBase _selectedObject;

        private void Awake()
        {
            _camera = Camera.main;
            _interactionInputData = ScriptableObject.CreateInstance<InteractionInputData>();
            _interactionData = ScriptableObject.CreateInstance<InteractionData>();
            inputHandler.Initialize(_interactionInputData);
        }

        private void Update()
        {
            CheckForInteractable();
            CheckForInput();
            UpdateCrosshair();
        }

        private void UpdateCrosshair()
        {
            crosshair.localScale = _interacting ? Vector3.one * 1.5f : Vector3.one;
        }

        private void CheckForInteractable()
        {
       
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            var hitSomething = Physics.SphereCast(ray, raySphereRadius, out var hit, rayDistance, interactableLayer);
            
            if (hitSomething)
            {
                var dirToHit = hit.point - _camera.transform.position;
                var distanceToHit = dirToHit.magnitude;
                if (Physics.Raycast(_camera.transform.position, 
                                    _camera.transform.TransformDirection(Vector3.forward), distanceToHit, ~interactableLayer))
                {
                    _hoveredObject = null;
                    if (!_interacting)
                    {
                        interactionUIPanel.Reset();
                        _interactionData.ResetData();
                    }

                    return;
                }
                
                var interactableBase = hit.transform.GetComponent<InteractableBase>();
                if (!interactableBase) return;
                
                _hoveredObject = interactableBase;
                if (_interactionData.IsEmpty() || !_interactionData.IsSameInteractable(interactableBase))
                {
                    _interactionData.InteractableBase = interactableBase;
                    interactionUIPanel.SetTooltip(interactableBase.TooltipMessage);
                }
            }
            else if (!_interacting)
            {
                interactionUIPanel.Reset();
                _interactionData.ResetData();

            }
        }

       

        private void CheckForInput()
        {
            if (_interactionData.IsEmpty()) return;

            var interactable = _interactionData.InteractableBase;
            if (!interactable.IsInteractable) return;

            if (_interactionInputData.InteractedClicked && _hoveredObject)
            {
                _interacting = true;
                interactionUIPanel.Reset();
                _selectedObject = _hoveredObject;

                if (_selectedObject.HoldInteract && _selectedObject.HoldDuration > 0f)
                {
                    _holdTimer = 0f;
                }
            }

            if (_interactionInputData.InteractedReleased)
            {
                _interacting = false;
                interactionUIPanel.SetTooltip(interactable.TooltipMessage);
                _holdTimer = 0f;
                switch (_selectedObject)
                {
                    case MoveObject moveObject: moveObject.Release(); break;
                    case PuzzleMirror mirror: mirror.Release(); break;
                    case DoorHandle door: door.Release(); break;
                }

                _selectedObject = null;
            }

            if (!_interacting) return;
            if (_selectedObject == null) return;
            if (!_selectedObject.IsInteractable) return;
            switch (_selectedObject.HoldInteract)
            {
                case false:
                    _selectedObject.OnInteract();
                    _interacting = false;
                    _selectedObject = null;
                    break;
                case true when _selectedObject.HoldDuration <= 0f: _selectedObject.OnInteract(); break;
                case true when _selectedObject.HoldDuration > 0f:
                {
                    _holdTimer += Time.deltaTime;
                    if (_holdTimer >= _selectedObject.HoldDuration)
                    {
                        _selectedObject.OnInteract();
                        _interacting = false;
                        _selectedObject = null;
                    }

                    break;
                }
            }
        }
    }
}