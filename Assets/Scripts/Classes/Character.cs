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
    public bool hasOnAttack;
    public bool hasOnDamage;

    private UnitStatus status;

    public enum UnitStatus
    {
        NONE        = 0,
        AGGRESSIVE  = (1 << 0),
        VOLATILE    = (1 << 1),
        EMPOWERED   = (1 << 2)
    }

    public enum MovingReason
    {
        END_TURN = 0,
        SPAWN
    }

    private System.Action<Tile, bool> deathrattle;
    private System.Action<Tile, bool> battlecry;
    private System.Action<Tile, bool> onAttack;
    private System.Action<Tile, bool> onDamage;

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

        status = UnitStatus.NONE;

        this.died = false;
        this.hasDeathRattle = false;
        // initialize functions
        deathrattle = delegate (Tile tile, bool side) { };
        battlecry = delegate (Tile tile, bool side) { };
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public bool Move(MovingReason reason)
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

        return _Move(moveDirection, reason);
    }

    // returns false if it died while moving, returns true if it survived
    public bool _Move(Arena.Direction direction, MovingReason reason)
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
            Attack(temp.character);
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

            tile.UnitMoved();
            tile = temp;
            temp.character = this;

            arena.Moving(this, gameObject.transform.position, tile.unitPosition, reason);

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
        if (HasStatus(UnitStatus.VOLATILE) || (power <= 0 && !died))
        {
            this.Die();
            return true;
        }
        powerInfo.text = "Power:" + power.ToString();

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
    }

    // attacks character, returns true if kills
    public bool Attack(Character otherCharacter)
    {
        // Do something before attack
        if (hasOnAttack)
            ActivateOnAttack();

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

    public void ActivateBattlecry()
    {
        battlecry(tile, playerUnit);
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

    public void AddDeathrattle(Action<Tile, bool> deathrattle_)
    {
        this.hasDeathRattle = true;
        this.deathrattle = deathrattle_;
    }

    public void AddBattlecry(Action<Tile, bool> battlecry_)
    {
        this.battlecry = battlecry_;
    }

    public void AddOnAttack(Action<Tile, bool> onAttack_)
    {
        this.hasOnAttack = true;
        this.onAttack = onAttack_;
    }
  
    public void AddOnDamage(Action<Tile, bool> onDamage_)
    {
        this.hasOnDamage = true;
        this.onDamage = onDamage_;
    }

    //return index to know what card to show in details info
    public int getIndexOfCard()
    {
        return index;
    }

    public bool HasStatus(UnitStatus s)
    {
        return (status & s) != 0;
    }

    public void ChangeStatus(UnitStatus s)
    {
        status ^= s;
    }
}
