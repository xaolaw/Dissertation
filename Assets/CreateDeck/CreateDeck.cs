using Assets.Classes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
public class CreateDeck : MonoBehaviour
{
    public List<GameObject> cardsPlaceInUiList;
    public GameObject ArrowRight;
    public GameObject ArrowLeft;
    public GameObject ErrorAlert;

    //List of all cards from a game
    private List<CardJson> cardsJson;
    private List<int> cardDeck = new();
    private int page = 0;
    private readonly int cardsPerPage = 10;
    private readonly int deckSize = 10;
    private string deckName;

    // Start is called before the first frame update
    void Start()
    {
        ReadJson("Assets/CardDataBase/cardDB.json");
        DisplayCards();
    }
    //Display cards
    private void DisplayCards()
    {
        for(int i=(0 + page*cardsPerPage); i<(10 + page * cardsPerPage); i++)
        {
            if (i < cardsJson.Count)
            {
                InitalizeCardOnDisplay(cardsPlaceInUiList[i % 10], cardsJson[i].cardImage);
            }
            else
            {
                cardsPlaceInUiList[i % 10].SetActive(false);
            }
        }
    }
    //Display proper card
    private void InitalizeCardOnDisplay(GameObject cardPlaceInUi, string imagePath)
    {
        cardPlaceInUi.SetActive(true);
        Sprite image = Resources.Load<Sprite>(imagePath);

        Image imageComponent = cardPlaceInUi.transform.GetChild(0).GetComponent<Image>();

        if (imageComponent != null)
        {
            if (image == null)
            {
                Debug.LogError("Invalid path" + imagePath);
                image = Resources.Load<Sprite>("2DModels/noTexture");
            }
            imageComponent.sprite = image;
            
        }
        else
        {
            Debug.LogError("Invalid! imageComponent is null");
        }
    }
    ///////////////////////////////////////////
    ///Json collection prepareing functrions///
    ///////////////////////////////////////////
   
    //Read from jsonDb data and sort data
    private void ReadJson(string path)
    {
        using StreamReader reader = new(path);
        var jsonDB = reader.ReadToEnd();
        cardsJson = JsonConvert.DeserializeObject<List<CardJson>>(jsonDB);
        reader.Close();
        SortCardJsonList();

    }

    private void SortCardJsonList()
    {
        cardsJson = cardsJson.
            OrderBy(c => c.cardEnergy).
            ThenBy(c => c.cardName).ToList();
    }

    //////////////////////////////////
    ///Collection showing functions///
    //////////////////////////////////
    
    //Change collection page going right
    public void PageRight()
    {
        page++;
        DisplayCards();
        if ( page >= Mathf.Floor(cardsJson.Count/10))
        {
            ArrowRight.SetActive(false);
        }
        ArrowLeft.SetActive(true);
        UpdateAllCounters();
    }
    //Change collection page going left
    public void PageLeft()
    {
        page--;
        DisplayCards();
        if (page == 0)
        {
            ArrowLeft.SetActive(false);
        }
        ArrowRight.SetActive(true);
        UpdateAllCounters();
    }
    //////////////////////////////////////
    ///Deck building changing functions/// 
    //////////////////////////////////////
    
    //Add a card to deck
    public void AddToDeck(int index)
    {
        index += (page * 10);
        if (CheckIfAbleToAdd(index))
        {
            cardDeck.Add(index);
            UpdateCounter(cardDeck.FindAll(x => x == index).Count, index);
        }
    }

    //Remove card from deck
    public void DeleteFromDeck(int index)
    {
        index += (page * 10);
        if (CheckIfAbleToRemove(index))
        {
            cardDeck.Remove(index);
            UpdateCounter(cardDeck.FindAll(x => x == index).Count, index);
        }
    }
    //Check if we can add a card to deck
    private bool CheckIfAbleToAdd(int index)
    {
        //if limit of card is exceeded return false
        if (cardDeck.FindAll(x => x == index).Count == cardsJson[index].cardInDeckLimitNumber)
            return false;
        else if (cardDeck.Count == deckSize)
            return false;
        return true;
    }
    //Check if we can remove a card to deck
    private bool CheckIfAbleToRemove(int index)
    {
        if (cardDeck.FindAll(x => x == index).Count == 0)
            return false;
        return true;
    }
    ///////////////////////////
    ///UI changing functions/// 
    ///////////////////////////
    

    //Change number of cards in deck that are displayed
    private void UpdateCounter(int value,int index)
    {
        //Getting counter from list of objects
        GameObject counter = cardsPlaceInUiList[index % 10].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
        TMP_Text counterNumber = counter.transform.GetChild(0).GetComponent<TMP_Text>();
        counterNumber.text = value.ToString();
    }
    //Update all numbers when changing page in collection
    private void UpdateAllCounters()
    {
        for (int j=0 + page * 10; j< 10 + page * 10; j++)
        {
            UpdateCounter(cardDeck.FindAll(x => x == j).Count, j);
        }
    }
    //Display Error when creating deck
    private void DisplayErrorAlert(string msg)
    {
        ErrorAlert.SetActive(true);
        ErrorAlert.transform.GetChild(0).GetComponent<TMP_Text>().text = msg;
    }
    //Save deck name
    public void ReadStringInput(string inputName){deckName = inputName;}

    /////////////////
    ///Saving Deck/// 
    /////////////////

    //Save deck to json
    public void SaveDeck()
    {
        if (deckName == null)
        {
            DisplayErrorAlert("Enter deck name");
            Debug.LogError("Enter deck name");
        }
        if (deckSize == cardDeck.Count)
        {
            Deck newDeck = new()
            {
                Name = deckName,
                CardList = cardDeck.ToArray()
            };
            using StreamReader reader = new("Assets/CreateDeck/Decks.json");
            var jsonDeck = reader.ReadToEnd();
            DeckCollection decks = JsonConvert.DeserializeObject<DeckCollection>(jsonDeck);
            reader.Close();

            if (decks.Decks.Find(d => d.Name == deckName) == null)
            {
                decks.Decks.Add(newDeck);
                string updatedJson = JsonConvert.SerializeObject(decks);
                File.WriteAllText("Assets/CreateDeck/Decks.json", updatedJson);
            }
            else
            {
                DisplayErrorAlert("Deck with this name already exists");
                Debug.LogError("Deck with this name already exists");
            }
        }
        else
        {
            DisplayErrorAlert("You have to put more cards, decksize: " + deckSize + " you have: " + cardDeck.Count);
            Debug.LogError("To less cards");
        }
    }
}

