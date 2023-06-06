using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public GameObject basePrefab; //base object assigned in Inspector
    public Arena arena;

    public bool playerBase;

    private Vector3 basePosition;
    public int hp = 10;
    public int maxHp = 10;

    public HealthBar healthBar;
    public Color baseColor;

    // Start is called before the first frame update
    void Start()
    {

        basePosition = arena.GetBaseSpawnPoint(playerBase); //- new Vector3(transform.localScale.x,0,0);
        Vector3 offset = playerBase ? -new Vector3(transform.localScale.x, 0, 0) : new Vector3(transform.localScale.x, 0, 0);
        basePosition += offset;


        healthBar.SetMaxHealth(maxHp);
        healthBar.SetHealth(hp);

        basePrefab.transform.localScale = new Vector3(1.5f, 0.4f, 5f);
        GameObject ob = Instantiate(basePrefab, basePosition, Quaternion.identity);

        MeshRenderer mesh = ob.GetComponent<MeshRenderer>();
        mesh.material.color = baseColor;
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
