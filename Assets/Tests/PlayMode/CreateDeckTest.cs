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
        int MAX_DECK_NUMBER = 30;
        GameObject.Find("CollectionButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("NewDeckButton").GetComponent<Button>().onClick.Invoke();
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
        // overwrite deck, so first run of this test can fail
        GameObject.Find("SaveAnywayButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("CloseButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        GameObject.Find("ReturnButton").GetComponent<Button>().onClick.Invoke();
        yield return null;

        GameObject.Find("DeckCollectionButton").GetComponent<Button>().onClick.Invoke();
        yield return null;
        string last_deck_name = "not found";
        for (int i = 0; i < MAX_DECK_NUMBER; i++){
            GameObject.Find("ArrowRight").GetComponent<Button>().onClick.Invoke();
            yield return null;
            var deckName = GameObject.Find("DeckName").GetComponentInChildren<TMP_Text>();
            last_deck_name = deckName.text;
            if (deckName.text == "test deck") break;
        }
        Assert.AreEqual("test deck", last_deck_name);
    }
}
