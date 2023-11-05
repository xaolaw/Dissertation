using Assets.Classes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CreateDeck : MonoBehaviour
{
    public List<GameObject> cardsPlaceInUiList;
    public GameObject ArrowRight;
    public GameObject ArrowLeft;
    public GameObject ErrorAlert;
    public GameObject DynamicDeckCreator;

    //List of all cards from a game
    private List<CardJson> cardsJson;
    private List<int> cardDeck = new();
    private int page = 0;
    private readonly int cardsPerPage = 8;
    private readonly int deckSize = 10;
    private string deckName;

    //variables for page flip animation
    public List<GameObject> pages;


    private string JSON_PATH = "Assets/CardDataBase/Decks.json";

    // Start is called before the first frame update
    void Start()
    {
        ReadJson("Assets/CardDataBase/cardDB.json");
        DisplayCards();
        pages[0].transform.SetAsLastSibling();
    }
    //Display cards
    private void DisplayCards()
    {
        for(int i=(0 + page*cardsPerPage); i<(cardsPerPage + page * cardsPerPage); i++)
        {
            if (i < cardsJson.Count)
            {
                InitalizeCardOnDisplay(cardsPlaceInUiList[i % (cardsPerPage * 2)], cardsJson[i].cardImage);
            }
            else
            {
                cardsPlaceInUiList[i % (cardsPerPage * 2)].SetActive(false);
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
        if ( page >= Mathf.Floor(cardsJson.Count/cardsPerPage))
        {
            ArrowRight.SetActive(false);
        }
        ArrowLeft.SetActive(true);
        UpdateAllCounters();
        
        //check if we loop
        if (page > 1)
        {
            pages[(page - 1) % 2].transform.SetAsLastSibling();
            pages[page % 2].transform.rotation = Quaternion.identity;
        }
        //show next page
        StartCoroutine(Rotate(90, (page - 1) % 2, 0.25f));
        //pages[(page - 1) % 2].SetActive(false);
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

        pages[page % 2].transform.SetAsLastSibling();
        //show next page
        StartCoroutine(Rotate(0, page % 2, 0.25f));
    }
    //Rotate pages;
    IEnumerator Rotate(float angle, int index, float time)
    {
        float value = 0f;
        while (true)
        {
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            value += Time.deltaTime * time;
            pages[index].transform.rotation = Quaternion.Slerp(pages[index].transform.rotation, targetRotation, value); //smoothly turn the page
            float angle1 = Quaternion.Angle(pages[index].transform.rotation, targetRotation); //calculate the angle between the given angle of rotation and the current angle of rotation
            if (angle1 < 0.1f)
            {            
                if (page != 0)
                    pages[(page + 1) % 2].transform.rotation = Quaternion.Euler(0, -270, 0);
                break;
            }
            yield return null;

        }
    } 
    //////////////////////////////////////
    ///Deck building changing functions/// 
    //////////////////////////////////////
    
    //Add a card to deck
    public void AddToDeck(int index)
    {
        index += (page * cardsPerPage);
        if (CheckIfAbleToAdd(index))
        {
            cardDeck.Add(index);
            UpdateCounter(cardDeck.FindAll(x => x == index).Count, index);
        }
    }

    //Remove card from deck
    public void DeleteFromDeck(int index)
    {
        index += (page * cardsPerPage);
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
        GameObject counter = cardsPlaceInUiList[index % (cardsPerPage * 2)].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
        TMP_Text counterNumber = counter.transform.GetChild(0).GetComponent<TMP_Text>();
        counterNumber.text = value.ToString();
    }
    //Update all numbers when changing page in collection
    private void UpdateAllCounters()
    {
        for (int j=0 + page * cardsPerPage; j< cardsPerPage + page * cardsPerPage; j++)
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
            using StreamReader reader = new(JSON_PATH);
            var jsonDeck = reader.ReadToEnd();
            DeckCollection decks = JsonConvert.DeserializeObject<DeckCollection>(jsonDeck);
            reader.Close();

            if (decks.Decks.Find(d => d.Name == deckName) == null)
            {
                decks.Decks.Add(newDeck);
                string updatedJson = JsonConvert.SerializeObject(decks);
                File.WriteAllText(JSON_PATH, updatedJson);
                DisplayErrorAlert("You have successfuly saved deck");
            }
            else
            {
                DisplayErrorAlert("Deck with this name already exists [To add i want to replace]");
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

