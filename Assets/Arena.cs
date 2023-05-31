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

    //Currently clicked tile
    private List<Tile> tileList = new List<Tile>();

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

    public int[] neighbourId = new int[8];

    //a contianer for unit ifno with text and images
    public GameObject unitInfoContainer;
    //a text for a playr on enemy turn
    public TMP_Text playerIndicatorText;
    //colors of player and enemy
    public Color playerColor;
    public Color opponentColor;

    void Start()
    {
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
    }

    //Showing informationa about unit on board in certain tile
    private void ShowInfoAboutGameObject(GameObject gameObject)
    {
        if (gameObject)
        {
            Tile tile = tileList.Find(obj => obj.gameObject == gameObject);
            Animator animator = unitInfoContainer.GetComponent<Animator>();
            if (tile != null && tile.character != null)
            {
                //asnimation to slide in TODO stop showing triigger elsewhere
                animator.ResetTrigger("stopShowing");
                animator.SetTrigger("isShowing");
                unitInfoContainer.GetComponentInChildren<TMP_Text>().text = "ABOUT UNIT: \n" + tile.character.toString();
            }
            else
            {
                animator.SetTrigger("stopShowing");

            }
        }

    }
    //Displays on ui info about units attack or hide
    public void ShowAttackInfo(bool isShowing)
    {
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
    }

    public Tile GetTile(int id, Direction direction)
    {
        if (id + neighbourId[(int)direction] < tileList.Count && id + neighbourId[(int)direction] >= 0)
        {
            return tileList[id + neighbourId[(int)direction]];
        }
        if (id < columns || id >= columns * (rows - 1))
        {
            Debug.Log("go into base?");
        }
        return null;
    }
}