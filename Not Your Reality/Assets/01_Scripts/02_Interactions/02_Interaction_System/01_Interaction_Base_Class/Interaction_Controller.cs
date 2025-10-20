using _01_Scripts._02_Interactions._02_Interaction_System._02_Interactions;
using UnityEngine;

public class Interaction_Controller : MonoBehaviour
{
    [Header("Data")] [SerializeField] private Interaction_Input_Data interactionInputData;
    [SerializeField] private Interaction_Data interactionData;

    [Header("Spherecast Settings")] [SerializeField]
    private float rayDistance;

    [SerializeField] private float raySphereRadius;
    [SerializeField] private LayerMask interactableLayer;

    private Camera _camera;
    private bool _interacting;
    private float _holdTimer;

    private Interactable_Base _hoveredObject;
    private Interactable_Base _selectedObject;

    private void Awake()
    {
        _camera = FindFirstObjectByType<Camera>();
    }

    private void Update()
    {
        CheckForInteractable();
        CheckForInput();
    }

    private void CheckForInteractable()
    {
        var ray = new Ray(_camera.transform.position, _camera.transform.forward);
        var hitSomething = Physics.SphereCast(ray, raySphereRadius, out var hit, rayDistance, interactableLayer);
        if (hitSomething)
        {
            var interactableBase = hit.transform.GetComponent<Interactable_Base>();
            if (interactableBase == null) return;
            _hoveredObject = interactableBase;
            if (interactionData.IsEmpty() || !interactionData.IsSameInteractable(interactableBase))
            {
                interactionData.InteractableBase = interactableBase;
            }
        }
        else
        {
            if (!_interacting)
            {
                interactionData.ResetData();
            }
        }
    }

    private void CheckForInput()
    {
        if (interactionData.IsEmpty()) return;

        var interactable = interactionData.InteractableBase;
        if (!interactable.IsInteractable) return;

        if (interactionInputData.InteractedClicked && _hoveredObject != null)
        {
            _interacting = true;
            _selectedObject = _hoveredObject;
            
            if (_selectedObject.HoldInteract && _selectedObject.HoldDuration > 0f)
            {
                _holdTimer = 0f;
            }
        }

        if (interactionInputData.InteractedReleased)
        {
            _interacting = false;
            _holdTimer = 0f;
            switch (_selectedObject)
            {
                case Move_Object moveObject:
                    moveObject.Release();
                    break;
                case Puzzle_Mirror mirror:
                    mirror.Release();
                    break;
                case OpenDoor door:
                    door.Release();
                    break;
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
            case true when _selectedObject.HoldDuration <= 0f:
                _selectedObject.OnInteract();
                break;
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