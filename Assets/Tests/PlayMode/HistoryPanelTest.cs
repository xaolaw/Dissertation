using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class HistoryPanelTest
{
    [OneTimeSetUp]
    public void LoadScene(){
        SceneManager.LoadScene("MainMenuScene");
    }

    [UnityTest]
    public IEnumerator HistoryPanelTestWithEnumeratorPasses()
    {
        GameObject.Find("Play Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("ShowHistory").GetComponent<Button>().onClick.Invoke();
        yield return null;
        var historyPanel = GameObject.Find("HistoryPanel");
        Assert.IsTrue(historyPanel.activeSelf);
        GameObject.Find("ReturnButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        Assert.IsFalse(historyPanel.activeSelf);
    }
}
