using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Classes;
using TMPro;
using System.Linq;

public class DeckEditMenu : MonoBehaviour
{
    public TMP_Text DeckName;
    public GameObject ArrowLeft;
    public GameObject ArrowRight;
    public GameObject DeckEditBox;
    public GameObject NoDeckMsg;
    public CreateDeck DeckCollection;

    private DeckCollection CardDecks = new();
    private readonly string JSON_PATH = "CardDataBase/Decks";
    private readonly string SELECTED_DECK_PATH = "CardDataBase/selectedDeckIndex";
    private int displayedDeck = 0;
    private int selectedDeckIndex = 0;

    void Start()
    {
        ReadJson(JSON_PATH);
        ShowDeckToEdit();
    }
    public void ResetView()
    {
        Debug.Log("Reset view");
        displayedDeck = 0;
        ReadJson(JSON_PATH);
        ShowDeckToEdit();
    }

    private void ReadJson(string path)
    {
        TextAsset json_file = Resources.Load<TextAsset>(path);
        var jsonDB = json_file.text;
        CardDecks = JsonConvert.DeserializeObject<DeckCollection>(jsonDB);
    }

    private void ShowDeckToEdit()
    {
        switch (CardDecks.Decks.Count)
        {
            case 0:
                DeckEditBox.SetActive(false);
                NoDeckMsg.SetActive(true);
                break;

            case 1:
                ArrowLeft.SetActive(false);
                ArrowRight.SetActive(false);
                break;

            default:
                DeckEditBox.SetActive(true);
                NoDeckMsg.SetActive(false);
                ArrowLeft.SetActive(true);
                ArrowRight.SetActive(true);
                break;
        }

        if (CardDecks.Decks.Count != 0)
            DeckName.text = CardDecks.Decks[displayedDeck].Name;
    }

    public void ShowRight()
    {
        displayedDeck++;
        
        if(displayedDeck > CardDecks.Decks.Count -1)
        {
            displayedDeck = 0;
        }
        ShowDeckToEdit();
    }
    public void ShowLeft()
    {
        displayedDeck--;
        if (displayedDeck < 0)
        {
            displayedDeck = CardDecks.Decks.Count - 1;
        }
        ShowDeckToEdit();
    }

    public void EditDeck()
    {
        DeckCollection.InitalizeDeck(CardDecks.Decks[displayedDeck].CardList.ToList(),CardDecks.Decks[displayedDeck].Name);
    }

    public void SelectDeck(){
        selectedDeckIndex = displayedDeck;
        using (var stream = new FileStream(SELECTED_DECK_PATH, FileMode.Truncate))
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(selectedDeckIndex);
            }
        }

    }
}