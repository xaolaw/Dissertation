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
    private int idx = 0;
    public EventCollector eventCollector;
    private List<CardJson> cardsJson;
        
    //a list of all cards with models path
    public void DrawCards()
    {
        for (int i = 0; i < cards_number; i++)
        {
            //temporary it's a random card from json file
            idx = Random.Range(0, cardsJson.Count);
            
            Card card = Instantiate(defaultCard);
            card.Initialize(cardsJson[idx].cardName, cardsJson[idx].cardPower, cardsJson[idx].cardImage, cardsJson[idx].cardModel, cardsJson[idx].cardDetails);
            addCard(card);
            card.transform.position = cardSlots[i].position;
            cards_in_hand.Add(card);
        }
    
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
        DrawCards();
    }

    void OnCollisionEnter (Collision collision){
     //   print("clickerd123");
    }

    public void update_cards(Card updated_card){
        cards_in_hand.Remove(updated_card);
        for(int i=0; i<cards_in_hand.Count; i++){
            cards_in_hand[i].transform.position = cardSlots[i].position;
        }
        idx = Random.Range(0, cardsJson.Count);
        Card card = Instantiate(defaultCard);
        card.Initialize(cardsJson[idx].cardName, cardsJson[idx].cardPower, cardsJson[idx].cardImage, cardsJson[idx].cardModel, cardsJson[idx].cardDetails);
        addCard(card);
        card.transform.position = cardSlots[cards_in_hand.Count].position;
        cards_in_hand.Add(card);
        eventCollector.AddEvent(new GameEvent(updated_card.cardName, arena.playerTurn ? "Player" : "Opponent", "played"));
    }

    void Update(){
  //      if (Input.GetMouseButtonDown(0)){
   //         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
   //         print(mousePos);
   //     }
    }
}
