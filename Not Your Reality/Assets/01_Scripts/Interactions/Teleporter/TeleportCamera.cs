using UnityEngine;

namespace Interactions.Teleporter
{
    public class PortalView : MonoBehaviour
    {
        [Space]
        [SerializeField] private Transform entryPortal;
        [SerializeField] private Transform exitPortal;
        [SerializeField] private Camera portalCamera;
        private Camera _playerCamera;

        private void Start()
        {
            _playerCamera = Camera.main;
            portalCamera.fieldOfView = 80f;
        }
        private void LateUpdate()
        {
            if (!entryPortal || !exitPortal || !portalCamera || !_playerCamera) return;
        
            var relativePos = entryPortal.InverseTransformPoint(_playerCamera.transform.position);
            var relativeRot = Mathf.Atan2(relativePos.x, relativePos.z) * Mathf.Rad2Deg;

            var exitRotation = Quaternion.AngleAxis(relativeRot, exitPortal.up) * exitPortal.rotation;
            
            portalCamera.transform.rotation = exitRotation;
        }
    }
}