using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class Tile
{
    public Color mainColor;
    public GameObject gameObject;
    public MeshRenderer mesh;
    public bool isClicked;
    public Character character;
    public int id;
    public Vector3 unitPosition;

    private Arena arena;

    public Tile(Color mainColor_, GameObject gameObject_, MeshRenderer mesh_, int _id)
    {
        this.gameObject = gameObject_;
        this.mainColor = mainColor_;
        this.mesh = mesh_;
        this.id = _id;
        isClicked = false;

        arena = (Arena)GameObject.FindObjectOfType(typeof(Arena));
        if (!arena)
            Debug.Log("TurnButton: Arena not found");

        unitPosition = gameObject.transform.position + new Vector3(0.0f, 0.0f, 0.1f);
    }
    //add character to a tile
    public void addCharacter(Character character_)
    {
        this.character = character_;
    }

    public Tile GetTile(Arena.Direction direction)
    {
        return arena.GetTile(id, direction);
    }

    public void UnitMoved()
    {
        character = null;
    }
}

