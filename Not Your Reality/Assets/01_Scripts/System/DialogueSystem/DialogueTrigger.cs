using System.DialogueSystem.SO;
using UnityEngine;

namespace System.DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        
        [SerializeField] private DialogueSequence sequence;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private string playerTag = "Player";

        private void Trigger()
        {
            if (!dialogueManager)
            {
                Debug.LogWarning("Dialogue Trigger not found");
                return;
            }
            dialogueManager.PlaySequence(sequence);
            
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.tag);
            
            if (other.CompareTag(playerTag))
            {
                Trigger();
            }
        }
    }
}
