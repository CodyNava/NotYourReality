using UnityEngine;

public class Puzzle_Mirror : Interactable_Base
{
    [Space]
    [Tooltip("The speed at which the object follows the mouse")]
    [SerializeField] private float dragSpeed;
    
    private bool _isHeld;
    private Camera _cam;
    private Rigidbody _rigidbody;
    private float _initialYRotation;
    private Vector3 _initialCamForward;

    private void Awake()
    {
        _cam = FindFirstObjectByType<Camera>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnInteract()
    {
        base.OnInteract();
        _isHeld = true;
        _initialYRotation = transform.eulerAngles.y;
        _initialCamForward = _cam.transform.forward;
    }

    private void Update()
    {
        if (!_isHeld) return;
        RotateMirror();
    }

    private void RotateMirror()
    {
        var currentCamForward = _cam.transform.forward;
        currentCamForward.y = 0;
        _initialCamForward.y = 0;
        
        currentCamForward.Normalize();
        _initialCamForward.Normalize();

        var angle = -Vector3.SignedAngle(_initialCamForward, currentCamForward, Vector3.up);
        var targetYRotation = _initialYRotation + angle;

        var targetRotation = Quaternion.Euler(0, targetYRotation, 0);
        
        var time = Time.deltaTime * dragSpeed;
        var rotate = Quaternion.RotateTowards(transform.rotation, targetRotation, time);
        _rigidbody.MoveRotation(rotate);
    }

    public void Release()
    {
        _isHeld = false;
    }
}
