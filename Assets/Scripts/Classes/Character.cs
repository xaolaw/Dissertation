﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
public class Character
{
    string name;
    int power;              // attack and hp at the same time - combined power
    public bool playerUnit; // true - player unit / false - enemy unit
    GameObject gameObject;
    GameObject canvasInfo;
    TMP_Text powerInfo;
    Tile tile;

    private Animator animator;
    private Arena arena;
    private EventCollector eventCollector;
    private bool died;
    private int index;
    private int speed;
    public bool hasDeathRattle;
    public bool hasOnEndTurn;
    public bool hasOnAttack;
    public bool hasOnDamage;

    public bool hasBattlecryAnimation = false;
    public bool hasDeathRattleAnimation = false;
    public bool hasOnEndTurnAnimation= false;
    public bool hasOnAttackAnimation= false;
    public bool hasOnDamageAnimation= false;


    private UnitType type = UnitType.CREATURE;
    private bool moving = true;

    private UnitStatus status;

    public enum UnitType
    {
        CREATURE = 0,
        BUILDING
    }

    public enum UnitStatus
    {
        NONE        = 0,
        AGGRESSIVE  = (1 << 0),
        VOLATILE    = (1 << 1),
        EMPOWERED   = (1 << 2)
    }

    public enum AttackReason
    {
        MOVING = 0,
        SPAWN
    }

    public enum MovingReason
    {
        END_TURN = 0,
        SPAWN
    }

    private System.Action<Tile, bool> deathrattle;
    private System.Action<Tile, bool> battlecry;
    private System.Action<Tile, bool> onEndTurn;
    private System.Action<Tile, bool> onAttack;
    private System.Action<Tile, bool> onDamage;

    private Character targetCharacter;
    private AttackReason attackReason;

    private Arena.Direction moveDirection;
    private MovingReason movingReason;
    private int movesToMake;

    public static UnitStatus GetStatusFromString(string s)
    {
        char[] delimeter_characters = { ',', ' ', '|' };
        string[] status_strings = s.Split(delimeter_characters);

        UnitStatus status = UnitStatus.NONE, temp;

        foreach (string status_string in status_strings)
        {
            switch (status_string)
            {
                case "aggressive":
                    temp = UnitStatus.AGGRESSIVE;
                    break;
                case "volatile":
                    temp = UnitStatus.VOLATILE;
                    break;
                case "empowered":
                    temp = UnitStatus.EMPOWERED;
                    break;
                default:
                    temp = UnitStatus.NONE;
                    break;
            }
            status |= temp;
        }

        return status;
    }

    public Character(string name_, int power_, bool playerUnit_, GameObject gameObject_, Tile tile_, GameObject canvasInfo_, int index_, int speed_)
    {
        this.power = power_;
        this.name = name_;
        this.playerUnit = playerUnit_;
        this.gameObject = gameObject_;
        animator = gameObject.GetComponent<Animator>();
        this.tile = tile_;
        this.canvasInfo = canvasInfo_;
        this.index = index_;
        this.speed = speed_;
        canvasInfo.SetActive(false);
        powerInfo = canvasInfo.GetComponentInChildren<TMP_Text>();
        powerInfo.text = power.ToString();

        arena = (Arena)GameObject.FindObjectOfType(typeof(Arena));
        eventCollector = GameObject.FindObjectOfType<EventCollector>();

        status = UnitStatus.NONE;

        this.died = false;
        this.hasDeathRattle = false;
        this.hasOnEndTurn = false;
        // initialize functions
        deathrattle = delegate (Tile tile, bool side) { };
        battlecry = delegate (Tile tile, bool side) { };
        onEndTurn = delegate (Tile tlie, bool side) { };
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public bool Move(MovingReason reason, int moves_to_make = 1)
    {
        bool turn = arena.playerTurn;
        Arena.Direction moveDirection = moveDirection = turn ? Arena.Direction.UP : Arena.Direction.DOWN;
        if (HasStatus(Character.UnitStatus.AGGRESSIVE))
        {
            bool found_unit = false;
            Tile tile_to_check = arena.GetTile(tile.id, moveDirection);
            Arena.OutOfBoarder oob = arena.GetTargetInfo(tile.id, moveDirection);
            if (oob == (turn?Arena.OutOfBoarder.OPPONENT_BASE: Arena.OutOfBoarder.PLAYER_BASE)
                || (oob == Arena.OutOfBoarder.INSIDE && tile_to_check.character != null && tile_to_check.character.playerUnit != arena.playerTurn))
            {
                found_unit = true;
            }
            if (!found_unit)
            {
                moveDirection = turn ? Arena.Direction.LEFT : Arena.Direction.RIGHT;
                if (arena.GetTargetInfo(tile.id, moveDirection) == Arena.OutOfBoarder.INSIDE)
                {
                    tile_to_check = arena.GetTile(tile.id, moveDirection);
                    found_unit = tile_to_check.character != null && tile_to_check.character.playerUnit != turn;
                }
            }
            if (!found_unit)
            {
                moveDirection = turn ? Arena.Direction.RIGHT : Arena.Direction.LEFT;
                if (arena.GetTargetInfo(tile.id, moveDirection) == Arena.OutOfBoarder.INSIDE)
                {
                    tile_to_check = arena.GetTile(tile.id, moveDirection);
                    found_unit = tile_to_check.character != null && tile_to_check.character.playerUnit != turn;
                }
            }

            if (!found_unit)
                moveDirection = turn ? Arena.Direction.UP : Arena.Direction.DOWN;
        }

        return _Move(moveDirection, reason, moves_to_make);
    }

    // returns false if it died while moving, returns true if it survived
    public bool _Move(Arena.Direction direction, MovingReason reason, int moves_to_make)
    {
        Debug.Log("try moving");
        movingReason = reason;
        movesToMake = moves_to_make;
        moveDirection = direction;
        if (HasDied())
            return false;
        Tile temp = tile.GetTile(direction);
        // moves out of boarder - moved sideways or moves in base
        if (temp == null)
        {
            Arena.OutOfBoarder info = arena.GetTargetInfo(tile.id, direction);
            switch (info)
            {
                case Arena.OutOfBoarder.PLAYER_BASE:
                    arena.playerBase.TakeDamage(this.power);
                    Die();
                    if (reason == MovingReason.END_TURN)
                        arena.CheckEndTurnTile();
                    return false;
                case Arena.OutOfBoarder.OPPONENT_BASE:
                    arena.opponentBase.TakeDamage(this.power);
                    Die();
                    if (reason == MovingReason.END_TURN)
                        arena.CheckEndTurnTile();
                    return false;
                default:
                    return false;
            }
        }
        // if tile has enemy on it
        else if (temp.character != null && temp.character.playerUnit != this.playerUnit)
        {
            Attack(temp.character, AttackReason.MOVING);
            // if died while attacking don't move
            if (HasDied())
            {
                if (reason == MovingReason.END_TURN)
                    arena.CheckEndTurnTile();
                return false;
            }
        }
        // if tile is empty
        if (temp.character == null)
        {
            Debug.Log("moves");
            tile.UnitMoved();
            tile = temp;
            temp.character = this;

            arena.Moving(this, gameObject.transform.position, tile.unitPosition, reason, moves_to_make);

            //gameObject.transform.position = tile.unitPosition;

            arena.CheckFrontline(temp.id, playerUnit);

            Vector3 canvas_position = Camera.main.WorldToScreenPoint(tile.gameObject.gameObject.transform.position);
            canvasInfo.transform.position = canvas_position + new Vector3(0, 20, 0);
        }
        return true;
        
    }

    // takes damage, returns true if killed
    public bool TakeDamage(int dmg)
    {
        this.power -= dmg;
        powerInfo.text = power.ToString();

        if ((HasStatus(UnitStatus.VOLATILE) || power <= 0) && !died)
        {
            powerInfo.text = "0";
            this.Die();
            return true;
        }
       

        // if survived damage try to activate on damage taken effect
        if (!HasDied() && hasOnDamage)
        {
            ActivateOnDamage();
            return HasDied();
        }

        return false;
    }

    public void GivePower(int power)
    {
        this.power += power;
        powerInfo.text = this.power.ToString();
    }

    // attacks character
    public void Attack(Character otherCharacter, AttackReason reason)
    {
        // if was already dead we do nothing
        if (otherCharacter.HasDied())
            return;

        targetCharacter = otherCharacter;
        attackReason = reason;

        // Do something before attack
        if (hasOnAttack)
        {
            ActivateOnAttack();
            return;
        }

        ContinueAttack();
    }

    public void ContinueAttack()
    {
        // if out ability killed it we do not take negative damage
        if (!targetCharacter.HasDied())
        {

            Debug.Log("attacks");

            int damage_to_take = targetCharacter.power;
            bool killed = targetCharacter.TakeDamage(this.power);
            TakeDamage(damage_to_take);
        }

        switch (attackReason)
        {
            case AttackReason.MOVING:
                _Move(moveDirection, movingReason, movesToMake);
                break;
            case AttackReason.SPAWN:
                arena.unitSpawn.ContinueSpawn(this);
                break;
            default:
                Debug.LogError("no attack reason");
                break;
        }
    }

    public void Die()
    {
        // Set dead status and go to queue of deathrattles if has one
        died = true;
        if (hasDeathRattle)
        {
            arena.AddToDyingQueue(this);
            return;
        }

        // Clean up arena board and add death to history
        tile.UnitDied();
        UnityEngine.GameObject.Destroy(gameObject);
        UnityEngine.GameObject.Destroy(canvasInfo);
        eventCollector.AddEvent(new GameEvent(this.name, this.playerUnit ? "Opponent" : "Player", "killed"));
    }

    public bool HasDied()
    {
        return died;
    }

    public void ActivateBattlecry()
    {
        battlecry(tile, playerUnit);
    }

    public void ActivateDeathRattle()
    {
        // Do possible Deathrattle
        deathrattle(tile, playerUnit);

        if (!this.hasDeathRattleAnimation)
        {
            // Clean up arena board and add death to history
            ContinueDeathratlle();
        }
    }
    public void ContinueDeathratlle()
    {
        // Clean up arena board and add death to history
        tile.UnitDied();
        UnityEngine.GameObject.Destroy(gameObject);
        UnityEngine.GameObject.Destroy(canvasInfo);
        eventCollector.AddEvent(new GameEvent(this.name, this.playerUnit? "Opponent" : "Player", "killed"));

        // Activate possible next Deathrattles
        if (hasDeathRattle)
            arena.NextDying();
}

    public void ActivateOnEndTurn()
    {
        onEndTurn(tile, playerUnit);
    }
  
    public void ActivateOnAttack()
    {
        onAttack(tile, playerUnit);
    }
  
    public void ActivateOnDamage()
    {
        onDamage(tile, playerUnit);
    }

    public string toString() {
        return "Name: " + name + "\nPower: " + power;
    }

    public void DisplayAttackInfo()
    {
        canvasInfo.SetActive(true);
    }

    public void HideAttackInfo()
    {
        canvasInfo.SetActive(false);
    }

    public void AddDeathrattle(Action<Tile, bool> deathrattle_, bool animation)
    {
        this.hasDeathRattle = true;
        this.deathrattle = deathrattle_;
        this.hasDeathRattleAnimation = animation;
    }

    public void AddBattlecry(Action<Tile, bool> battlecry_, bool animation)
    {
        this.battlecry = battlecry_;
        this.hasBattlecryAnimation = animation;
    }

    public void AddOnEndturn(Action<Tile, bool> onEndTurn_, bool animation)
    {
        this.hasOnEndTurn = true;
        this.onEndTurn = onEndTurn_;
        this.hasOnEndTurnAnimation = animation;
    }
    public void AddOnAttack(Action<Tile, bool> onAttack_, bool animation)
    {
        this.hasOnAttack = true;
        this.onAttack = onAttack_;
        this.hasOnAttackAnimation = animation;
    }
  
    public void AddOnDamage(Action<Tile, bool> onDamage_, bool animation)
    {
        this.hasOnDamage = true;
        this.onDamage = onDamage_;
        this.hasOnDamageAnimation = animation;
    }

    //return index to know what card to show in details info
    public int GetIndexOfCard()
    {
        return index;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public bool HasStatus(UnitStatus s)
    {
        return (status & s) != 0;
    }

    public void ChangeStatus(UnitStatus s)
    {
        status ^= s;
    }

    public void SetUnitType(string type_s)
    {
        switch (type_s)
        {
            case "creature":
                type = UnitType.CREATURE;
                break;
            case "building":
                type = UnitType.BUILDING;
                moving = false;
                break;
            default:
                type = UnitType.CREATURE;
                break;
        }
    }

    public bool IsType(UnitType type)
    {
        return this.type == type;
    }

    public bool IsMovingType()
    {
        return moving;
    }

    public int GetTileID()
    {
        return tile.id;
    }

    public void StartWalking()
    {
        if (animator)
        {
            animator.SetBool("isWalking", true);
        }
    }

    public void EndWalking()
    {
        if (animator)
        {
            animator.SetBool("isWalking", false);
        }
    }
}
