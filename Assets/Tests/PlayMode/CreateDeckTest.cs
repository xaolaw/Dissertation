using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using TMPro;

public class CreateDeckTest
{
    [OneTimeSetUp]
    public void LoadScene(){
        SceneManager.LoadScene("MainMenuScene");
    }
    [UnityTest]
    public IEnumerator CreateDeckTestWithEnumeratorPasses()
    {
        GameObject.Find("CollectionButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        var inputField = GameObject.Find("InputDeckName").GetComponent<TMP_InputField>();
        for (int i=1; i<9; i++){
            string cardPlace = "CardPlace" + i;
            var cardplace = GameObject.Find(cardPlace);
            var buttons = cardplace.GetComponentsInChildren<Button>();
                foreach (var button in buttons){
                if (button.name == "Plus"){
                    button.onClick.Invoke();
                    }
                }
        }
        for (int i=1; i<3; i++){
            string cardPlace = "CardPlace" + i;
            var cardplace = GameObject.Find(cardPlace);
            var buttons = cardplace.GetComponentsInChildren<Button>();
                foreach (var button in buttons){
                if (button.name == "Plus"){
                    button.onClick.Invoke();
                    }
                }
        }
        inputField.text = "test deck";
        GameObject.Find("SaveDeckButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("ReturnButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
    }
}
