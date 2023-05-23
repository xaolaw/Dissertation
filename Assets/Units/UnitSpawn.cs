using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unitPrefab;

    private Arena arena;

    // Start is called before the first frame update
    void Start()
    {
        //Getting a button form game
        arena = FindObjectOfType<Arena>();

    }
    public bool Spawn()
    {
        Debug.Log(arena);
        List<Tile> tileList = arena.getTileList();
        Tile tile = tileList.Find(obj => obj.isClicked);
        if (tile != null && tile.character == null)
        {
            //creating an object on map
            Vector3 unitPosition = tile.gameObject.transform.position;
            unitPosition.y += 0.5f;
            transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
            GameObject character = Instantiate(unitPrefab, unitPosition, Quaternion.identity, transform);
            tile.addCharacter(character);
        }
        else
        {
            Debug.LogWarning("tile is not cliked");
            return false;
        }
        return true;
    }
}
