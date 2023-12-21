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
    public IEnumerator GoToDeckEditMenuWithEnumeratorPasses()
    {
        GameObject.Find("CollectionButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("DeckCollectionButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Scene scene = SceneManager.GetActiveScene();
        Assert.AreEqual("CreateDeckScene", scene.name);
    }
}
