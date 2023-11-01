using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class PanelHistory : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject canvas;
    public EventCollector eventCollector;

    public void CreatePanel(){
        print("panel show");
        foreach (GameEvent gameEvent in eventCollector.new_game_events){
        GameObject tile = Instantiate(buttonPrefab, transform);
        TextMeshProUGUI mText = tile.GetComponentInChildren<TextMeshProUGUI>();
        mText.text = gameEvent.ToString();
        tile.transform.SetParent(canvas.transform);
        tile.gameObject.SetActive(true);
        }
        eventCollector.new_game_events = new List<GameEvent>();
    }
}
