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
    private GameObject canvas;
    public Transform[] cardSlots;
    public EventCollector eventCollector;
    //a list of all cards with models path
    private List<CardJson> cardsJson;

    private List<int> playerDeck = new List<int>() { 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 3, 3, 3, 3, 3};
    private List<int> usedCards = new List<int>();
    private int card_index;

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

    public void DrawCard(int slot)
    {
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

        Card card = Instantiate(defaultCard);
        card.Initialize(cardsJson[idx].cardName, cardsJson[idx].cardEnergy, cardsJson[idx].cardImage, cardsJson[idx].spawnUnit, cardsJson[idx].spellEffect, idx);
        addCard(card);
        card.transform.position = cardSlots[slot].position;
        cards_in_hand.Add(card);
    }
   
    private void addCard(Card card){
        card.transform.SetParent(canvas.transform);
        card.gameObject.SetActive(true);
    }


    void Start(){
        arena = FindObjectOfType<Arena>();
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

        DrawCard(cards_in_hand.Count);

        eventCollector.AddEvent(new GameEvent(updated_card.cardName, arena.playerTurn ? "Player" : "Opponent", "played"));
    }

    void Update(){
  //      if (Input.GetMouseButtonDown(0)){
   //         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
   //         print(mousePos);
   //     }
    }
}
