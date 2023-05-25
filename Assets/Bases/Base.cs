using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public GameObject basePrefab; //base object assigned in Inspector

    public bool enemyBase;

    private Vector3 basePosition;
    public int hp = 10;
    public int maxHp = 10;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        basePosition = enemyBase ? new Vector3(-1.75f, 0, 1.5f) : new Vector3(5.75f, 0, 1.5f);

        healthBar.SetMaxHealth(maxHp);
        healthBar.SetHealth(hp);

        basePrefab.transform.localScale = new Vector3(1.5f, 0.4f, 5f);
        Instantiate(basePrefab, basePosition, Quaternion.identity);
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
