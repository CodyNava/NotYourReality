using System.Linq;
using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Puzzle.Balloon_Matryoshka
{
    public class ScissorsForBalloon : InteractableBase
    {
        private Camera _cam;
        private Transform _anchorTransform;
        private Quaternion _anchorRotation;
        private Rigidbody _rb;
        private bool _isSticking;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _cam = Camera.main;
        }

        public override void OnInteract()
        {
            base.OnInteract();
            _isSticking = true;
        }

        private void FixedUpdate()
        {
            if (!_isSticking) return;
            Stuck();
        }

        private void Stuck()
        {
            var currentPosition = transform.position;
            var targetPosition = _cam.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.CompareTag("Inspection Anchor"))!.position;
            var time = Time.deltaTime;
            var lerp = Vector3.Lerp(currentPosition, targetPosition, time);
            _rb.MovePosition(lerp);
        }
    }
}
