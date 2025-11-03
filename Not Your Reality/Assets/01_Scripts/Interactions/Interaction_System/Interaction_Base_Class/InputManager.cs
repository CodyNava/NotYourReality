using UnityEngine;

namespace Interactions.Interaction_System.Interaction_Base_Class
{
   public class InputManager : MonoBehaviour
   {
      public static MainInput Input;
      private void Awake() { Input ??= new MainInput(); }
      private void OnEnable() { Input.Enable(); }
      private void OnDisable() { Input.Disable(); }

      private void OnDestroy()
      {
         Input.Dispose();
         Input = null;
      }
   }
}