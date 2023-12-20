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
    //variables for page flip animation
    public List<GameObject> pages;
    public List<GameObject> cardsPlaceInUiList;
    public GameObject ArrowRight;
    public GameObject ArrowLeft;
    public GameObject ErrorAlert;
    public Transform DynamicDeckCreatorListObjectTransform;
    public GameObject DynamicDeckCreator;
    //variable that stores deck name inside input
    public TMP_InputField InputDeckName;


    //List of all cards from a game
    private List<CardJson> cardsJson;
    private List<int> cardDeck = new();
    private int page = 0;
    private readonly int cardsPerPage = 8;
    private readonly int deckSize = 10;
    private string deckName;
    // this path did not work, maybe fix is needed
    private readonly string JSON_DECK_PATH = "Assets/Resources/CardDataBase/Decks.json";
    private readonly string JSON_COLLECTION_PATH = "CardDataBase/cardDB";
    private bool wasJsonRead = false;
    //Dict to remember new create objects in dynamic deck view
    private Dictionary<int, GameObject> dynamicDeckCreatorDictObject = new();
    //variable to Store all decks from a file
    private DeckCollection decks;

    // Start is called before the first frame update
    void Start()
    {
        if (!wasJsonRead)
            ReadJson(JSON_COLLECTION_PATH);
        wasJsonRead = true;

        DisplayCards();
        pages[0].transform.SetAsLastSibling();
        for (int i = 0; i < cardsPerPage * 2; i++)
        {
            int currentIndex = i;
  
            Button minusButton = cardsPlaceInUiList[i].transform.GetChild(1).transform.gameObject.transform.Find("Minus").GetComponent<Button>();
            minusButton.onClick.AddListener(() => DeleteFromDeck(currentIndex));

            Button plusbutton = cardsPlaceInUiList[i].transform.GetChild(1).transform.gameObject.transform.Find("Plus").GetComponent<Button>();
            plusbutton.onClick.AddListener(() => AddToDeck(currentIndex));
        }
     
    }
    public void ResetView()
    {
        page = 0;
        pages[0].transform.SetAsLastSibling();
        ArrowLeft.SetActive(false);
        ArrowRight.SetActive(true);
        pages[0].transform.rotation = Quaternion.Euler(0, 0, 0);
        pages[1].transform.rotation = Quaternion.Euler(0, 0, 0);
        cardDeck = new();
        deckName = null;
        InputDeckName.text = "";

        List<int> keysList = dynamicDeckCreatorDictObject.Keys.ToList();
        foreach (int key in keysList)
        {
            DestroyCardInDynamicView(key);
     
        }
        UpdateAllCounters();
        DisplayCards();
    }
    //Display cards
    private void DisplayCards()
    {
        for(int i=(0 + page*cardsPerPage); i<(cardsPerPage + page * cardsPerPage); i++)
        {
            if (i < cardsJson.Count)
            {
                GameObject cardPlaceInUi = cardsPlaceInUiList[i % (cardsPerPage * 2)];
                cardPlaceInUi.SetActive(true);
                InitalizeCardOnDisplay(cardPlaceInUi.transform.GetChild(0).GetComponent<Image>(), cardsJson[i].cardImage);
            }
            else
            {
                cardsPlaceInUiList[i % (cardsPerPage * 2)].SetActive(false);
            }
        }
    }
    //Display proper card
    private void InitalizeCardOnDisplay(Image imageComponent, string imagePath)
    {
        
        Sprite image = Resources.Load<Sprite>(imagePath);

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
        TextAsset json_file = Resources.Load<TextAsset>(path);
        var jsonDB = json_file.text;
        cardsJson = JsonConvert.DeserializeObject<List<CardJson>>(jsonDB);
        //SortCardJsonList();

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


        if (CheckIfAbleToAdd(index))
        {
            if (cardDeck.FindAll(x => x == index).Count == 0)
            {
                IntalizeNewCardInDynamicView(index);
            }
            cardDeck.Add(index);
            UpdateCounter(cardDeck.FindAll(x => x == index).Count, index);
        }
    }

    //Remove card from deck
    public void DeleteFromDeck(int index)
    {
 

        if (CheckIfAbleToRemove(index))
        {
            if (cardDeck.FindAll(x => x == index).Count == 1)
            {
                DestroyCardInDynamicView(index);
            }
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
    ////////////////////////////
    ///GameObject Instantiate/// 
    ////////////////////////////

    //Crate new card in DeckCardList view
    public void IntalizeNewCardInDynamicView(int index)
    {
        //we create a new object in DeckCardList
        GameObject newField = Instantiate(DynamicDeckCreator, DynamicDeckCreatorListObjectTransform);

        cardDeck.Sort();
        int position = 0;
        for (int i = 0; i < cardDeck.Count; i++)
        {
            if (cardDeck[i] > index)
            {
                break;
            }
            if (i == 0 || cardDeck[i] != cardDeck[i - 1])
            {
                position++;
            }
        }
        newField.transform.SetSiblingIndex(position);

        newField.name = index.ToString();

        Button minusButton = newField.transform.GetChild(0).transform.gameObject.transform.Find("Minus").GetComponent<Button>();
        minusButton.onClick.AddListener(() => DeleteFromDeck(index));

        Button plusbutton = newField.transform.GetChild(0).transform.gameObject.transform.Find("Plus").GetComponent<Button>();
        plusbutton.onClick.AddListener(() => AddToDeck(index));

        newField.SetActive(true);
        InitalizeCardOnDisplay(newField.transform.Find("CardField").gameObject.GetComponent<Image>(), cardsJson[index].cardImage);

        dynamicDeckCreatorDictObject.Add(index, newField);
    }

    //Destroy card in DeckCardList view
    public void DestroyCardInDynamicView(int index)
    {
        Destroy(dynamicDeckCreatorDictObject[index]);
        dynamicDeckCreatorDictObject.Remove(index);
    }

    ///////////////////////////
    ///UI changing functions/// 
    ///////////////////////////

    //Change number of cards in deck that are displayed
    private void UpdateCounter(int value, int index)
    {
        //Getting counter from list of objects
        List<GameObject> tempList = new();
        tempList.Add(cardsPlaceInUiList[index % (cardsPerPage * 2)].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject);

        if (dynamicDeckCreatorDictObject.ContainsKey(index))
        {
            tempList.Add(dynamicDeckCreatorDictObject[index].transform.GetChild(0).transform.Find("Counter").gameObject);
        }
        foreach (GameObject counter in tempList)
        {
            TMP_Text counterNumber = counter.transform.GetChild(0).GetComponent<TMP_Text>();
            counterNumber.text = value.ToString();
        }
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
            using StreamReader reader = new(JSON_DECK_PATH);
            var jsonDeck = reader.ReadToEnd();
            decks = JsonConvert.DeserializeObject<DeckCollection>(jsonDeck);
            reader.Close();
        
            if (decks.Decks.Find(d => d.Name == deckName) == null)
            {
                SaveDeckToFile();
            }
            else
            {
                DisplayErrorAlert("Deck with this name already exists");
                ErrorAlert.transform.Find("ButtonGroup").gameObject.transform.Find("SaveAnywayButton").gameObject.SetActive(true);
                Debug.LogError("Deck with this name already exists");
            }
        }
        else
        {
            DisplayErrorAlert("You have to put more cards, decksize: " + deckSize + " you have: " + cardDeck.Count);
            Debug.LogError("To less cards");
        }
    }
    //Saving deck to file
    public void SaveDeckToFile()
    {
        cardDeck.Sort();
        Deck newDeck = new()
        {
            Name = deckName.ToString(),
            CardList = cardDeck.ToArray()
        };

        Deck deck = decks.Decks.Find(d => d.Name == deckName);
        if (deck == null)
        {
            decks.Decks.Add(newDeck);
        }
        else
        {
            deck.CardList = cardDeck.ToArray();
        }

        string updatedJson = JsonConvert.SerializeObject(decks);
        File.WriteAllText(JSON_DECK_PATH, updatedJson);
        DisplayErrorAlert("You have successfuly saved deck");       
    }

    //////////////////
    ///Editing Deck/// 
    //////////////////
    
    //Get deck from editing view
    public void InitalizeDeck(List<int> deck, string name)
    {
        cardDeck = deck;
        deckName = name;

        if (!wasJsonRead)
            ReadJson(JSON_COLLECTION_PATH);
        wasJsonRead = true;

        InputDeckName.text = name;

        List<int> temp = deck.Distinct().ToList();
        for (int index = 0; index < temp.Count; index++) 
        {
            IntalizeNewCardInDynamicView(temp[index]);
            UpdateCounter(cardDeck.FindAll(x => x == index).Count, index);
        }
        UpdateAllCounters();
    }
}

