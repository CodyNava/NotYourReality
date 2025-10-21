using System.Collections;
using _01_Scripts._03_Player.PlayerMovement.Playermovement_Scripts;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform targetLocation;
    [SerializeField] private Camera portalCamera;
    [SerializeField] private Renderer portalScreen;
    [SerializeField] private GameObject player;

    private void Start()
    {
        if (!portalCamera || !portalScreen) return;
        player = GameObject.FindGameObjectWithTag("Player");
        var portalTexture = new RenderTexture(Screen.width, Screen.height, 24);
        portalCamera.targetTexture = portalTexture;
        portalScreen.material.mainTexture = portalTexture;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StartTeleport(transform, targetLocation));
        }
    }

    private IEnumerator StartTeleport(Transform entryPortal, Transform exitPortal)
    {
        var controller = player.GetComponent<CharacterController>();
        var script = player.GetComponent<FirstPersonController>();
        
        script.MoveActive = false;
        controller.enabled = false;

        var localPos = entryPortal.InverseTransformPoint(player.transform.position);
        var localRotate = Quaternion.Inverse(entryPortal.rotation) * player.transform.rotation;

        player.transform.position = exitPortal.TransformPoint(localPos);
        player.transform.rotation = exitPortal.rotation * localRotate;
        
        yield return new WaitForSeconds(0.01f);
        controller.enabled = true;
        script.MoveActive = true;
    }
}