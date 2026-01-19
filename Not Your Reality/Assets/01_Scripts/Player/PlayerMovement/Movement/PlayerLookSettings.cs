using UnityEngine;


namespace Player.PlayerMovement.Movement
{
   [CreateAssetMenu(fileName = "LookSense", menuName = "Player/LookSense")]
   public class PlayerLookSettings : ScriptableObject
   {
      
      [SerializeField][Range(0f, 0.5f)] public float mouseSmoothTime = 0.03f;
      [SerializeField] public float mouseSensitivity = 2f;
      
   }
}
