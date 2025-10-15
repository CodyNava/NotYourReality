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
            if (interactionData.IsEmpty() || !interactionData.IsSameInteractable(interactableBase))
            {
                interactionData.InteractableBase = interactableBase;
            }
        }
        else
        {
            interactionData.ResetData();
        }
    }

    private void CheckForInput()
    {
        if (interactionData.IsEmpty())
        {
            return;
        }

        if (interactionInputData.InteractedClicked)
        {
            _interacting = true;
            _holdTimer = 0f;
        }

        if (interactionInputData.InteractedReleased)
        {
            _interacting = false;
            _holdTimer = 0f;
        }

        if (!_interacting) return;
        if (!interactionData.InteractableBase.IsInteractable) return;

        if (interactionData.InteractableBase.HoldInteract)
        {
            _holdTimer += Time.deltaTime;
            if (!(_holdTimer >= interactionData.InteractableBase.HoldDuration)) return;
        }
        interactionData.Interact();
        _interacting = false;
    }
}