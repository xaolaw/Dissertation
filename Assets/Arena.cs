using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Classes;
using Newtonsoft.Json;
using System.IO;
using Unity.Netcode;

public class Arena : MonoBehaviour
{

    ////////////////////////////////////////////////
    /// Variables for ground and tiles and bases ///
    ////////////////////////////////////////////////
    
    public GameObject tilePrefab; // assign the tile prefab in the Inspector
    //Currently clicked tile
    private List<Tile> tileList = new List<Tile>();
    
    public float groundOffset = 0.1f;
    public const int rows = 5;
    public const int columns = 4;
    public Vector3 ArenaCenterPoint()
    {
        return new Vector3(transform.position.x + (columns - 1) / 2f, transform.position.y, transform.position.z + (rows - 1) / 2f);
    }

    private int playerFrontline = 4;
    private int enemyFrontline = 0;

    public Base playerBase;
    public Base opponentBase;

    /////////////////////////////////////////////////
    /// Variables for turn mechanics and damaging ///
    /////////////////////////////////////////////////

    private TurnButton turn_button;
    private float TURN_TIME = 20.0f;
    private float time_left = 20.0f;
    public bool timer_started = false;
    public TurnTimer turn_timer;

    // variable of moving entity - only one can move at the same time
    private Character movingCharacter = null;
    private Vector3 movingStartPos;
    private Vector3 movingEndPos;

    // constant time it takes one unit to move one tile
    private float TIME_OF_ONE_MOVE;

    private int endTurnTileID;
    private int endTurnTileIDEnd;
    private int endTurnTileIDIncrement;
    private int endTurnTileIDrecent;
    private Character.MovingReason movingReason;

    private float moveTime = 0.8f;

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

    public enum SpellTarget
    {
        NONE = 0,
        UNIT,
    }

    public enum PlayerUnitTarget
    {
        ENEMY = 0,
        OWN,
        ANY
    }

    public enum UnitTargetGroup     //  Legend for JSON                                             [meaning]
    {
        SINGLE = 0,                 // single or self                                               [on place of unit]
        BORDERING,                  // bordering                                                    [units orthogonally connected (no corners)]
        SURROUNDING,                // surrounding                                                  [units orthogonally and diagonally connected (bordering with corners)]
        IN_FRONT,                   // in_front                                                     [all units in front]
        BEHIND,                     // behind                                                       [all units behind]
        SIDEWAYS,                   // sideways                                                     [units directly to left or right (that are bordering)]
        ALL,                        // all (or anything that doesn't match the rest - it is default)[all units]
        SINGLE_BEHIND,              // single_behind                                                [one unit directly behind]
        SINGLE_IN_FRONT             // single_in_front                                              [one unit directly in front]
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

    public LineRenderer playerFrontlineLine;
    public LineRenderer enemyFrontlineLine;

    ////////////////////////////////////////
    /// Variables for cards and spawning ///
    ////////////////////////////////////////

    private List<CardJson> cardsJson;
    public CardManager cardManager;

    public UnitSpawn unitSpawn;

    ////////////////////////////////////////
    /// Variables for cards and spawning ///
    ////////////////////////////////////////

    public ArenaNetworkManager manager;

    ////////////////////////////////////////
    /// Variables for end script ///
    ////////////////////////////////////////

    public EndScript endScript;
    public Canvas mainCanvas;

    public bool hasEnded = false;

    ////////////////////////////////////////
    /// Variables for start ///
    ///////////////////////////////////////
    
    public bool hasStarted = false; //slideCamera changes it (started animation)

    //////////////////////
    /// Initialization ///
    //////////////////////

    void Start()
    {
        //setting db Json
        ReadJson("Assets/CardDataBase/cardDB.json");

        SetPlanetPosition();

        //setting details canvas
        Vector3 startPosition = transform.position; // starting position of the grid

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // initialize tile position
                Vector3 tilePosition = new Vector3(startPosition.x + row, startPosition.y, startPosition.z + col);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                tile.name = "tile [" + (col + row * columns) + "]";

                MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
                tileList.Add(new Tile(renderer.material.color, tile, renderer, col + row * columns));

                ClickEvent clickEvent = tile.AddComponent<ClickEvent>();
                //clickEvent.OnClick += ChangeTileColor;
                clickEvent.OnClick += ShowInfoAboutGameObject;

                OnMouseEventTile onDropEvent = tile.AddComponent<OnMouseEventTile>();
            }
        }

        neighbourId[(int)Direction.UP] = -columns;
        neighbourId[(int)Direction.DOWN] = columns;
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

        turn_timer.setBarActive(playerTurn);
        turn_timer.set_time(1.0f, playerTurn);

        // host sets random starting player and if there is no host, then (offline test) it is as before
        if (manager.IsServer)
        {
            playerTurn = manager.SetStart(Random.Range(0, 2));
            turn_timer.setBarActive(playerTurn);
            turn_timer.set_time(TURN_TIME, playerTurn);
            UpdateTurnIndicator();
        }
    }

    // update arena and its components

    private void Update()
    {
        if (!hasEnded) {
            UpdateTimer();
            if (movingCharacter != null)
                UpdateMovingCharacter();
        }
       
    }


    ////////////////////////////////
    /// Functions for Background ///
    ////////////////////////////////

    private void SetPlanetPosition()
    {
        float planetRadious = gameObject.transform.GetChild(0).localScale.x / 2;
        Vector3 planetPosition = new Vector3(transform.position.x + (columns - 1) / 2f, transform.position.y - planetRadious - .5f, transform.position.z + (rows - 1) / 2f);
        gameObject.transform.GetChild(0).position = planetPosition;
    }

    private void UpdateFrontlineGraphics(bool player)
    {
        if (player)
        {
            playerFrontlineLine.SetPosition(0, new Vector3((float)playerFrontline - 0.48f, 0.1f, -1));
            playerFrontlineLine.SetPosition(1, new Vector3((float)playerFrontline - 0.48f, 0.1f, 4));
        }
        else
        {
            enemyFrontlineLine.SetPosition(0, new Vector3((float)enemyFrontline + 0.48f, 0.1f, -1));
            enemyFrontlineLine.SetPosition(1, new Vector3((float)enemyFrontline + 0.48f, 0.1f, 4));
        }
    }

    //////////////////////////////////////////////////////
    /// Functions for Tile selection and Tile checking ///
    //////////////////////////////////////////////////////

    //return a list of all tile objects
    public List<Tile> GetTileList()
    {
        return tileList;
    }

    public Tile GetSingleTile(int tileID, bool reversed = false)
    {
        return tileList[reversed?(tileList.Count - tileID - 1):tileID];
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
    public bool IsBehindFrontline(Tile tile, bool player)
    {
        int row = tileList.IndexOf(tile) / columns;

        return player ? row >= playerFrontline : row <= enemyFrontline;
    }

    private void UpdateFrontline(bool player)
    {
        bool foundUnit = false;

        int start = player ? 0 : (tileList.Count - 1),
            end = player ? tileList.Count : -1,
            inc = player ? 1 : -1;
        // find most forward players unit
        for (int i = start; i != end; i += inc)
        {
            if (tileList[i].character != null && tileList[i].character.playerUnit == player)
            {
                if (player)
                    playerFrontline = i / columns;
                else
                    enemyFrontline = i / columns;
                foundUnit = true;
                break;
            }
        }

        if (!foundUnit)
        {
            if (player)
                playerFrontline = 4;
            else
                enemyFrontline = 0;
        }

        if (player && playerFrontline == 0)
            playerFrontline = 1;
        else if (!player && enemyFrontline == 4)
            enemyFrontline = 3;

        UpdateFrontlineGraphics(player);
    }

    public void CheckFrontline(int tileID, bool player)
    {
        int row = tileID / columns;
        if (player)
        {
            row = row == 0 ? 1 : row;
            if (row < playerFrontline)
            {
                playerFrontline = row;
                UpdateFrontline(player);
            }
        }
        else
        {
            row = row == 4 ? 3 : row;
            if (row > enemyFrontline)
            {
                enemyFrontline = row;
                UpdateFrontline(player);
            }
        }
    }
    public int GetPlayerFrontline(bool player)
    {
        return player ? playerFrontline : enemyFrontline;
    }

    ////////////////////////////////////
    /// Functions for Turn mechanics ///
    ////////////////////////////////////

    private void UpdateTimer()
    {
        if (timer_started)
        {
            if (time_left > 0)
            {
                time_left = (time_left > Time.deltaTime) ? time_left - Time.deltaTime : 0;
                turn_timer.set_time(time_left / TURN_TIME, playerTurn);
            }
            else
            {
                timer_started = false;
                // we trust other player's timer
                if (playerTurn || !NetworkManager.Singleton.IsClient)
                    EndTurn();
            }
        }
    }

    private void UpdateMovingCharacter()
    {
        TIME_OF_ONE_MOVE += Time.deltaTime;
        movingCharacter.SetPosition(Vector3.Lerp(movingStartPos, movingEndPos, Mathf.Min(TIME_OF_ONE_MOVE / moveTime, 1.0f)));
        if (TIME_OF_ONE_MOVE >= moveTime)
        {
            movingCharacter.EndWalking();
            movingCharacter = null;
            switch (movingReason)
            {
                case Character.MovingReason.END_TURN:
                    CheckEndTurnTile();
                    break;
                case Character.MovingReason.SPAWN:
                    break;
                default:
                    Debug.LogError("Moved without reason");
                    break;
            }
        }
    }

    public void Moving(Character unit, Vector3 start, Vector3 end, Character.MovingReason reason)
    {
        movingCharacter = unit;
        movingStartPos = start;
        movingEndPos = end;
        TIME_OF_ONE_MOVE = 0.0f;
        movingReason = reason;
    }

    public void CheckEndTurnTile()
    {
        if (endTurnTileID + endTurnTileIDIncrement == endTurnTileIDEnd)
            return;

        if (tileList[endTurnTileID].character == null || tileList[endTurnTileID].character.playerUnit != playerTurn || !tileList[endTurnTileID].character.IsMovingType())
        {
            endTurnTileID += endTurnTileIDIncrement;
            CheckEndTurnTile();
            return;
        }

        Character unit = tileList[endTurnTileID].character;

        // if we don't check the same unit twice
        if (endTurnTileID != endTurnTileIDrecent)
        {

            if (unit.HasStatus(Character.UnitStatus.EMPOWERED))
            {
                unit.GivePower(1);
            }

            // if survived moving
            if (unit.Move(Character.MovingReason.END_TURN))
            {
                unit.StartWalking();
                endTurnTileIDrecent = unit.GetTileID();
            }
        }
        endTurnTileID += endTurnTileIDIncrement;
    }

    public void EndTurn()
    {
        // signal other player
        manager.SendSignal(ArenaNetworkManager.GameSignal.EndTurn);

        // call end turn for only your arena
        _EndTurn();
    }

    // Can only be called to end turn for one player
    public void _EndTurn()
    {
        timer_started = false;
        turn_timer.set_time(0, playerTurn);
        turn_timer.setBarActive(!playerTurn);
        time_left = TURN_TIME;

        endTurnTileIDrecent = -1;

        if (!NetworkManager.Singleton.IsClient || playerTurn)
        {
            cardManager.DrawCard();
        }
        cardManager.RestoreCardsColor(playerBase.GetEnergy());
        cardManager.RestoreCardsColor(opponentBase.GetEnergy());
        

        if (playerTurn){
            playerBase.UpdateEnergy(energyFlow);
            }
        else {
            opponentBase.UpdateEnergy(energyFlow);
            }

        playerTurn = !playerTurn;
        //int begin, end, increment;
        if (playerTurn)
        {
            playerIndicatorText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">Your Turn</color>";
            endTurnTileID = 0;
            endTurnTileIDEnd = tileList.Count;
            endTurnTileIDIncrement = 1;

        }
        else
        {
            playerIndicatorText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(opponentColor) + ">Enemy Turn</color>";
            endTurnTileID = tileList.Count - 1;
            endTurnTileIDEnd = -1;
            endTurnTileIDIncrement = -1;
        }

        for (int i = endTurnTileID; i != endTurnTileIDEnd; i += endTurnTileIDIncrement)
        {
            if (tileList[i].character != null && tileList[i].character.playerUnit != playerTurn)
            {
                Character unit = tileList[i].character;
                if (unit.hasOnEndTurn)
                {
                    unit.ActivateOnEndTurn();
                }
            }
        }

        CheckEndTurnTile();

        UpdateFrontline(!playerTurn);

        timer_started = true;

       
    }

    public void End(bool youWin) {
 
        mainCanvas.enabled = false;

       

        endScript.ShowEndCanvas(youWin);

        hasEnded = true;

        //TODO - sync and end connection between players

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

    public List<Tile> GetTargetTiles(UnitTargetGroup utg, Tile originTile, bool playerSide)
    {
        List<Tile> areaTiles = new List<Tile>();

        // get corresponding tiles

        if (utg == UnitTargetGroup.SINGLE)
        {
            areaTiles.Add(originTile);
        }
        else if (utg == UnitTargetGroup.SINGLE_BEHIND || utg == UnitTargetGroup.SINGLE_IN_FRONT)
        {
            Direction moveDirection = (utg == UnitTargetGroup.SINGLE_BEHIND ^ playerTurn) ? Direction.UP : Direction.DOWN;
            if (GetTargetInfo(originTile.id, moveDirection) == OutOfBoarder.INSIDE)
            {
                areaTiles.Add(GetTile(originTile.id, moveDirection));
            }
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
            areaTiles = GetTileList();
        }

        return areaTiles;
    }

    public List<Tile> GetEmptyTargetTiles(UnitTargetGroup utg, Tile originTile, bool playerSide)
    {
        List<Tile> wyn = new List<Tile>();
        foreach (Tile tile in GetTargetTiles(utg, originTile, playerSide))
        {
            if (tile.character == null)
            {
                wyn.Add(tile);
            }
        }

        return wyn;
    }

    public List<Character> GetTargets(PlayerUnitTarget put, UnitTargetGroup utg, Tile originTile, bool playerSide)
    {
        List<Character> characters = new List<Character>();
        List<Tile> areaTiles = GetTargetTiles(utg, originTile, playerSide);

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

        if (damage <= 0)
        {
            foreach (Character character in characters)
            {
                character.GivePower(-damage);
            }
        }
        else
        {
            foreach (Character character in characters)
            {
                // if unit died - update frontline
                if (character.TakeDamage(damage))
                    UpdateFrontline(character.playerUnit);
            }
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
    
    // Update turn indicator

    public void UpdateTurnIndicator()
    {
        playerIndicatorText.text = playerTurn?
            "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">Your Turn</color>":
            "<color=#" + ColorUtility.ToHtmlStringRGB(opponentColor) + ">Enemy Turn</color>";
    }


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

        reader.Close();
        //initalize card
        cardManager.InitalizeHand();

    }

    public List<CardJson> getJsonCards()
    {
        return cardsJson;
    }

    public void PlayCard(int cardID, int tileID)
    {
        manager.SendSignal(ArenaNetworkManager.GameSignal.PlayCard, cardID, tileID);
    }
}