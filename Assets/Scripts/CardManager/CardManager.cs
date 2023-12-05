using Assets.Classes;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    
    public Card defaultCard;
    private List<Card> cards_in_hand = new List<Card>();
    // number of cards player start with
    private int cards_number = 5;
    private Arena arena;
    private UnitSpawn unitSpawn;
    private GameObject canvas;
    public Transform[] cardSlots;
    public EventCollector eventCollector;
    //a list of all cards with models path
    private List<CardJson> cardsJson;

    private List<int> playerDeck = new List<int>() { 6, 6, 6, 6, 7, 7, 7, 7, 7, 8, 8, 8, 8, 5, 5};
    private List<int> usedCards = new List<int>();
    private int selectedDeckIndex = 0;
    private readonly string SELECTED_DECK_PATH = "Assets/CardDataBase/selectedDeckIndex.txt";
    private readonly string JSON_PATH = "Assets/CardDataBase/Decks.json"; 
    private int card_index;
    private int maxCardsInHand = 5;

    //deck graphics handler
    public Image backOfDeck;
    public Sprite[] backOfDeckSprites = new Sprite[5];//1,2,3,6,9
    public Text backOfDeckText;

    //variable for drawing cards by effects
    private bool cardIsBeingPlayed = false;
    private int triedToDraw = 0;

    //a way to get number of cards left to draw / to next deck shuffle
    public int GetCardsLeftInDeck()
    {
        return playerDeck.Count - card_index - usedCards.Count;
    }

    public static void Shuffle<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // shuffles list and resets list of cards not to draw
    public void ShuffleDeck(ref List<int> deck)
    {
        usedCards.Clear();
        card_index = 0;

        foreach (Card c in cards_in_hand)
        {
            usedCards.Add(c.GetJsonIndex());
        }

        Shuffle<int>(ref deck);
    }

    private Card CreateCard(int cardID, bool only_for_online_call = false)
    {
        Card card = Instantiate(defaultCard);
        card.Initialize(cardsJson[cardID].cardName, cardsJson[cardID].cardEnergy, cardsJson[cardID].cardImage, cardsJson[cardID].spawnUnit, cardsJson[cardID].spellEffect, cardID);
        if (only_for_online_call)
        {
            card.SetStartFields(arena, unitSpawn);
        }
        return card;
    }

    public void DrawCard(int slot = -1)
    {
        // can`t draw more cards than max limit
        if (cards_in_hand.Count >= maxCardsInHand){
            if (cardIsBeingPlayed)
                triedToDraw += 1;
            return;
        }
        // draw Card and add to the end of the hand
        if (slot.Equals(-1)){
            slot = cards_in_hand.Count;
        }
        
        // if no cards left do draw
        if (card_index + usedCards.Count >= playerDeck.Count)
        {
            ShuffleDeck(ref playerDeck);
        }

        // get card json id
        int idx = playerDeck[card_index++];
        while (usedCards.Count > 0 && usedCards.Contains(idx))
        {
            usedCards.Remove(idx);
            idx = playerDeck[card_index++];
        }

        Card card = CreateCard(idx);
        AddCard(card);
        card.transform.position = cardSlots[slot].position;
        cards_in_hand.Add(card);

        UpdateBackOfDeck();
    }

    public void StartPlayingCard()
    {
        cardIsBeingPlayed = true;
        triedToDraw = 0;
    }

    public void FinishPlayingCard()
    {
        cardIsBeingPlayed = false;
        while (triedToDraw-- > 0)
        {
            DrawCard();
        }
    }


    private void AddCard(Card card){
        card.transform.SetParent(canvas.transform);
        card.gameObject.SetActive(true);
    }


    void Start(){
        arena = FindObjectOfType<Arena>();
        unitSpawn = FindObjectOfType<UnitSpawn>();
        canvas = GameObject.Find("SpawnObjects");
        // getting actual deck
        using (var reader = new StreamReader(SELECTED_DECK_PATH)){
            selectedDeckIndex = int.Parse(reader.ReadToEnd());
        }
        using (var reader = new StreamReader(JSON_PATH)){
            var jsonDeck = reader.ReadToEnd();
            DeckCollection decks = JsonConvert.DeserializeObject<DeckCollection>(jsonDeck);
            playerDeck = new List<int> (decks.Decks[selectedDeckIndex].CardList);
        }

        
    }

    public void InitalizeHand()
    {
        cardsJson = arena.getJsonCards();
        ShuffleDeck(ref playerDeck);

        for (int i = 0; i < cards_number; i++)
        {
            DrawCard(i);
        }
    }

    public void update_cards(Card updated_card, int currentEnergy){
        cards_in_hand.Remove(updated_card);
        for(int i=0; i<cards_in_hand.Count; i++){
            cards_in_hand[i].transform.position = cardSlots[i].position;
            if (cards_in_hand[i].GetEnergy() > currentEnergy){
                cards_in_hand[i].GetComponent<Image>().color = Color.grey;
            } 
        }

        eventCollector.AddEvent(new GameEvent(updated_card.cardName, arena.playerTurn ? "Player" : "Opponent", "played"));
    }

    public void RestoreCardsColor(int currentEnergy){
        foreach (Card c in cards_in_hand)
        {
            if (c.GetEnergy() <= currentEnergy)
            c.GetComponent<Image>().color = Color.white;;
        }
    }

    private void UpdateBackOfDeck()
    {
        int cards_left = GetCardsLeftInDeck();
        if (!backOfDeck.gameObject.activeSelf && cards_left > 0)
        {
            backOfDeck.gameObject.SetActive(true);
        }

        if (cards_left >= 9)
        {
            backOfDeck.sprite = backOfDeckSprites[4];
        }
        else if (cards_left >= 6) {
            backOfDeck.sprite = backOfDeckSprites[3];
        }
        else if (cards_left >= 3)
        {
            backOfDeck.sprite = backOfDeckSprites[2];
        }
        else if (cards_left >= 2)
        {
            backOfDeck.sprite = backOfDeckSprites[1];
        }
        else if(cards_left >= 1){
            backOfDeck.sprite = backOfDeckSprites[0];
        }
        else {
            backOfDeck.gameObject.SetActive(false);
            return;
        }

        backOfDeckText.text = "Deck cards: " + cards_left;
    }

    public Vector3 GetCardPositionInHand(Card card){
        int index = cards_in_hand.IndexOf(card);
        return cardSlots[index].position;
    }

    public Card GetCardByID(int cardID, bool only_for_online_call = false)
    {
        return CreateCard(cardID, only_for_online_call);
    }

}
