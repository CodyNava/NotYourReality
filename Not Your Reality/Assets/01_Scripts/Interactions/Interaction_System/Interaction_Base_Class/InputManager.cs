using UnityEngine;

namespace Interactions.Interaction_System.Interaction_Base_Class
{
   public class InputManager : MonoBehaviour
   {
      public static MainInput Input { get; private set; }

      private void Awake()
      {
         if (Input == null)
         {
            Input ??= new MainInput();
            DontDestroyOnLoad(gameObject);
         }
         else
         {
            Destroy(gameObject);
            return;
         }
      }
      private void OnEnable() { Input?.Enable(); }
      private void OnDisable() { Input?.Disable(); }

      private void OnDestroy()
      {
         Input?.Dispose();
         Input = null;
      }
   }
}