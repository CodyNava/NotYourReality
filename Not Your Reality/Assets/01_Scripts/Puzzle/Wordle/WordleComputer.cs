using Interactions.Interaction_System.Interaction_Base_Class;
using Player.PlayerMovement.Movement;
using UnityEngine;

namespace Puzzle.Wordle
{
   public class WordleComputer : InteractableBase
   {
      private bool _isActive;
      private PlayerController _playerController;
      [SerializeField] private GameObject computerCamera;
      [SerializeField] private Canvas crosshairCanvas;
      [SerializeField] private CanvasGroup wordleCanvas;

      public void Start()
      {
          _playerController = FindFirstObjectByType<PlayerController>();
         var crosshairCanvasGameObject = GameObject.FindGameObjectWithTag("Crosshair");

         crosshairCanvas = crosshairCanvasGameObject.GetComponent<Canvas>();
      }

      public override void OnInteract()
      {
         base.OnInteract();
         if (!_isActive)
         {
             EnterTerminal();
         }
         else
         {
             ExitTerminal();
         }
      }

      private void EnterTerminal()
      {
          wordleCanvas.blocksRaycasts = true;
         _isActive = true;
         _playerController.CameraActive = false;
         computerCamera.SetActive(true);
         crosshairCanvas.enabled = false;
         InputManager.Input.Player.Disable();
         InputManager.Input.UI.Disable();
      }

      public void ExitTerminal()
      {
          wordleCanvas.blocksRaycasts = false;
         _isActive = false;
         _playerController.CameraActive = true;
         computerCamera.SetActive(false);
         crosshairCanvas.enabled = true;
         InputManager.Input.Player.Enable();
         InputManager.Input.UI.Enable();
      }
   }
}