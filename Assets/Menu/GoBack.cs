using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBack : MonoBehaviour
{

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ReturnToMultiplayerMenu(){
        SceneManager.LoadScene("MultiplayerMenuScene");
    }

    public void QuitGame()
    {
        //it does not work in unity so i put a debug msg
        Debug.Log("Quiting...");
        Application.Quit();
    }
}
