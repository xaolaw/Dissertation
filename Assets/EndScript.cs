using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScript : MonoBehaviour
{
    public GameObject YouWonText, OpponentWonText;
    //public Button mainMenuButton, leaveGameButton;

    public void ShowEndCanvas(bool youwin) {
        if (youwin) YouWonText.SetActive(true);
        else OpponentWonText.SetActive(true);

        GetComponent<Canvas>().enabled = true;
    }

    void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

    public void leaveGame()
    {
        Application.Quit();
    }

    public void goToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
