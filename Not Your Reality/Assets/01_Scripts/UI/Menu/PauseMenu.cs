using System;
using Interactions.Interaction_System.Interaction_Base_Class;
using Player.PlayerMovement.Movement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject settingsMenu;
        [SerializeField] private GameObject crosshairCanvas;
        [SerializeField] private string menuScene;
        private bool _isPaused;
        private PlayerController _playerController;
        private GameObject _player;

        private void Start()
        {
            _playerController = FindFirstObjectByType<PlayerController>();
            _player = _playerController.gameObject;
            ResumeGame();
        }

        private void Update()
        {
            crosshairCanvas.SetActive(!_isPaused);
            if (InputManager.Input.UI.Pause.WasPressedThisFrame())
            {
                if (_isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _isPaused = false;
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false); 
            _playerController.CameraActive = true;
            InputManager.Input.Player.Enable();
        }

        private void PauseGame()
        {
            InputManager.Input.Player.Disable();
            _isPaused = true;
            Time.timeScale = 0f;
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);
            _playerController.CameraActive = false;
        }

        public void BackToMenu()
        {
            Destroy(_player);
            SceneManager.LoadScene(menuScene);
        }
    }
}