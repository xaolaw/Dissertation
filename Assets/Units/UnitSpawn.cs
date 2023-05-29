using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unitPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }
    public enum UnitType
    {
        DEFAULT = 0,
        COUNT,
    }
    public bool Spawn(Tile tile, UnitType unitType, bool playerUnit)
    {

        if (tile != null && tile.character == null)
        {
            //creating an object on map
            tile.addCharacter(CreateUnit(tile, unitType, playerUnit));
        }
        else
        {
            return false;
        }
        return true;
    }
    private Character CreateUnit(Tile tile, UnitType unitType, bool playerUnit)
    {
        Vector3 position = tile.gameObject.gameObject.transform.position;
        switch (unitType)
        {
            case UnitType.DEFAULT:
                position.y += 0.1f;
                Vector3 rotation= new Vector3(0, 0, 0);
                if (playerUnit) 
                {
                    //rotate
                    rotation =  new Vector3(0,180f,0);
                }
                transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
                GameObject characterObject = Instantiate(unitPrefab, position, Quaternion.Euler(rotation), transform);
                Character character = new Character("test1", 10, 10, playerUnit, characterObject, tile);
                return character;

            default:
                return null;
        }

        
    }
    
}
