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
        [SerializeField] private PlayerController player;
        private bool _isPaused;

        private void Update()
        {
            crosshairCanvas.SetActive(!_isPaused);
            if (InputManager.Input.Player.PauseGame.WasPressedThisFrame())
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
            _isPaused = false;
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);   
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            player.enabled = true;
        }

        private void PauseGame()
        {
            _isPaused = true;
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            player.enabled = false;
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(menuScene);
        }
    }
}