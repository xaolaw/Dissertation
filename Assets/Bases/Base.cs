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

    public HealthBar healthBar;
    public Color baseColor;

    // Start is called before the first frame update
    void Start()
    {
        basePosition = playerBase ? new Vector3(3.50f, 0, 1) : new Vector3(.5f, 0, 2f);

        healthBar.SetMaxHealth(maxHp);
        healthBar.SetHealth(hp);

        
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
}
