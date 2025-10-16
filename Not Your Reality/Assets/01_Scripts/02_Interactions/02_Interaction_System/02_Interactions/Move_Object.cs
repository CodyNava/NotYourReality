

using UnityEngine;

public class Move_Object : Interactable_Base
{
    [SerializeField] private float distance;
    [SerializeField] private float speed;
    public override void OnInteract()
    {
        base.OnInteract();
        MoveItem();
    }

    private void MoveItem()
    {
        var cam = FindFirstObjectByType<Camera>();
        var rb = GetComponent<Rigidbody>();
        
        var currentPosition = gameObject.transform.position;
        var targetPosition = cam.transform.position + cam.transform.forward * distance;
        var time = Time.deltaTime * speed;
        
        var lerp = Vector3.Lerp(currentPosition, targetPosition, time);
        rb.MovePosition(lerp);
    }
}