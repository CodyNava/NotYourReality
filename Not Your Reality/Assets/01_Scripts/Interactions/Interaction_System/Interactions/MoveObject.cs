using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Interactions.Interaction_System.Interactions
{
   public class MoveObject : InteractableBase
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
          TooltipMessage = "Hold E to Move Object";
         _cam = Camera.main;
         _rb = GetComponent<Rigidbody>();
      }

      public override void OnInteract()
      {
         base.OnInteract();
         _isHeld = true;
         _rb.useGravity = false;
      }

     /* private void LateUpdate()
      {
          _cachedTarget = _cam.transform.position + _cam.transform.forward * holdingDistance;
      }

      private void MoveItem()
      {
          var velocity = Vector3.zero;
          var smoothLerp = Vector3.SmoothDamp(_rb.position, _cachedTarget, ref velocity, 0.05f);
          _rb.MovePosition(smoothLerp);
      }*/
      
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
         if (!_rb) return;
         _rb.useGravity = true;
      }
   }
}