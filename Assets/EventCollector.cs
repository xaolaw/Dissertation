using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCollector : MonoBehaviour
{
    public List<GameEvent> game_events = new List<GameEvent>();
    public List<GameEvent> new_game_events = new List<GameEvent>();

    public void AddEvent(GameEvent game_event){
        game_events.Add(game_event);
        new_game_events.Add(game_event);
    }
    public void ShowEvents(){
        foreach (GameEvent gameEvent in game_events){
            print(gameEvent);
        }
    }
}

public class GameEvent
{
    string unitName;
    string playerName;
    string eventType;

    public GameEvent(string unitName, string playerName, string eventType){
        this.unitName = unitName;
        this.playerName = playerName;
        this.eventType = eventType;
    }

    public override string ToString()
    {
        return playerName + " " + eventType + " " + unitName;
    }
}