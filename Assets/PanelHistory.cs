using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class PanelHistory : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject buttonPrefab;
    public GameObject canvas;
    public EventCollector eventCollector;
    void Start()
    {
        print("panel start");
    }
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
