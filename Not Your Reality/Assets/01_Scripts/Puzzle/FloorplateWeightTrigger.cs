using System.Collections.Generic;
using Interactions.Interaction_System.Interactions;
using UnityEngine;

namespace Puzzle
{
   public class FloorplateWeightTrigger : MonoBehaviour
   {
      [Space]
      [Tooltip("The weight that needs to be reached for the plate to trigger")]
      [SerializeField] private float goalWeight;

      [Tooltip("The distance the platform will move once it is activated")]
      [SerializeField] private float platformMoveDistance;

      [Tooltip("The speed the platform will move at once it is activated")]
      [SerializeField] private float platformMoveSpeed;
      [Tooltip("The target effected by the plate (e.g. a Door)")]
      [SerializeField] private GameObject winTarget;
      [SerializeField] private bool toggleDoor;
      [SerializeField] private bool toggleObject;

      private List<MoveObject> _objects;
      private Rigidbody _rb;
      private bool _hasWon;
      private Collider _collider;

      private Vector3 _startPos;
      private Vector3 _targetPos;

      private void Awake()
      {
         _objects = new List<MoveObject>();
         _objects.Clear();
         _rb = GetComponent<Rigidbody>();
         _startPos = transform.position;
         _targetPos = _startPos;
         if (toggleDoor) _collider = winTarget.GetComponent<Collider>();
         _collider.enabled = false;
      }

      private void FixedUpdate()
      {
         var moveTo = Vector3.Lerp(transform.position, _targetPos, Time.fixedDeltaTime * platformMoveSpeed);
         _rb.MovePosition(moveTo);
      }

      private void CheckWin()
      {
         var currentWeight = 0f;
         foreach (var moveObject in _objects) { currentWeight += moveObject.Weight; }

         if (currentWeight >= goalWeight && !_hasWon)
         {
            _targetPos = _startPos + Vector3.down * platformMoveDistance;
            _hasWon = true;
            Debug.Log("You did it");
            if (toggleObject) winTarget.SetActive(false);
            if (toggleDoor) _collider.enabled = true;
         }
         else if (currentWeight < goalWeight && _hasWon)
         {
            _hasWon = false;
            _targetPos = _startPos;
            Debug.Log("Not enough");
            if (toggleObject) winTarget.SetActive(true);
            if (toggleDoor) _collider.enabled = false;
         }
      }

      private void OnTriggerEnter(Collider other)
      {
         if (!other.CompareTag("Weight")) return;
         var moveObject = other.GetComponent<MoveObject>();
         _objects.Add(moveObject);
         CheckWin();
      }

      private void OnTriggerExit(Collider other)
      {
         if (!other.CompareTag("Weight")) return;
         var moveObject = other.GetComponent<MoveObject>();
         _objects.Remove(moveObject);
         CheckWin();
      }
   }
}