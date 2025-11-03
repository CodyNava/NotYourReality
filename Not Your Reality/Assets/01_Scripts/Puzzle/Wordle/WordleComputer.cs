using Interactions.Interaction_System.Interaction_Base_Class;
using Player.PlayerMovement.Movement;
using UnityEngine;

namespace Puzzle.Wordle
{
   public class WordleComputer : InteractableBase
   {
      private bool _isActive = false;
      [SerializeField] private PlayerController player;
      [SerializeField] private GameObject computerCamera;
      [SerializeField] private Canvas crosshairCanvas;

      public void Start()
      {
         var crosshairCanvasGameObject = GameObject.FindGameObjectWithTag("Crosshair");
         var playerGameObject = GameObject.FindGameObjectWithTag("Player");
         player = playerGameObject.GetComponent<PlayerController>();
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
         player.MoveActive = false;
         player.CameraActive = false;
      }

      public void ExitTerminal()
      {
         _isActive = false;
         Cursor.lockState = CursorLockMode.Locked;
         Cursor.visible = false;
         computerCamera.SetActive(false);
         crosshairCanvas.enabled = true;
         player.MoveActive = true;
         player.CameraActive = true;
      }
   }
}