using System.Collections;
using _01_Scripts._03_Player.PlayerMovement.Playermovement_Scripts;
using UnityEngine;
public class Teleport : MonoBehaviour
{
    //Property for the Texture, the camera is Rendering 
    private static readonly int Mask = Shader.PropertyToID("_Mask"); 
    [SerializeField] private Transform targetLocation;
    [SerializeField] private Camera portalCamera;
    [SerializeField] private Renderer portalScreen;
    [SerializeField] private GameObject player;

    private void Start()
    {
        if (!portalCamera || !portalScreen) return;
        player = GameObject.FindGameObjectWithTag("Player");
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
        var localRotate = Quaternion.Inverse(entryPortal.rotation) * other.transform.rotation;
        
        switch (other.tag)
        {
            case "Player":
                var controller = player.GetComponent<CharacterController>();
                var script = player.GetComponent<FirstPersonController>();
        
                script.MoveActive = false;
                controller.enabled = false;

                other.transform.position = exitPortal.TransformPoint(localPos);
                other.transform.rotation = exitPortal.rotation * localRotate;
        
                yield return new WaitForSeconds(0.01f);
                controller.enabled = true;
                script.MoveActive = true;
                break;
            case "Weight":
            case "Misc Interactable":
                other.transform.position = exitPortal.TransformPoint(localPos);
                other.transform.rotation = exitPortal.rotation * localRotate;
                break;
        }
    }
}