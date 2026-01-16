using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Puzzle.Desert_Reflection_Room
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public class PuzzleMirror : InteractableBase
    {
        [Header("Mirror Settings")]
        [Tooltip("The value of the mirror rotation")]
        [SerializeField] private float rotationValue;
        [Tooltip("Each mirror can have 3 Positions: 0, 1 and 2.\nPut in the value here, in which the mirror has it's correct position")]
        [SerializeField] private int correctRotationIndex;
        private int _rotationIndex;

        [Tooltip("The rotation axis of the mirror")]
        [SerializeField] private RotationAxis rotationAxis;
        private Quaternion _startRotation;

        private void Awake()
        {
            _startRotation = transform.rotation;
            RandomStarter();
            TooltipMessage = "Press E to Interact";
        }

        private void Start()
        {
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    transform.Rotate(Vector3.right, rotationValue * _rotationIndex);
                    break;
                case RotationAxis.Y:
                    transform.Rotate(Vector3.up, rotationValue * _rotationIndex);
                    break;
                case RotationAxis.Z:
                    transform.Rotate(Vector3.forward, rotationValue * _rotationIndex);
                    break;
            }
        }

        public override void OnInteract()
        {
            base.OnInteract();
            RotateMirror();
        }

        private void RotateMirror()
        {
            if (_rotationIndex == 2)
            {
                transform.rotation = _startRotation;
                _rotationIndex = 0;
                return;
            }

            switch (rotationAxis)
            {
                case RotationAxis.X:
                    transform.Rotate(Vector3.right, rotationValue);
                    break;
                case RotationAxis.Y:
                    transform.Rotate(Vector3.up, rotationValue);
                    break;
                case RotationAxis.Z:
                    transform.Rotate(Vector3.forward, rotationValue);
                    break;
            }
            _rotationIndex++;
        }

        private void RandomStarter()
        {
            _rotationIndex = Random.Range(0, 3);
            if (_rotationIndex == correctRotationIndex)
            {
                RandomStarter();
            }
        }
    }
}