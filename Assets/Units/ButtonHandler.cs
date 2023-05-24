using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private Button button;
    private UnitSpawn unitSpawn;
    private Arena arena;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleSpawning);
        unitSpawn = FindObjectOfType<UnitSpawn>();
        arena = FindObjectOfType<Arena>();
    }

    private void HandleSpawning()
    {
        List<Tile> tileList = arena.getTileList();
        Tile tile = tileList.Find(obj => obj.isClicked);

        
        if (!unitSpawn.Spawn(tile,UnitSpawn.UnitType.DEFAULT))
        {
            Debug.LogError("Button error");
        }
        
    }
}
