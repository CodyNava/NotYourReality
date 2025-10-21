using _01_Scripts._02_Interactions._02_Interaction_System._02_Interactions;
using UnityEngine;

public class Interaction_Controller : MonoBehaviour
{
   [Header("Data")]
   [SerializeField] private Input_Handler inputHandler;

   [Header("UI")]
   [SerializeField] private InteractionUIPanel interactionUIPanel;

   [Header("Spherecast Settings")]
   [SerializeField] private float rayDistance;

   [Header("Crosshair")]
   [SerializeField] private RectTransform crosshair;

   [SerializeField] private float raySphereRadius;
   [SerializeField] private LayerMask interactableLayer;

   private Camera _camera;
   private bool _interacting;
   private float _holdTimer;

   private Interaction_Input_Data _interactionInputData;
   private Interaction_Data _interactionData;

   private Interactable_Base _hoveredObject;
   private Interactable_Base _selectedObject;

   private void Awake()
   {
      _camera = GetComponentInChildren<Camera>();
      _interactionInputData = ScriptableObject.CreateInstance<Interaction_Input_Data>();
      _interactionData = ScriptableObject.CreateInstance<Interaction_Data>();
      inputHandler.Intialize(_interactionInputData);
   }

   private void Update()
   {
      CheckForInteractable();
      CheckForInput();
      UpdateCrosshair();
   }

   private void UpdateCrosshair() { crosshair.localScale = _interacting ? Vector3.one * 1.5f : Vector3.one; }

   private void CheckForInteractable()
   {
      var ray = new Ray(_camera.transform.position, _camera.transform.forward);
      var hitSomething = Physics.SphereCast(ray, raySphereRadius, out var hit, rayDistance, interactableLayer);
      if (hitSomething)
      {
         var interactableBase = hit.transform.GetComponent<Interactable_Base>();
         if (interactableBase == null) return;
         _hoveredObject = interactableBase;
         if (_interactionData.IsEmpty() || !_interactionData.IsSameInteractable(interactableBase))
         {
            _interactionData.InteractableBase = interactableBase;
            interactionUIPanel.SetTooltip(interactableBase.TooltipMessage);
         }
      }
      else
      {
         if (!_interacting)
         {
            interactionUIPanel.Reset();
            _interactionData.ResetData();
         }
      }
   }

   private void CheckForInput()
   {
      if (_interactionData.IsEmpty()) return;

      var interactable = _interactionData.InteractableBase;
      if (!interactable.IsInteractable) return;

      if (_interactionInputData.InteractedClicked && _hoveredObject != null)
      {
         _interacting = true;
         interactionUIPanel.Reset();
         _selectedObject = _hoveredObject;

         if (_selectedObject.HoldInteract && _selectedObject.HoldDuration > 0f) { _holdTimer = 0f; }
      }

      if (_interactionInputData.InteractedReleased)
      {
         _interacting = false;
         interactionUIPanel.SetTooltip(interactable.TooltipMessage);
         _holdTimer = 0f;
         switch (_selectedObject)
         {
            case Move_Object moveObject: moveObject.Release(); break;
            case Puzzle_Mirror mirror:   mirror.Release(); break;
            case OpenDoor door:          door.Release(); break;
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