using UnityEngine;

namespace Interactions.Interaction_System.Interactions.Door_Rework
{
    public class VoiceColliderTrigger : MonoBehaviour
    {
        public RoomVoiceManager manager;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            manager.OnVoiceTriggered(gameObject);
        }
    }
}
