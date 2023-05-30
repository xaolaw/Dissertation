using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Character
{
    string name;
    int health;
    int attack;
    public bool playerUnit; // true - player unit / false - enemy unit
    GameObject gameObject;
    GameObject canvasInfo;
    Tile tile;
    public Character(string name_, int health_, int attack_, bool _playerUnit, GameObject gameObject_, Tile _tile, GameObject _canvasInfo)
    {
        this.attack = attack_;
        this.health = health_;
        this.name = name_;
        this.playerUnit = _playerUnit;
        this.gameObject = gameObject_;
        this.tile = _tile;
        this.canvasInfo = _canvasInfo;
    }

    public void Move(Arena.Direction direction)
    {
        Tile temp = tile.GetTile(direction);
        if (temp != null && temp.character == null)
        {
            tile.UnitMoved();
            tile = temp;
            temp.character = this;
            gameObject.transform.position = tile.unitPosition;

            Vector3 canvas_position = Camera.main.WorldToScreenPoint(tile.gameObject.gameObject.transform.position);
            canvasInfo.transform.position = canvas_position + new Vector3(0, 20, 0);
        }
    }

    public string toString() {
        return "Name: " + name + "\nAttack: " + attack;
    }
}