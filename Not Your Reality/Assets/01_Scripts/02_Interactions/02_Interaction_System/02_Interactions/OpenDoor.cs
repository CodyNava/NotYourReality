using System;
using UnityEngine;

namespace _01_Scripts._02_Interactions._02_Interaction_System._02_Interactions
{
   public class OpenDoor : Interactable_Base
   {
      [Space]
      [SerializeField] private float drag;
      [Tooltip("The speed at which the object follows the mouse")]
      [SerializeField] private float pullStrength;
      [SerializeField] private float lockThreshold;
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
         _cam = FindFirstObjectByType<Camera>();
         _rb = GetComponent<Rigidbody>();
         _joint = GetComponent<HingeJoint>();
         _lockRotation = _rb.transform.rotation;
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