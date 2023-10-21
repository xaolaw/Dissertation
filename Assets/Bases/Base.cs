using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public GameObject basePrefab; //base object assigned in Inspector

    public bool playerBase;

    private Vector3 basePosition;
    public int hp = 10;
    public int maxHp = 10;
    public int energy = 10;
    // fixed enrgy values
    public int startEnergy = 10;
    public int maxEnergy = 20;

    public HealthBar healthBar;
    public EnergyBar energyBar;
    public Color baseColor;

    // Start is called before the first frame update
    void Start()
    {
        basePosition = playerBase ? new Vector3(3.50f, 0, 1) : new Vector3(.5f, 0, 2f);

        healthBar.SetMaxHealth(maxHp);
        healthBar.SetHealth(hp);

        print("hello");
        if (playerBase){
            print("player");
        } else{
            print("opponent");
        }
        energyBar.SetStartEnergy(startEnergy, maxEnergy);

        
        GameObject ob = Instantiate(basePrefab, basePosition, Quaternion.Euler(0, playerBase ? 90 : -90, 0));
         
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        healthBar.SetHealth(hp);
    }

    public bool TakeEnergy(int energy){
        if (this.energy >= energy){
                this.energy -= energy;
                energyBar.SetEnergy(this.energy);
                return true;
        } else
        return false;;
    }
    public bool TryTakeEnergy(int energy){
        return this.energy >= energy;
    }
    public void GiveEnergy(int energy){
        this.energy = Math.Min(this.energy + energy, maxEnergy);
        energyBar.SetEnergy(this.energy);
    }
}
