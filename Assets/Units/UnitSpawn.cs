using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UnitSpawn : MonoBehaviour
{
    public GameObject unitPrefab;

    public Object infoPrefab;
    public Canvas canvas;
        
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
            Character new_unit = CreateUnit(tile, unitType, playerUnit);
            tile.addCharacter(new_unit);

            //adding canvas info about unit
            Vector3 canvas_position = Camera.main.WorldToScreenPoint(tile.gameObject.gameObject.transform.position);
            Instantiate(infoPrefab, canvas_position+new Vector3(0,20,0), Quaternion.identity, canvas.transform);

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
