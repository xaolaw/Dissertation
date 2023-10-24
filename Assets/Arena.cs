using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Classes;
using Newtonsoft.Json;
using System.IO;

public class Arena : MonoBehaviour
{

    ////////////////////////////////////////////////
    /// Variables for ground and tiles and bases ///
    ////////////////////////////////////////////////
    
    public GameObject tilePrefab; // assign the tile prefab in the Inspector
    //Currently clicked tile
    private List<Tile> tileList = new List<Tile>();

    public float groundOffset = 0.1f;
    public int rows = 5;
    public int columns = 4;

    public Base playerBase;
    public Base opponentBase;

    /////////////////////////////////////////////////
    /// Variables for turn mechanics and damaging ///
    /////////////////////////////////////////////////

    private TurnButton turn_button;

    public bool playerTurn = true;
    private int energyFlow = 2;

    //Dying units
    private Queue dyingUnits = new Queue();
    // if there are units with deathrattles being killed by other deathrattles (future event queue)
    private bool deathrattleWave = false;

    //Direction for finding tiles
    public enum Direction
    {
        UP = 0,
        DOWN,
        LEFT,
        RIGHT,
        UL,
        UR,
        DL,
        DR
    }

    //Border output info
    public enum OutOfBoarder
    {
        INSIDE = 0,
        OUTSIDE,
        PLAYER_BASE,
        OPPONENT_BASE
    }

    public enum PlayerUnitTarget
    {
        ENEMY = 0,
        OWN,
        ANY
    }

    public enum UnitTargetGroup
    {
        SINGLE = 0,
        BORDERING,
        SURROUNDING,
        IN_FRONT,
        BEHIND,
        SIDEWAYS,
        ALL
    }

    public int[] neighbourId = new int[8];

    ////////////////////////
    /// Variables for UI ///
    ////////////////////////

    public bool showUnitsPower = false;
    //a contianer for unit ifno with text and images
    public GameObject UnitDetailsPanel;
    //a text for a playr on enemy turn
    public TMP_Text playerIndicatorText;
    //colors of player and enemy
    public Color playerColor;
    public Color opponentColor;
    //bool if menus area on
    public bool areMenus = false;

    ////////////////////////////////////////
    /// Variables for cards and spawning ///
    ////////////////////////////////////////

    private List<CardJson> cardsJson;
    public CardManager cardManager;

    public UnitSpawn unitSpawn;

    //////////////////////
    /// Initialization ///
    //////////////////////

    void Start()
    {
        //setting db Json
        ReadJson("Assets/CardDataBase/cardDB.json");

        setPlanetPosition();

        //setting details canvas
        Vector3 startPosition = transform.position; // starting position of the grid
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                // initialize tile position
                Vector3 tilePosition = new Vector3(startPosition.x + col, startPosition.y, startPosition.z + row);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);

                MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
                tileList.Add(new Tile(renderer.material.color, tile, renderer, row + col * rows));

                ClickEvent clickEvent = tile.AddComponent<ClickEvent>();
                //clickEvent.OnClick += ChangeTileColor;
                clickEvent.OnClick += ShowInfoAboutGameObject;

                OnMouseEventTile onDropEvent = tile.AddComponent<OnMouseEventTile>();
            }
        }

        neighbourId[(int)Direction.UP] = -rows;
        neighbourId[(int)Direction.DOWN] = rows;
        neighbourId[(int)Direction.LEFT] = -1;
        neighbourId[(int)Direction.RIGHT] = 1;
        neighbourId[(int)Direction.UL] = neighbourId[(int)Direction.UP] + neighbourId[(int)Direction.LEFT];
        neighbourId[(int)Direction.UR] = neighbourId[(int)Direction.UP] + neighbourId[(int)Direction.RIGHT];
        neighbourId[(int)Direction.DL] = neighbourId[(int)Direction.DOWN] + neighbourId[(int)Direction.LEFT];
        neighbourId[(int)Direction.DR] = neighbourId[(int)Direction.DOWN] + neighbourId[(int)Direction.RIGHT];

        Base[] bases = FindObjectsOfType<Base>();
        playerBase = bases[0];
        opponentBase = bases[1];
        Debug.Log(bases[0]);
        Debug.Log(bases[1]);
        turn_button = FindObjectOfType<TurnButton>();
    }


    ////////////////////////////////
    /// Functions for Background ///
    ////////////////////////////////

    private void setPlanetPosition()
    {
        float planetRadious = gameObject.transform.GetChild(0).localScale.x / 2;
        Vector3 planetPosition = new Vector3(transform.position.x + (columns - 1) / 2f, transform.position.y - planetRadious - .5f, transform.position.z + (rows - 1) / 2f);
        gameObject.transform.GetChild(0).position = planetPosition;
    }

    //////////////////////////////////////////////////////
    /// Functions for Tile selection and Tile checking ///
    //////////////////////////////////////////////////////

    //return a list of all tile objects
    public List<Tile> getTileList()
    {
        return tileList;
    }

    // returns tiles, but doesn't detect going out of boarders - it goes around the arena
    public Tile GetTile(int id, Direction direction)
    {
        if (id + neighbourId[(int)direction] < tileList.Count && id + neighbourId[(int)direction] >= 0)
        {
            return tileList[id + neighbourId[(int)direction]];
        }
        return null;
    }

    // returns true if is at or behind frontlines during arena.playerTurn 
    public bool IsBehindFrontline(Tile tile)
    {
        int row = tileList.IndexOf(tile) / rows;
        // is near players base
        if (row == (playerTurn ? 4 : 0))
        {
            return true;
        }
        // is near opponents base
        if (row == (playerTurn ? 0 : 4))
        {
            return false;
        }

        int start   = playerTurn ? 0 : (tileList.Count - 1),
            end     = playerTurn ? tileList.Count : -1,
            inc     = playerTurn ? 1 : -1;
        // find most forward players unit
        for (int i = start; i != end; i += inc)
        {
            if (tileList[i].character != null && tileList[i].character.playerUnit == playerTurn)
            {
                return ((i / rows > row) ^ playerTurn) || (i / rows == row);
            }
        }
        // not in front of base and no other units
        return false;
    }

    ////////////////////////////////////
    /// Functions for Turn mechanics ///
    ////////////////////////////////////
    
    public void EndTurn()
    {

        if (playerTurn)
            playerBase.UpdateEnergy(energyFlow);
            else
            opponentBase.UpdateEnergy(energyFlow);

        playerTurn = !playerTurn;
        int begin, end, increment;
        if (playerTurn)
        {
            playerIndicatorText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">Your Turn</color>";
            begin = 0;
            end = tileList.Count;
            increment = 1;

        }
        else
        {
            playerIndicatorText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(opponentColor) + ">Enemy Turn</color>";
            begin = tileList.Count - 1;
            end = -1;
            increment = -1;
        }

        for (int i = begin; i != end; i += increment)
        {
            Tile tile = tileList[i];
            if (tile.character != null && tile.character.playerUnit == playerTurn)
            {
                tile.character.Move(playerTurn ? Direction.UP : Direction.DOWN);
            }
        }
        turn_button.timer_started = true;

    }

    //////////////////////////////
    /// Functions for damaging ///
    //////////////////////////////

    // detects exacly where unit comes out after move in given direction (even on sides)
    public OutOfBoarder GetTargetInfo(int id, Direction direction)
    {
        // goes out of bounds (left or right)
        if ((id % columns == 0 && (direction == Direction.DL || direction == Direction.LEFT || direction == Direction.UL)) ||
           (id % columns == columns - 1 && (direction == Direction.DR || direction == Direction.RIGHT || direction == Direction.UR)))
            return OutOfBoarder.OUTSIDE;
        // which base
        if (id < columns && (direction == Direction.UL || direction == Direction.UP || direction == Direction.UR))
        {
            return OutOfBoarder.OPPONENT_BASE;
        }
        if (id >= columns * (rows - 1) && (direction == Direction.DL || direction == Direction.DOWN || direction == Direction.DR))
        {
            return OutOfBoarder.PLAYER_BASE;
        }
        return OutOfBoarder.INSIDE;

    }

    public List<Character> GetTargets(PlayerUnitTarget put, UnitTargetGroup utg, Tile originTile, bool playerSide)
    {
        List<Character> characters = new List<Character>();
        List<Tile> areaTiles = new List<Tile>();

        // get corresponding tiles

        if (utg == UnitTargetGroup.SINGLE)
        {
            areaTiles.Add(originTile);
        }
        else if (utg == UnitTargetGroup.IN_FRONT || utg == UnitTargetGroup.BEHIND)
        {
            Direction moveDirection = (playerTurn ^ utg == UnitTargetGroup.BEHIND) ? Direction.UP : Direction.DOWN;
            Tile temp = originTile;
            while (GetTargetInfo(temp.id, moveDirection) == OutOfBoarder.INSIDE)
            {
                temp = GetTile(temp.id, moveDirection);
                areaTiles.Add(temp);
            }
        }
        else if (utg == UnitTargetGroup.BORDERING || utg == UnitTargetGroup.SURROUNDING || utg == UnitTargetGroup.SIDEWAYS)
        {
            Direction[] directions = new Direction[0];
            switch (utg)
            {
                case UnitTargetGroup.BORDERING:
                    directions = new Direction[4] { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT };
                    break;
                case UnitTargetGroup.SURROUNDING:
                    directions = new Direction[8] { Direction.UP, Direction.UR, Direction.RIGHT, Direction.DR, Direction.DOWN, Direction.DL, Direction.LEFT, Direction.UL };
                    break;
                case UnitTargetGroup.SIDEWAYS:
                    directions = new Direction[2] { Direction.RIGHT, Direction.LEFT };
                    break;
                default:
                    Debug.Log("unknown UnitTargetGroup" + utg);
                    break;
            }
            foreach (Direction direction in directions)
            {
                if (GetTargetInfo(originTile.id, direction) == OutOfBoarder.INSIDE)
                {
                    areaTiles.Add(GetTile(originTile.id, direction));
                }
            }
        }
        else
        {
            areaTiles = getTileList();
        }

        // check if there are units, and if they belong to correct player

        if (put == PlayerUnitTarget.ANY)
        {
            foreach (Tile tile in areaTiles)
            {
                if (tile.character != null && !tile.character.HasDied())
                {
                    characters.Add(tile.character);
                }
            }
        }
        else // player target own or enemy
        {
            bool side = playerSide ^ put == PlayerUnitTarget.ENEMY;
            foreach (Tile tile in areaTiles)
            {
                if (tile.character != null && tile.character.playerUnit == side && !tile.character.HasDied())
                {
                    characters.Add(tile.character);
                }
            }
        }

        return characters;
    }

    public void Damage(PlayerUnitTarget put, UnitTargetGroup utg, Tile originTile, bool playerSide, int damage)
    {
        List<Character> characters = GetTargets(put, utg, originTile, playerSide);

        foreach (Character character in characters)
        {
            character.TakeDamage(damage);
        }
    }

    public void AddToDyingQueue(Character c)
    {
        if (dyingUnits.Count == 0 && !deathrattleWave)
        {
            deathrattleWave = true;
            c.ActivateDeathRattle();
            return;
        }
        dyingUnits.Enqueue(c);
    }

    public void NextDying()
    {
        if (dyingUnits.Count == 0)
        {
            deathrattleWave = false;
            return;
        }
        ((Character)dyingUnits.Dequeue()).ActivateDeathRattle();
    }

    ////////////////////////
    /// Functions for UI ///
    ////////////////////////
    
    //Shows information about unit on board in certain tile
    private void ShowInfoAboutGameObject(GameObject gameObject)
    {
        if (gameObject)
        {
            Tile tile = tileList.Find(obj => obj.gameObject == gameObject);
            if (tile != null && tile.character != null && UnitDetailsPanel && !areMenus)
            {
                ShowDetails(tile);
                EnableMenu();
            }
        }

    }

    //Displays on ui info about units attack or hide
    public void ShowAttackInfo(bool isShowing)
    {
        showUnitsPower = isShowing;
        foreach (Tile tile in tileList)
        {
            if (tile.character != null && isShowing)
            {
                tile.character.DisplayAttackInfo();
            }
            else if (tile.character != null && !isShowing)
            {
                tile.character.HideAttackInfo();
            }
        }
    }

    //showing appropiate image in details container
    public void ShowDetails(Tile tile)
    {
        //change canvas status to appear
        UnitDetailsPanel.SetActive(true);
        //changing image to appropiate
        int index = tile.character.getIndexOfCard();
        Image image = UnitDetailsPanel.transform.Find("UnitDetailsContainer").transform.Find("UnitDetailsImage").GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>(cardsJson[index].cardImage);
    }

    public void EnableMenu()
    {
        areMenus = true;
    }

    public void DisableMenu()
    {
        areMenus = false;
    }

    ///////////////////////////
    /// Functions for cards ///
    ///////////////////////////

    //read json db
    private void ReadJson(string path)
    {
        using StreamReader reader = new(path);
        var jsonDB = reader.ReadToEnd();
        cardsJson = JsonConvert.DeserializeObject<List<CardJson>>(jsonDB);

        //initalize card
        cardManager.InitalizeHand();

    }

    public List<CardJson> getJsonCards()
    {
        return cardsJson;
    }
}