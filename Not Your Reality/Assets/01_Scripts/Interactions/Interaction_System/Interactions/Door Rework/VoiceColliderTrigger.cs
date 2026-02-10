using Interactions.Interaction_System.Interactions.Door_Rework;
using UnityEngine;

public class VoiceColliderTrigger : MonoBehaviour
{
    public RoomVoiceManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        manager.OnVoiceTriggered(gameObject);
    }
}
