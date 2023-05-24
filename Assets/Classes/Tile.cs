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

    public Tile(Color mainColor_, GameObject gameObject_, MeshRenderer mesh_)
    {
        this.gameObject = gameObject_;
        this.mainColor = mainColor_;
        this.mesh = mesh_;
        isClicked = false;
    }
    //add character to a tile
    public void addCharacter(Character character_)
    {
        this.character = character_;
    }
}

