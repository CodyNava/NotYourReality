using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
   public class PuzzleMirror : InteractableBase
   {
      [Header("Mirror Settings")]
      [Tooltip("The speed at which the object follows the mouse")]
      [SerializeField] private float dragSpeed;
      [Tooltip("<b>On</b>: Mirror rotates Up and Down \n" + "<b>Off</b>: Mirror rotates Left and Right")]
      [SerializeField] private bool zRotation;

      private bool _isHeld;
      private Camera _cam;
      private Rigidbody _rigidbody;
      private float _initialYRotation;
      private float _initialZRotation;
      private Vector3 _initialCamForward;

      private void Start()
      { 
          TooltipMessage = "Hold E to Rotate";
         _cam = Camera.main;
         _rigidbody = GetComponent<Rigidbody>();
         _rigidbody.isKinematic = false;
         _rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
      }

      public override void OnInteract()
      {
         base.OnInteract();
         _rigidbody.constraints = zRotation switch
         {
            true => RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX |
                    RigidbodyConstraints.FreezePositionY,
            false => RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX |
                     RigidbodyConstraints.FreezePositionZ
         };
         _isHeld = true;
         _initialYRotation = transform.eulerAngles.y;
         _initialZRotation = transform.eulerAngles.z;
         _initialCamForward = _cam.transform.forward;
      }

      private void Update()
      {
         if (!_isHeld) return;
         RotateMirror();
      }

      private void RotateMirror()
      {
         _rigidbody.isKinematic = false;
         switch (zRotation)
         {
            case true:
            {
               var zForward = _cam.transform.forward;
               zForward.z = 0;
               _initialCamForward.z = 0;

               zForward.Normalize();
               _initialCamForward.Normalize();

               var zAngle = -Vector3.SignedAngle(_initialCamForward, zForward, Vector3.forward);
               var targetZRotation = _initialZRotation + zAngle;

               var zTargetRotation = Quaternion.Euler(0, _initialYRotation, targetZRotation);

               var zTime = Time.deltaTime * dragSpeed;
               var zRotate = Quaternion.RotateTowards(transform.rotation, zTargetRotation, zTime);
               _rigidbody.MoveRotation(zRotate);
               break;
            }
            case false:
            {
               var currentCamForward = _cam.transform.forward;
               currentCamForward.y = 0;
               _initialCamForward.y = 0;

               currentCamForward.Normalize();
               _initialCamForward.Normalize();

               var angle = -Vector3.SignedAngle(_initialCamForward, currentCamForward, Vector3.up);
               var targetYRotation = _initialYRotation + angle;

               var targetRotation = Quaternion.Euler(0, targetYRotation, _initialZRotation);

               var time = Time.deltaTime * dragSpeed;
               var rotate = Quaternion.RotateTowards(transform.rotation, targetRotation, time);
               _rigidbody.MoveRotation(rotate);
               break;
            }
         }
      }

      public void Release()
      {
         _isHeld = false;
         _rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
         _rigidbody.isKinematic = true;
      }
   }
}