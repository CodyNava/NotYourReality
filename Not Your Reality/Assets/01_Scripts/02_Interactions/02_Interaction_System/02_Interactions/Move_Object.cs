using UnityEngine;

public class Move_Object : Interactable_Base
{
    [SerializeField] private float distance;
    [SerializeField] private float speed;
    
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnInteract()
    {
        base.OnInteract();
        _rb.useGravity = false;
        MoveItem();
    }

    private void MoveItem()
    {
        var cam = FindFirstObjectByType<Camera>();
        var currentPosition = gameObject.transform.position;
        var targetPosition = cam.transform.position + cam.transform.forward * distance;
        var time = Time.deltaTime * speed;
        var lerp = Vector3.Lerp(currentPosition, targetPosition, time);
        _rb.MovePosition(lerp);
    }

    public void Release()
    {
        _rb.useGravity = true;
    }
}