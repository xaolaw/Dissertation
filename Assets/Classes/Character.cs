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
    GameObject gameObject;
    public Character(string name_, int health_, int attack_,GameObject gameObject_)
    {
        this.attack = attack_;
        this.health = health_;
        this.name = name_;
        this.gameObject = gameObject_;
    }

    public string toString() {
        return "Name: " + name + "\nAttack: " + attack;
    }
}