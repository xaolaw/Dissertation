using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEditor;

public class GamePlayTest
{
    [OneTimeSetUp]
    public void LoadScene(){
        SceneManager.LoadScene("MainMenuScene");
    }
    [UnityTest]
    public IEnumerator GamePlayTestWithEnumeratorPasses()
    {
        GameObject.Find("Play Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("ShowHistory").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("ReturnButton").GetComponent<Button>().onClick.Invoke();
        yield return null;

        var card = GameObject.Find("Card(Clone)").GetComponent<Card>();

        var arena = GameObject.Find("Arena").GetComponent<Arena>();
        var tileList = arena.GetTileList();
        tileList[19].Select();
        card.HandleSpawning();
        // var unitSpawn = GameObject.Find("Unit Spawner").GetComponent<UnitSpawn>();
        // unitSpawn.Spawn(tileList[0], )

    }
}
