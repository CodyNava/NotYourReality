using System;
using UnityEngine;

namespace PNG_Room
{
    //[ExecuteInEditMode]
    public class PngLookAtPlayer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerCam;

        [Header("Occlusion (Walls)")]
        [SerializeField] private LayerMask wallMask;

        [SerializeField] private float sphereCastRadius = 0.15f;
        [SerializeField] private float castStartOffset = 0.02f;

        [Header("Rotation")]
        [SerializeField] private bool yAxisOnly = true;

        [SerializeField] private float turnSpeedDegreesPerSecond;

        [Header("Collision Safety")]
        [SerializeField] private bool rejectRotationIfColliding = true;

        [SerializeField] private float penetrationExtraPush = 0.001f;

        [Header("Look up Offset")]
        [SerializeField] private float lookUpOffset = 0.3f;

        [Header("Pitch Clamp")]
        [SerializeField] private float maxLookUpAngle = 25f;

        [SerializeField] private float maxLookDownAngle = 10f;

        private Quaternion _lastSafeRotation;
        private BoxCollider _box;

        private void OnEnable()
        {
            _lastSafeRotation = transform.rotation;
            _box = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (!playerCam) return;

            Vector3 targetPos = playerCam.position + (playerCam.up * lookUpOffset);
            Vector3 toCam = targetPos - transform.position;

            if (yAxisOnly)
            {
                toCam.y = 0f;
                if (toCam.sqrMagnitude < 0.0001f) return;
            }

            Quaternion rawRot = Quaternion.LookRotation(toCam.normalized, Vector3.up);

            Vector3 euler = rawRot.eulerAngles;

            if (euler.x > 180f)
                euler.x -= 360f;

            euler.x = Mathf.Clamp(euler.x, -maxLookDownAngle, maxLookUpAngle);

            Quaternion desiredRot = Quaternion.Euler(euler);

            if (!HasLineOfSightToCamera())
            {
                ApplyRotation(_lastSafeRotation);
                return;
            }

            Quaternion nextRot = desiredRot;

            if (turnSpeedDegreesPerSecond > 0f)
            {
                nextRot = Quaternion.RotateTowards(transform.rotation, desiredRot,
                    turnSpeedDegreesPerSecond * Time.deltaTime);
            }

            if (rejectRotationIfColliding && _box != null)
            {
                Quaternion originalRot = transform.rotation;

                transform.rotation = nextRot;

                if (isCollidingWithWalls(out Vector3 pushDir, out float pushDist))
                {
                    transform.rotation = originalRot;
                    ApplyRotation(_lastSafeRotation);
                    return;
                }

                _lastSafeRotation = transform.rotation;
                return;
            }

            ApplyRotation(nextRot);
            _lastSafeRotation = transform.rotation;
        }

        private void ApplyRotation(Quaternion rot)
        {
            transform.rotation = rot;
        }

        private bool HasLineOfSightToCamera()
        {
            Vector3 origin = transform.position;

            Vector3 dir = (playerCam.position - origin);
            float dist = dir.magnitude;
            if (dist <= 0.0001f) return true;

            dir /= dist;
            origin += dir * castStartOffset;

            return !Physics.SphereCast(
                origin,
                sphereCastRadius,
                dir,
                out RaycastHit hit,
                dist - castStartOffset,
                wallMask,
                QueryTriggerInteraction.Ignore);
        }

        private bool isCollidingWithWalls(out Vector3 pushDirection, out float pushDistance)
        {
            pushDirection = Vector3.zero;
            pushDistance = 0f;

            Collider[] hits = Physics.OverlapBox(
                _box.bounds.center,
                _box.bounds.extents,
                transform.rotation,
                wallMask,
                QueryTriggerInteraction.Ignore);

            if (hits == null || hits.Length == 0) return false;

            foreach (var other in hits)
            {
                if (!other) continue;

                if (Physics.ComputePenetration(
                        _box, _box.transform.position, _box.transform.rotation,
                        other, other.transform.position, other.transform.rotation,
                        out Vector3 dir, out float dist))
                {
                    pushDirection = dir;
                    pushDistance = dist + penetrationExtraPush;
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            if (!playerCam) return;

            Gizmos.DrawSphere(transform.position, sphereCastRadius);
        }

        /*
        [SerializeField] private Transform playerCam;

        void Update()
        {
            LookAtPlayer();
        }

        void LookAtPlayer()
        {
            //transform.LookAt(playerCam);
            transform.LookAt(playerCam, Vector3.up);
        }*/
    }
}