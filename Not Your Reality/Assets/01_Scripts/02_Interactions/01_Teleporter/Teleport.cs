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
        if (portalCamera && portalScreen)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            var portalTexture = new RenderTexture(Screen.width, Screen.height, 24);
            portalCamera.targetTexture = portalTexture;
            portalScreen.material.mainTexture = portalTexture;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StartTeleport());
        }
    }

    private IEnumerator StartTeleport()
    {
        var controller = player.GetComponent<CharacterController>();
        var script = player.GetComponent<FirstPersonController>();
        
        script.MoveActive = false;
        controller.enabled = false;
        player.transform.position = targetLocation.position;
        player.transform.rotation = targetLocation.rotation;
        yield return new WaitForSeconds(0.01f);
        controller.enabled = true;
        script.MoveActive = true;
    }
}