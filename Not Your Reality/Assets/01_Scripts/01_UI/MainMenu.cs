using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Enter name of the Scene you want to Load")]
    [SerializeField] private string gameScene;
    
    public void StartGame()
    {
        if (gameScene == String.Empty)
        {
            Debug.Log("No Scene Found");
            return;
        }
        SceneManager.LoadScene(gameScene);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}