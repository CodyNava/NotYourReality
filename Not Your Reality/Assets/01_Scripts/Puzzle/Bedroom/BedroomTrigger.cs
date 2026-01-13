using System.Collections;
using Player.PlayerMovement.Movement;
using UnityEngine;

public class BedroomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] propSets;
    [SerializeField] private Transform targetLocation;

    private int _counter = 0;
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_counter >= propSets.Length) return;

        propSets[_counter].SetActive(false);
        _counter++;
        propSets[_counter].SetActive(true);

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