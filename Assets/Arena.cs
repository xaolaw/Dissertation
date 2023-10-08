using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Arena : MonoBehaviour
{
    public GameObject tilePrefab; // assign the tile prefab in the Inspector
    public GameObject groundPrefab;
    public float groundOffset = 0.1f;
    public int rows = 5;
    public int columns = 4;
    public bool playerTurn = true;

    public bool showUnitsPower = false;
    public Base playerBase;
    public Base opponentBase;

    //Currently clicked tile
    private List<Tile> tileList = new List<Tile>();
    private TurnButton turn_button;

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
        BOARDERING,
        SURROUNDING,
        IN_FRONT,
        BEHIND,
        SIDEWAYS,
        ALL
    }

    public int[] neighbourId = new int[8];

    //a contianer for unit ifno with text and images
    public GameObject UnitDetailsPanel;
    //a text for a playr on enemy turn
    public TMP_Text playerIndicatorText;
    //colors of player and enemy
    public Color playerColor;
    public Color opponentColor;

    void Start()
    {
        //setting details canvas
        UnitDetailsPanel = GameObject.Find("UnitDetailsPanel");
        UnitDetailsPanel.SetActive(false);
      
        Vector3 startPosition = transform.position; // starting position of the grid
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
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

        Vector3 groundPosition = new Vector3(startPosition.x + ((float)columns + 1) / 2 - 1, startPosition.y - groundOffset, startPosition.z + ((float)rows + 1) / 2 - 1);
        groundPrefab.transform.localScale = new Vector3(columns + 1, 2 * groundOffset, rows + 1);
        Instantiate(groundPrefab, groundPosition, Quaternion.identity, transform);

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

    //Showing informationa about unit on board in certain tile
    private void ShowInfoAboutGameObject(GameObject gameObject)
    {
        if (gameObject)
        {
            Tile tile = tileList.Find(obj => obj.gameObject == gameObject);
            if (tile != null && tile.character != null && UnitDetailsPanel)
            {
                //change canvas status to appear
                UnitDetailsPanel.SetActive(true);
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
            else if(tile.character!=null && !isShowing)
            {
                tile.character.HideAttackInfo();
            }
        }
    }
   

    //return a list of all tile objects
    public List<Tile> getTileList()
    {
        return tileList;
    }

    public void EndTurn()
    {
        playerTurn = !playerTurn;
        int begin, end, increment;
        if (playerTurn)
        {
            begin = 0;
            end = tileList.Count;
            increment = 1;
            playerIndicatorText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">Your Turn</color>";
           
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

    // zwraca tile, nie wykrywa wychodzenia na boki z planszy - wyjdzie z przeciwnej strony
    public Tile GetTile(int id, Direction direction)
    {
        if (id + neighbourId[(int)direction] < tileList.Count && id + neighbourId[(int)direction] >= 0)
        {
            return tileList[id + neighbourId[(int)direction]];
        }
        return null;
    }

    // wykrywa gdzie dok�adnie znajdzie si� jednostka po poruszeniu
    public OutOfBoarder GetTargetInfo(int id, Direction direction)
    {
        // czy wychodzi lewo lub prawo
        if ((id % columns == 0 && (direction == Direction.DL || direction == Direction.LEFT || direction == Direction.UL)) ||
           (id % columns == columns - 1 && (direction == Direction.DR || direction == Direction.RIGHT || direction == Direction.UR)))
            return OutOfBoarder.OUTSIDE;
        // jaka baza
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

        if ( utg == UnitTargetGroup.SINGLE)
        {
            areaTiles.Add(originTile);
        }
        else if ( utg == UnitTargetGroup.IN_FRONT || utg == UnitTargetGroup.BEHIND)
        {
            Direction moveDirection = (playerTurn ^ utg == UnitTargetGroup.BEHIND) ? Direction.UP : Direction.DOWN;
            Tile temp = originTile;
            while(GetTargetInfo(temp.id, moveDirection) == OutOfBoarder.INSIDE)
            {
                temp = GetTile(temp.id, moveDirection);
                areaTiles.Add(temp);
            }
        }
        else if ( utg == UnitTargetGroup.BOARDERING || utg == UnitTargetGroup.SURROUNDING || utg == UnitTargetGroup.SIDEWAYS)
        {
            Direction[] directions = new Direction[0];
            switch (utg)
            {
                case UnitTargetGroup.BOARDERING:
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
                if(GetTargetInfo(originTile.id, direction) == OutOfBoarder.INSIDE)
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
            foreach(Tile tile in areaTiles)
            {
                if(tile.character != null)
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
                if (tile.character != null && tile.character.playerUnit == side)
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

        foreach(Character character in characters)
        {
            character.TakeDamage(damage);
        }
    }
}