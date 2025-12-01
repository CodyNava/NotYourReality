using System.Collections;
using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
   public class OpenDoor : InteractableBase
   {
      [Space]
      [Tooltip("The drag of the door")]
      [SerializeField] private float drag;
      [Tooltip("The speed at which the door follows the mouse")]
      [SerializeField] private float pullStrength;
      [Header("Close and Lock Mechanism")]
      [Tooltip("The maximum velocity the door is allowed to have to close and stay shut if the angle is low enough")]
      [SerializeField] private float lockThreshold;
      [Tooltip("The angle at which the door closes and stays shut if the velocity is low enough")]
      [SerializeField] private float lockAngle;
      
      private bool _isHeld;
      private Camera _cam;
      private Rigidbody _rb;
      private float _initialYRotation;
      private Vector3 _initialCamForward;
      private Quaternion _lockRotation;
      private Vector3 _torque;
      private HingeJoint _joint;

      private void Awake()
      {
         _rb = GetComponent<Rigidbody>();
         _joint = GetComponent<HingeJoint>();
         _lockRotation = _rb.transform.rotation;
         TooltipMessage = "Hold E to Interact";
         
         
      }

      private IEnumerator Start()
      {
         while (!_cam)
         {
            _cam = Camera.main;
            yield return null;
         }
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
         if (!_isHeld)
         {
            LockDoor();
            return;
         }
         RotateDoor();
      }

      private void RotateDoor()
      {
         var currentCamForward = _cam.transform.forward;
         currentCamForward.y = 0;
         _initialCamForward.y = 0;

         currentCamForward.Normalize();
         _initialCamForward.Normalize();

         var angle = -Vector3.SignedAngle(_initialCamForward, currentCamForward, Vector3.up);
         var targetYRotation = _initialYRotation + angle;

         var currentY = transform.eulerAngles.y;
         var moveAngle = Mathf.DeltaAngle(currentY, targetYRotation);

         _torque = Vector3.up * moveAngle * pullStrength;

         _torque -= _rb.angularVelocity * drag;

         _rb.AddTorque(_torque, ForceMode.Acceleration);
      }

      private void LockDoor()
      {
         var hingeVelocity = Vector3.Project(_rb.angularVelocity, Vector3.up).magnitude;
         var angle = Quaternion.Angle(_rb.rotation, _lockRotation);

         if (hingeVelocity < lockThreshold && angle < lockAngle)
         {
            _joint.useLimits = false;
            _rb.angularVelocity = Vector3.zero;
            _rb.rotation = _lockRotation;
         }
         else { _joint.useLimits = true; }
      }

      public void Release() { _isHeld = false; }
   }
}