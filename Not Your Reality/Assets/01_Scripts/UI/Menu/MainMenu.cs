using System;
using System.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Enter name of the Scene you want to Load")]
    [SerializeField] private string gameScene;

    void Start()
    {
        AudioManager.Instance.PlayMainMenu();
    }


    public void StartGame()
    {
        if (gameScene == String.Empty)
        {
            return;
        }

        AudioManager.Instance.StopMainMenu();
        SceneManager.LoadScene(gameScene);
        AudioManager.Instance.PlayBasement();
    }

    public void QuitGame()
    {
        AudioManager.Instance.StopMainMenu();
        Application.Quit();
    }
}