using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEditor;

public class GamePlayTest
{
    [OneTimeSetUp]
    public void LoadScene(){
        SceneManager.LoadScene("MainMenuScene");
    }
    public List<Card> GetCurrentCards(){
        var cards = GameObject.FindGameObjectsWithTag("card");
        List<Card> cardsList = new List<Card>();
        foreach (var card in cards){
            cardsList.Add(card.GetComponent<Card>());
        }
        return cardsList;
    }
    public Tile GetTileToPlayOn(){
        var arena = GameObject.Find("Arena").GetComponent<Arena>();
        var tileList = arena.GetTileList();
        foreach(var tile in tileList){
            if(arena.IsBehindFrontline(tile, arena.playerTurn) && tile.character == null){
                return tile;
            }
        }
        return null;
    }
    [UnityTest]
    public IEnumerator GamePlayTestWithEnumeratorPasses()
    {
        // gameplay scenario without using spells and with random cards in hand
        int TURNS_NUMBER = 200;
        GameObject.Find("Play Button").GetComponent<Button>().onClick.Invoke();
        yield return null;
        var endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();

        var arena = GameObject.Find("Arena").GetComponent<Arena>();
        var tileList = arena.GetTileList();
        List<Card> cardsList = GetCurrentCards();
        for (int i = 0; i < TURNS_NUMBER; i++){
            cardsList = GetCurrentCards();
            foreach (Card card in cardsList){
                if (!card.isSpell()){
                    Tile tile = GetTileToPlayOn();
                    // if there is no tile to play card on
                    if (tile == null) break;
                    tile.Select();
                    // if player has not got enough energy, no error should occur, card just should not have been played 
                    card.HandleSpawning();
                    tile.UnSelect();
                    break;
                }
            }
        bool who = arena.playerTurn;
        arena.SafeEndTurn();
        while(who==arena.playerTurn && !arena.hasEnded){
            yield return null;
        }
        if(arena.hasEnded) break;
        }
        Assert.IsTrue(arena.hasEnded);
    }
}
