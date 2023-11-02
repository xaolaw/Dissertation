using Assets.Classes;
using Newtonsoft.Json;
using System.Collections;
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
    //List of all cards from a game
    private List<CardJson> cardsJson;
    private List<int> cardDeck = new();
    private int page = 0;
    private int cardsPerPage = 10;
    private int deckSize = 10;

    // Start is called before the first frame update
    void Start()
    {
        ReadJson("Assets/CardDataBase/cardDB.json");
        DisplayCards();
    }
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

    private void ReadJson(string path)
    {
        using StreamReader reader = new(path);
        var jsonDB = reader.ReadToEnd();
        cardsJson = JsonConvert.DeserializeObject<List<CardJson>>(jsonDB);
        SortCardJsonList();

    }

    private void SortCardJsonList()
    {
        cardsJson = cardsJson.
            OrderBy(c => c.cardEnergy).
            ThenBy(c => c.cardName).ToList();
    }

    public void PageRight()
    {
        page++;
        DisplayCards();
        if ( page >= Mathf.Floor(cardsJson.Count/10))
        {
            ArrowRight.SetActive(false);
        }
        ArrowLeft.SetActive(true);
    }
    public void PageLeft()
    {
        page--;
        DisplayCards();
        if (page == 0)
        {
            ArrowLeft.SetActive(false);
        }
        ArrowRight.SetActive(true);
    }

    public void AddToDeck(int index)
    {
        if (CheckIfAbleToAdd(index))
        {
            cardDeck.Add(index + (page * 10));
            UpdateCounter(+1, index);
        }
    }

    public void DeleteFromDeck(int index)
    {
       if (CheckIfAbleToRemove(index))
       {
            cardDeck.Remove(index + (page * 10));
            UpdateCounter(-1, index);
       }
    }
    private bool CheckIfAbleToAdd(int index)
    {
        //if limit of card is exceeded return false
        if (cardDeck.FindAll(x => x == index).Count == cardsJson[index].cardInDeckLimitNumber)
            return false;
        else if (cardDeck.Count == deckSize)
            return false;
        return true;
    }
    private bool CheckIfAbleToRemove(int index)
    {
        if (cardDeck.FindAll(x => x == index).Count == 0)
            return false;
        return true;
    }
    private void UpdateCounter(int value,int index)
    {
        //Getting counter from list of objects
        GameObject counter = cardsPlaceInUiList[index].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
        TMP_Text counterNumber = counter.transform.GetChild(0).GetComponent<TMP_Text>();
        int currentNumber;
        int.TryParse(counterNumber.text, out currentNumber);
        currentNumber += value;

        counterNumber.text = currentNumber.ToString();
    }

    private void SaveDeck()
    {
        string name = "test1";
    }
}

