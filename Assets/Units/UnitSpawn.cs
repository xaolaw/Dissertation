using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using TMPro;
using System;
using System.Linq;
using Assets.Classes;

public class UnitSpawn : MonoBehaviour
{
    public GameObject infoPrefab;
    public GameObject defaultPrefab;
    private Arena arena;

    public Color playerColor;
    public Color opponentColor;

    [Serializable]
    public struct PrefabName{
        public string name;
        public GameObject prefab;
    }
    public List<PrefabName> prefabNames = new List<PrefabName>();
    public Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
    //unit types to spwan
    public enum UnitType
    {
        DEFAULT = 0,
        DEATHRATTLE,
        COUNT,
    }
    private void Start()
    {
        arena = FindObjectOfType<Arena>();
        foreach (PrefabName prefabName in prefabNames)
        {
            prefabDict.Add(prefabName.name, prefabName.prefab);
        }
    }
    //spawning unit on map
    public bool Spawn(Tile tile, UnitType unitType, int power, bool playerUnit, string model)
    {

        if (tile != null && tile.character == null)
        {
            //adding canvas info about unit
            Vector3 canvas_position = Camera.main.WorldToScreenPoint(tile.gameObject.gameObject.transform.position);
            GameObject canvas = GameObject.Find("SpawnObjects");
            GameObject canvasInfo = Instantiate(infoPrefab, canvas_position+new Vector3(0,20,0), Quaternion.identity, canvas.transform) as GameObject;
            
            //creating an object on map
            Character new_unit = CreateUnit(tile, unitType, playerUnit, power, canvasInfo, model);
            tile.addCharacter(new_unit);

        }
        else
        {
            return false;
        }
        return true;
    }
    private Character CreateUnit(Tile tile, UnitType unitType, bool playerUnit, int power, GameObject info, string model)
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
                characterObject = Instantiate(prefabDict[model], position, Quaternion.Euler(rotation), transform);
                
                character = new Character(model, power, playerUnit, characterObject, tile, info);
                return character;

            case UnitType.DEATHRATTLE:
                characterObject = Instantiate(prefabDict[model], position, Quaternion.Euler(rotation), transform);
                
                character = new Character(model, power, playerUnit, characterObject, tile, info);

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
