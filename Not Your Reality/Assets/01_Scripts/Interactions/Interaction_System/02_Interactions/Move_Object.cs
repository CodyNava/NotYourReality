using UnityEngine;

public class Move_Object : Interactable_Base
{
    [Tooltip("The distance at which the player holds the object")]
    [SerializeField] private float holdingDistance;
    [Tooltip("The speed at which the object follows the mouse")]
    [SerializeField] private float dragSpeed;
    [Tooltip("OPTIONAL: Weight for interacting with trigger plates")]
    [SerializeField] private float weight;

    public float Weight => weight;

    private Rigidbody _rb;
    private bool _isHeld;
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnInteract()
    {
        base.OnInteract();
        _isHeld = true;
        _rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (!_isHeld) return;
        MoveItem();
    }

    private void MoveItem()
    {
        var currentPosition = gameObject.transform.position;
        var targetPosition = _cam.transform.position + _cam.transform.forward * holdingDistance;
        var time = Time.deltaTime * dragSpeed;
        var lerp = Vector3.Lerp(currentPosition, targetPosition, time);
        _rb.MovePosition(lerp);
    }

    public void Release()
    {
        _isHeld = false;
        _rb.useGravity = true;
    }
}