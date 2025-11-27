using Interactions.Interaction_System.Interaction_Base_Class;
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
            _isPaused = false;
            InputManager.Input.Player.Enable();
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);   
        }

        private void PauseGame()
        {
            _isPaused = true;
            InputManager.Input.Player.Disable();
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(menuScene);
        }
    }
}