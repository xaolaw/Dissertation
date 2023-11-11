using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }
    public void ShowCollection()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2, LoadSceneMode.Single);
    }
    public void ReturnToMainMenu()
    {
        Debug.Log("WTF");
        SceneManager.LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        //it does not work in unity so i put a debug msg
        Debug.Log("Quiting...");
        Application.Quit();
    }
}
