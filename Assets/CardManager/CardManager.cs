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
    private int cards_number = 3;
    private Arena arena;
    private UnitSpawn unitSpawn;
    private GameObject canvas;
    public Transform[] cardSlots;
    public EventCollector eventCollector;
    //a list of all cards with models path
    private List<CardJson> cardsJson;

    private List<int> playerDeck = new List<int>() { 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 3, 3, 3, 3, 3};
    private List<int> usedCards = new List<int>();
    private int card_index;
    private int maxCardsInHand = 3;

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

    public void DrawCard(int slot)
    {
        // can`t draw more cards than max limit
        if (cards_in_hand.Count >= maxCardsInHand){
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
    }
   
    private void AddCard(Card card){
        card.transform.SetParent(canvas.transform);
        card.gameObject.SetActive(true);
    }


    void Start(){
        arena = FindObjectOfType<Arena>();
        unitSpawn = FindObjectOfType<UnitSpawn>();
        canvas = GameObject.Find("SpawnObjects");
        
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

    void OnCollisionEnter (Collision collision){
     //   print("clickerd123");
    }

    public void update_cards(Card updated_card){
        cards_in_hand.Remove(updated_card);
        for(int i=0; i<cards_in_hand.Count; i++){
            cards_in_hand[i].transform.position = cardSlots[i].position;
        }

        //DrawCard(cards_in_hand.Count);

        eventCollector.AddEvent(new GameEvent(updated_card.cardName, arena.playerTurn ? "Player" : "Opponent", "played"));
    }

    public Vector3 GetCardPositionInHand(Card card){
        int index = cards_in_hand.IndexOf(card);
        return cardSlots[index].position;
    }

    public Card GetCardByID(int cardID, bool only_for_online_call = false)
    {
        return CreateCard(cardID, only_for_online_call);
    }

    void Update(){
  //      if (Input.GetMouseButtonDown(0)){
   //         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
   //         print(mousePos);
   //     }
    }
}
