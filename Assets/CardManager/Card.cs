using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
        public string card_name;
        public string description;
        public int hp;
        private CardManager gm;

        public void Start(){
            gm = FindObjectOfType<CardManager>();
        }
}