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
        public GameObject playerPrefab;
        public GameObject enemyPrefab;
    }
    public List<PrefabName> prefabNames = new List<PrefabName>();
    public Dictionary<string, GameObject> playerPrefabDict = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> enemyPrefabDict = new Dictionary<string, GameObject>();
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
            playerPrefabDict.Add(prefabName.name, prefabName.playerPrefab);
            enemyPrefabDict.Add(prefabName.name, prefabName.enemyPrefab);
        }
    }
    //spawning unit on map
    public bool Spawn(Tile tile, SpawnDetails spawnDetails, bool playerUnit, int index)
    {

        if (tile != null && tile.character == null)
        {
            //adding canvas info about unit
            Vector3 canvas_position = Camera.main.WorldToScreenPoint(tile.gameObject.gameObject.transform.position);
            GameObject canvas = GameObject.Find("SpawnObjects");
            GameObject canvasInfo = Instantiate(infoPrefab, canvas_position+new Vector3(0,20,0), Quaternion.identity, canvas.transform) as GameObject;
            
            //creating an object on map
            Character new_unit = CreateUnit(tile, spawnDetails, playerUnit, canvasInfo, index);
            tile.addCharacter(new_unit);

            new_unit.ActivateBattlecry();
            if (!new_unit.hasBattlecryAnimation)
            {
               if (spawnDetails.speed > 0)
                {
                    ContinueSpawn(new_unit);
                }
            }  
        }
        else
        {
            return false;
        }
        return true;
    }
    public void ContinueSpawn(Character character)
    {
        character.StartWalking();
        character.Move(Character.MovingReason.SPAWN, character.GetSpeed());
    }

    static public Arena.PlayerUnitTarget PUTFromString(string s)
    {
        switch (s)
        {
            case "enemy":
                return Arena.PlayerUnitTarget.ENEMY;
            case "own":
                return Arena.PlayerUnitTarget.OWN;
            case "any":
            default:
                return Arena.PlayerUnitTarget.ANY;
        }
    }

    static public Arena.UnitTargetGroup UTGFromString(string s)
    {
        switch (s)
        {
            case "single":
            case "self":
                return Arena.UnitTargetGroup.SINGLE;
            case "bordering":
                return Arena.UnitTargetGroup.BORDERING;
            case "surrounding":
                return Arena.UnitTargetGroup.SURROUNDING;
            case "in_front":
                return Arena.UnitTargetGroup.IN_FRONT;
            case "behind":
                return Arena.UnitTargetGroup.BEHIND;
            case "sideways":
                return Arena.UnitTargetGroup.SIDEWAYS;
            case "single_behind":
                return Arena.UnitTargetGroup.SINGLE_BEHIND;
            case "single_in_front":
                return Arena.UnitTargetGroup.SINGLE_IN_FRONT;
            case "all":
            default:
                return Arena.UnitTargetGroup.ALL;
        }
    }

    private GameObject GetPrefab(string s, bool playerUnit)
    {
        if (playerUnit && playerPrefabDict.ContainsKey(s))
            return playerPrefabDict[s];
        if (!playerUnit && enemyPrefabDict.ContainsKey(s))
            return enemyPrefabDict[s];
        return defaultPrefab;
    }

    private Character CreateUnit(Tile tile, SpawnDetails spawnDetails, bool playerUnit, GameObject info, int index)
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

        characterObject = Instantiate(GetPrefab(spawnDetails.cardModel, playerUnit), position, Quaternion.Euler(rotation), transform);
                
        character = new Character(spawnDetails.cardModel, spawnDetails.cardPower, playerUnit, characterObject, tile, info, index, spawnDetails.speed);

        character.SetUnitType(spawnDetails.cardType);

        if (spawnDetails.status != null)
            character.ChangeStatus(Character.GetStatusFromString(spawnDetails.status));

        if (spawnDetails.deathrattle != null)
        {
            // set deathrattle
            character.AddDeathrattle(spawnDetails.deathrattle.GenerateAction(arena, Arena.EffectReason.DEATHRATTLE), spawnDetails.deathrattle.HasAnimation());
        }

        if (spawnDetails.battlecry != null)
        {
            // set battlecry
            character.AddBattlecry(spawnDetails.battlecry.GenerateAction(arena, Arena.EffectReason.BATTLECRY), spawnDetails.battlecry.HasAnimation());
        }

        if (spawnDetails.onTurnEnd != null)
        {
            // set on turn beggining effect
            character.AddOnEndturn(spawnDetails.onTurnEnd.GenerateAction(arena, Arena.EffectReason.ON_END_TURN), spawnDetails.onTurnEnd.HasAnimation());
        }
        
        if (spawnDetails.onAttack != null)
        {
            // set on attack effect
            character.AddOnAttack(spawnDetails.onAttack.GenerateAction(arena, Arena.EffectReason.ON_ATTACK), spawnDetails.onAttack.HasAnimation());
        }
        
        if (spawnDetails.onDamage != null)
        {
            // set on damage effect
            character.AddOnDamage(spawnDetails.onDamage.GenerateAction(arena, Arena.EffectReason.ON_DAMAGE), spawnDetails.onDamage.HasAnimation());
        }

        return character;
    }

}
