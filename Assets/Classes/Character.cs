using System;
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

    private Arena arena;
    private EventCollector eventCollector;
    private bool died;
    private int index;
    public bool hasDeathRattle;

    private System.Action<Tile, bool> deathrattle;

    public Character(string name_, int power_, bool playerUnit_, GameObject gameObject_, Tile tile_, GameObject canvasInfo_, int index_)
    {
        this.power = power_;
        this.name = name_;
        this.playerUnit = playerUnit_;
        this.gameObject = gameObject_;
        this.tile = tile_;
        this.canvasInfo = canvasInfo_;
        this.index = index_;
        canvasInfo.SetActive(false);
        powerInfo = canvasInfo.GetComponentInChildren<TMP_Text>();
        powerInfo.text="Power:" + power.ToString();

        arena = (Arena)GameObject.FindObjectOfType(typeof(Arena));
        eventCollector = GameObject.FindObjectOfType<EventCollector>();
        this.died = false;
        this.hasDeathRattle = false;
        // initialize functions
        deathrattle = delegate (Tile tile, bool side) { };
    }

    // returns false if it died while moving, returns true if it survived
    public bool Move(Arena.Direction direction)
    {
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
                    return false;
                case Arena.OutOfBoarder.OPPONENT_BASE:
                    arena.opponentBase.TakeDamage(this.power);
                    Die();
                    return false;
                default:
                    return false;
            }
        }
        // if tile has enemy on it
        else if (temp.character != null && temp.character.playerUnit != this.playerUnit)
        {
            Attack(temp.character);
            // if died while attacking don't move
            if (HasDied())
            {
                return false;
            }
        }
        // if tile is empty
        if (temp.character == null)
        {
            tile.UnitMoved();
            tile = temp;
            temp.character = this;
            gameObject.transform.position = tile.unitPosition;

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
        if (power <= 0 && !died)
        {
            this.Die();
            return true;
        }
        powerInfo.text = "Power:" + power.ToString();
        return false;
    }

    // attacks character, returns true if kills
    public bool Attack(Character otherCharacter)
    {
        // Do something before attack

        int damage_to_take = otherCharacter.power;
        bool killed = otherCharacter.TakeDamage(this.power);
        TakeDamage(damage_to_take);
        return killed;
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

    public void ActivateDeathRattle()
    {
        // Do possible Deathrattle
        deathrattle(tile, playerUnit);

        // Clean up arena board and add death to history
        tile.UnitDied();
        UnityEngine.GameObject.Destroy(gameObject);
        UnityEngine.GameObject.Destroy(canvasInfo);
        eventCollector.AddEvent(new GameEvent(this.name, this.playerUnit ? "Opponent" : "Player", "killed"));

        // Activate possible next Deathrattles
        if (hasDeathRattle)
            arena.NextDying();
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

    public void AddDeathrattle(Action<Tile, bool> deathrattle_)
    {
        this.hasDeathRattle = true;
        this.deathrattle = deathrattle_;
    }

    //return index to know what card to show in details info
    public int getIndexOfCard()
    {
        return index;
    }
}
