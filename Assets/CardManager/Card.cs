using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler
{
        public string card_name;
        public string description;
        public UnitSpawn.UnitType unitType;
        public int hp;
        private CardManager cm;
        private Arena arena;
        private UnitSpawn unitSpawn;


        public void Start(){
            cm = FindObjectOfType<CardManager>();
            arena = FindObjectOfType<Arena>();
            unitSpawn = FindObjectOfType<UnitSpawn>();
        }

       

    private void HandleSpawning()
    {
        List<Tile> tileList = arena.getTileList();
        Tile tile = tileList.Find(obj => obj.isClicked);

        
        if (!unitSpawn.Spawn(tile, unitType, arena.playerTurn))
        {
            Debug.LogError("Spawning error");
        }
        
    }

        public void OnPointerDown (PointerEventData eventData){
            HandleSpawning();
        }
}