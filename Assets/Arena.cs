using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arena : MonoBehaviour
{
    public GameObject tilePrefab; // assign the tile prefab in the Inspector
    public GameObject groundPrefab;
    public float groundOffset = 0.1f;
    public int rows = 4;
    public int columns = 5;

    //red color to change the color of the tile
    private Color redColor = new Color(255, 0, 0);
   
    //Currently clicked tile
    private List<Tile> tileList = new List<Tile>();

    //canvas 
    public Text unitInfoText;

    void Start()
    {
        Vector3 startPosition = transform.position; // starting position of the grid

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 tilePosition = new Vector3(startPosition.x + col, startPosition.y, startPosition.z + row);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);

                MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
                tileList.Add(new Tile(renderer.material.color, tile, renderer));

                if (startPosition.x + col ==0 || startPosition.x + col == 4)
                {
                    ClickEvent clickEvent = tile.AddComponent<ClickEvent>();
                    clickEvent.OnClick += ChangeTileColor;
                    clickEvent.OnClick += ShowInfoAboutGameObject;
                }
            }
        }

        Vector3 groundPosition = new Vector3(startPosition.x + ((float)columns +1)/2-1, startPosition.y - groundOffset, startPosition.z+((float)rows +1)/2-1);
        groundPrefab.transform.localScale = new Vector3(columns + 1, 2 * groundOffset, rows + 1);
        Instantiate(groundPrefab, groundPosition, Quaternion.identity, transform);

    }
    private void ShowInfoAboutGameObject(GameObject gameObject)
    {
        unitInfoText.text = "";
        if (gameObject)
        {
            Tile tile = tileList.Find(obj => obj.gameObject == gameObject);
            if (tile!=null && tile.character!=null)
            {
                unitInfoText.text = "ABOUT UNIT: \n" +tile.character.toString();
            }

        } 
    }

    //changes color of clicked tile to red to spawn a unit
    private void ChangeTileColor(GameObject gameObject)
    {
        
        if (gameObject)
        {
            //find a tile in list that is a clicked object
            Tile tile = tileList.Find(obj => obj.gameObject == gameObject);
            if(tile != null)
            {
                //change color of old tile
                Tile oldTile = tileList.Find(obj => obj.mesh.material.color != obj.mainColor);
                if (oldTile != null)
                {
                    oldTile.mesh.material.color = oldTile.mainColor;
                    oldTile.isClicked = false;
                }
                //change color of clicked tile
                tile.mesh.material.color = redColor;
                tile.isClicked = true;
            }
            else
            {
                Debug.LogError("Could not find the tile");
            }
        }
        else
        {
            Debug.LogError("No game object");
        }
    }
    //return a list of all tile objects
    public List<Tile> getTileList()
    {
        return tileList;
    }

    

    }
