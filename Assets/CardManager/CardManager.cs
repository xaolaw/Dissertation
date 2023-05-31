using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    private List<Card> cards_in_hand = new List<Card>();
    private int cards_number = 3;
    private Arena arena;
    private Canvas canvas;
    public Transform[] cardSlots;
    private int idx = 0;
    
    public void DrawCards(){
        for(int i = 0; i < cards_number; i++){
            idx = Random.Range(0, cards.Count);
            Card card = Instantiate(cards[idx]);
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
        canvas = FindObjectOfType<Canvas>();
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
        idx = Random.Range(0, cards.Count);
        Card card = Instantiate(cards[idx]);
        addCard(card);
        card.transform.position = cardSlots[cards_in_hand.Count].position;
        cards_in_hand.Add(card);
    }

    void Update(){
  //      if (Input.GetMouseButtonDown(0)){
   //         Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
   //         print(mousePos);
   //     }
    }
}
