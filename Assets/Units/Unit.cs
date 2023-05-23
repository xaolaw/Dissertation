using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public GameObject unitPrefab;
    public Vector3 scale;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = scale;
        Vector3 unitPosition = new Vector3(0, 0.55f, 0);
        Instantiate(unitPrefab, unitPosition, Quaternion.identity,transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    class CharacterUnit
    {
        string name;
        int health;
        int attack;
        public CharacterUnit(string name_,int health_,int attack_)
        {
            this.attack = attack_;
            this.health = health_;
            this.name = name_;
        }

    }
}
