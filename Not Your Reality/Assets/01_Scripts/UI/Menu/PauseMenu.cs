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
            InputManager.Input.Player.Enable();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _isPaused = false;
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false); 
        }

        private void PauseGame()
        {
            InputManager.Input.Player.Disable();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _isPaused = true;
            Time.timeScale = 0f;
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(menuScene);
        }
    }
}