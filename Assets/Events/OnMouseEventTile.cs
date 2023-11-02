using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseEventTile : MonoBehaviour
{
    private List<Tile> tileList;
    private Arena arena;
    GameObject attachedGameObject;
    Tile tile;
    private void Awake()
    {
        arena = FindObjectOfType<Arena>();
        tileList = arena.GetTileList();
        attachedGameObject = gameObject;
        tile = tileList.Find(obj => obj.gameObject == attachedGameObject);
    }

    private void OnMouseEnter()
    {
        if (!arena.areMenus)
        {
            tile.Select();
        }
       
    }
    private void OnMouseOver()
    {
        
    }
    private void OnMouseExit()
    {
        tile.UnSelect();
    }
}
