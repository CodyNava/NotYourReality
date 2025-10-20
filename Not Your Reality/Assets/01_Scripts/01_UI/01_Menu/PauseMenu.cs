using _01_Scripts._03_Player.PlayerMovement.Playermovement_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private string menuScene;
    [SerializeField] private FirstPersonController player;
    private bool _isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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