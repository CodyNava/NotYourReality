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
      [Tooltip("The LayerMask used by the Player")]
      [SerializeField] private int playerLayerMask = 9;

      [SerializeField] private float spring = 150f;
      [SerializeField] private float damping = 25f;
      [SerializeField] private float maxVelocity = 10f;

      public float Weight => weight;

      private Rigidbody _rb;
      private bool _isHeld;
      private Camera _cam;

      private void Awake()
      {
          TooltipMessage = "Press E to Move Object";
         _cam = Camera.main;
         _rb = GetComponent<Rigidbody>();
      }

      public override void OnInteract()
      {
         base.OnInteract();
         _isHeld = true;
         _rb.useGravity = false;
         _rb.interpolation = RigidbodyInterpolation.Interpolate;
         _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
         
         Physics.IgnoreLayerCollision(gameObject.layer, playerLayerMask, true);
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
         var targetPosition = _cam.transform.position + _cam.transform.forward * holdingDistance;
         var targetRotation = Quaternion.LookRotation(_cam.transform.forward, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);
         var displacement = targetPosition - _rb.position;

         var force = displacement * spring;
         
         force -= _rb.linearVelocity * damping;
         _rb.AddForce(force, ForceMode.Acceleration);
         _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, maxVelocity);
         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, weight);
      }

      public void Release()
      {
         _isHeld = false;
         if (!_rb) return;
         _rb.useGravity = true;
         
         Physics.IgnoreLayerCollision(gameObject.layer, playerLayerMask, false);
      }
   }
}