using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Enter name of the Scene you want to Load")]
    [SerializeField] private string gameScene;

    void Start()
    {
        AudioManager.instance.PlayMainMenu();
    }


    public void StartGame()
    {
        if (gameScene == String.Empty)
        {
            Debug.Log("No Scene Found");
            return;
        }

        AudioManager.instance.StopMainMenu();
        SceneManager.LoadScene(gameScene);
    }

    public void QuitGame()
    {
        AudioManager.instance.StopMainMenu();
        Application.Quit();
    }
}