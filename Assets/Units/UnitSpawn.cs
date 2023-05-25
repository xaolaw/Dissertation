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
    public bool Spawn(Tile tile, UnitType unitType)
    {

        if (tile != null && tile.character == null)
        {
            //creating an object on map
            Character new_unit = CreateUnit(tile.gameObject.gameObject.transform.position, unitType);
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
    private Character CreateUnit(Vector3 position,UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.DEFAULT:
                position.y += 0.1f;
                Vector3 rotation= new Vector3(0, 0, 0);
                if (position.x == 4) 
                {
                    //rotate
                    rotation =  new Vector3(0,180f,0);
                }
                transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
                GameObject characterObject = Instantiate(unitPrefab, position, Quaternion.Euler(rotation), transform);
                Character character = new Character("test1", 10, 10, characterObject);
                return character;

            default:
                return null;
        }

        
    }
}
