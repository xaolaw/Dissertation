using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public GameObject tilePrefab; // assign the tile prefab in the Inspector
    public GameObject groundPrefab;
    public float groundOffset = 0.1f;
    public int rows = 4;
    public int columns = 5;

    void Start()
    {
        Vector3 startPosition = transform.position; // starting position of the grid

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 tilePosition = new Vector3(startPosition.x + col, startPosition.y, startPosition.z + row);
                Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
            }
        }

        Vector3 groundPosition = new Vector3(startPosition.x + ((float)columns +1)/2-1, startPosition.y - groundOffset, startPosition.z+((float)rows +1)/2-1);
        groundPrefab.transform.localScale = new Vector3(columns + 1, 2 * groundOffset, rows + 1);
        Instantiate(groundPrefab, groundPosition, Quaternion.identity, transform);
    }

}
