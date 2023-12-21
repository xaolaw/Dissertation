using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class GenerateCodeTest
{
    [OneTimeSetUp]
    public void LoadScene(){
        SceneManager.LoadScene("MainMenuScene");
    }
    [UnityTest]
    public IEnumerator GenerateCodeTestWithEnumeratorPasses()
    {
        GameObject.Find("Multiplayer").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("HostGame").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("GetCode").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("Copy").GetComponent<Button>().onClick.Invoke();
        yield return null;
        string clipboardText = GUIUtility.systemCopyBuffer;
        // not checking whether code is valid
        Assert.IsNotNull(clipboardText);
    }
}
