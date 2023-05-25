using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public Transform[] cardSlots;
    int i = 0;
    
    public void DrawCards(){
        while(i < cards.Count){
            Card card = cards[i];
            card.gameObject.SetActive(true);
            card.transform.position = cardSlots[i].position;
            print(card.card_name);
            i++;
        }
    }

    void Start(){
        DrawCards();
    }
}
