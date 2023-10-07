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
    private bool died;

    private System.Action<Tile, bool> deathrattle;

    public Character(string name_, int power_, bool playerUnit_, GameObject gameObject_, Tile tile_, GameObject canvasInfo_)
    {
        this.power = power_;
        this.name = name_;
        this.playerUnit = playerUnit_;
        this.gameObject = gameObject_;
        this.tile = tile_;
        this.canvasInfo = canvasInfo_;
        canvasInfo.SetActive(false);
        powerInfo = canvasInfo.GetComponentInChildren<TMP_Text>();
        powerInfo.text="Power:" + power.ToString();

        arena = (Arena)GameObject.FindObjectOfType(typeof(Arena));
        died = false;

        // initialize functions
        deathrattle = delegate (Tile tile, bool side) { };
    }

    public void Move(Arena.Direction direction)
    {
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
                    return;
                case Arena.OutOfBoarder.OPPONENT_BASE:
                    arena.opponentBase.TakeDamage(this.power);
                    Die();
                    return;
                default:
                    return;
            }
        }
        // if tile has enemy on it
        else if (temp.character != null && temp.character.playerUnit != this.playerUnit)
        {
            Attack(temp.character);
            // if died while attacking don't move
            if (power <= 0)
            {
                return;
            }
        }
        // if tile is empty
        if (temp.character == null)
        {
            tile.UnitMoved();
            tile = temp;
            temp.character = this;
            gameObject.transform.position = tile.unitPosition;

            Vector3 canvas_position = Camera.main.WorldToScreenPoint(tile.gameObject.gameObject.transform.position);
            canvasInfo.transform.position = canvas_position + new Vector3(0, 20, 0);
        }
        
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

    // attacks character, returns true if killed
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
        // possible death rattle activation

        died = true;

        deathrattle(tile, playerUnit);

        tile.UnitDied();
        UnityEngine.GameObject.Destroy(gameObject);
        UnityEngine.GameObject.Destroy(canvasInfo);
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
        this.deathrattle = deathrattle_;
    }
}
