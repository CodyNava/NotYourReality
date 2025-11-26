using Interactions.Interaction_System.Interaction_Base_Class;
using UnityEngine;

namespace Puzzle.Wordle
{
   public class WordleComputer : InteractableBase
   {
      private bool _isActive;
      [SerializeField] private GameObject computerCamera;
      [SerializeField] private Canvas crosshairCanvas;

      public void Start()
      {
         var crosshairCanvasGameObject = GameObject.FindGameObjectWithTag("Crosshair");

         crosshairCanvas = crosshairCanvasGameObject.GetComponent<Canvas>();
      }

      private void Update()
      {
         if (_isActive)
         {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
         }
      }

      public override void OnInteract()
      {
         base.OnInteract();
         if (!_isActive) { EnterTerminal(); }
         else { ExitTerminal(); }
      }

      private void EnterTerminal()
      {
         _isActive = true;
         Cursor.lockState = CursorLockMode.None;
         Cursor.visible = true;
         computerCamera.SetActive(true);
         crosshairCanvas.enabled = false;
         InputManager.Input.Player.Disable();
      }

      public void ExitTerminal()
      {
         _isActive = false;
         Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false;
         computerCamera.SetActive(false);
         crosshairCanvas.enabled = true;
         InputManager.Input.Player.Enable();
      }
   }
}