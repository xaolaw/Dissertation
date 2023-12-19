using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class MainMenuTest
{
    [OneTimeSetUp]
    public void LoadScene(){
        SceneManager.LoadScene("MainMenuScene");
    }
    [UnityTest]
    public IEnumerator CreateDeckTestWithEnumeratorPasses()
    {
        GameObject.Find("Collection").GetComponent<Button>().onClick.Invoke();
        yield return null;
    }
}
