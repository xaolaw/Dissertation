using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using TMPro;

public class UnitSpawn : MonoBehaviour
{
    public GameObject defaultUnitPrefab;

    public Object infoPrefab;

    public Color playerColor;
    public Color opponentColor;
        
    //unit types to spwan
    public enum UnitType
    {
        DEFAULT = 0,
        DEATHRATTLE,
        COUNT,
    }
    //spawning unit on map
    public bool Spawn(Tile tile, UnitType unitType, int power, bool playerUnit)
    {

        if (tile != null && tile.character == null)
        {
            //adding canvas info about unit
            Vector3 canvas_position = Camera.main.WorldToScreenPoint(tile.gameObject.gameObject.transform.position);
            GameObject canvas = GameObject.Find("SpawnObjects");
            GameObject canvasInfo = Instantiate(infoPrefab, canvas_position+new Vector3(0,20,0), Quaternion.identity, canvas.transform) as GameObject;
            
            //creating an object on map
            Character new_unit = CreateUnit(tile, unitType, playerUnit, power, canvasInfo);
            tile.addCharacter(new_unit);

        }
        else
        {
            return false;
        }
        return true;
    }
    private Character CreateUnit(Tile tile, UnitType unitType, bool playerUnit, int power, GameObject info)
    {
        Vector3 position = tile.unitPosition;
        Vector3 rotation = new Vector3(0, 0, 0);
        if (playerUnit)
        {
            //rotate
            rotation = new Vector3(0, 180f, 0);
        }
        GameObject characterObject = null;
        Character character = null;
        switch (unitType)
        {
            case UnitType.DEFAULT:
                transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
                characterObject = Instantiate(defaultUnitPrefab, position, Quaternion.Euler(rotation), transform);
                {
                    MeshRenderer cubeMesh = characterObject.transform.Find("Cube").GetComponent<MeshRenderer>();
                    MeshRenderer cyllinderMesh = characterObject.transform.Find("Cylinder").GetComponent<MeshRenderer>();
                    if (playerUnit)
                    {
                        cubeMesh.material.color = playerColor;
                        cyllinderMesh.material.color = playerColor;
                    }
                    else
                    {
                        cubeMesh.material.color = opponentColor;
                        cyllinderMesh.material.color = opponentColor;
                    }
                }
                character = new Character("Tank", power, playerUnit, characterObject, tile, info);
                return character;

            case UnitType.DEATHRATTLE:
                transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
                characterObject = Instantiate(defaultUnitPrefab, position, Quaternion.Euler(rotation), transform);
                {
                    MeshRenderer cubeMesh = characterObject.transform.Find("Cube").GetComponent<MeshRenderer>();
                    MeshRenderer cyllinderMesh = characterObject.transform.Find("Cylinder").GetComponent<MeshRenderer>();
                    if (playerUnit)
                    {

                        cubeMesh.material.color = playerColor;
                        cyllinderMesh.material.color = playerColor;
                    }
                    else
                    {
                        cubeMesh.material.color = opponentColor;
                        cyllinderMesh.material.color = opponentColor;
                    }
                }
                character = new Character("Tank", power, playerUnit, characterObject, tile, info);

                // set deathrattle
                System.Action<Tile, bool> newDeathrattle = delegate (Tile origintile, bool side)
                {
                    origintile.Damage(Arena.PlayerUnitTarget.ENEMY, Arena.UnitTargetGroup.ALL, side, 3);
                };
                character.AddDeathrattle(newDeathrattle);

                return character;

            default:
                return null;
        }

        
    }
}
