using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseEventTile : MonoBehaviour
{
    private List<Tile> tileList;
    GameObject attachedGameObject;
    Tile tile;
    private void Awake()
    {
        tileList = FindObjectOfType<Arena>().getTileList();
        attachedGameObject = gameObject;
        tile = tileList.Find(obj => obj.gameObject == attachedGameObject);
    }

    private void OnMouseEnter()
    {
        tile.Select();
       
    }
    private void OnMouseOver()
    {
        
    }
    private void OnMouseExit()
    {
        tile.UnSelect();
    }
}
