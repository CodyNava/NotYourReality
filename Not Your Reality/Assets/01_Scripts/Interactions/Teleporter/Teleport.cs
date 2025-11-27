using System.Collections;
using Player.PlayerMovement.Movement;
using UnityEngine;

namespace Interactions.Teleporter
{
    public class Teleport : MonoBehaviour
    {
        private static readonly int Mask = Shader.PropertyToID("_Mask"); 
        [SerializeField] private Transform targetLocation;
        [SerializeField] private Camera portalCamera;
        [SerializeField] private Renderer portalScreen;
        private GameObject _player;

        private void Start()
        {
            if (!portalCamera || !portalScreen) return;
            _player = GameObject.FindGameObjectWithTag("Player");
            var portalTexture = new RenderTexture(1024, 2048, 24);
            portalCamera.targetTexture = portalTexture;
            portalScreen.material.SetTexture(Mask, portalTexture);
        }
    

        private void OnTriggerEnter(Collider other)
        {
            StartCoroutine(StartTeleport(transform, targetLocation, other));
        }

        private IEnumerator StartTeleport(Transform entryPortal, Transform exitPortal, Collider other)
        {
            var localPos = entryPortal.InverseTransformPoint(other.transform.position);
        
            switch (other.tag)
            {
                case "Player":
                    var controller = _player.GetComponent<CharacterController>();
                    var script = _player.GetComponent<PlayerController>();
        
                    script.MoveActive = false;
                    controller.enabled = false;

                    other.transform.position = exitPortal.TransformPoint(localPos);
                    var relativeRot = Mathf.Atan2(localPos.x, localPos.z) * Mathf.Rad2Deg;
                    var exitRot = Quaternion.AngleAxis(relativeRot, exitPortal.up) * exitPortal.rotation;
                    other.transform.rotation = exitRot;
        
                    yield return new WaitForSeconds(0.01f);
                    controller.enabled = true;
                    script.MoveActive = true;
                    break;
                case "Weight":
                case "Misc Interactable":
                    var localRotate = Quaternion.Inverse(entryPortal.rotation) * other.transform.rotation;
                    other.transform.position = exitPortal.TransformPoint(localPos);
                    other.transform.rotation = exitPortal.rotation * localRotate;
                    break;
            }
        }
    }
}