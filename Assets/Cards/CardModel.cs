using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using System.Drawing.Image;
//using System.Drawin.g.dll;

public class CardModel : MonoBehaviour
{
    public Vector3 position;
    public Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = scale;
        transform.position = position;
        var card = new Card("first card", "some_description",
                                        new Stats(10, 20, 30));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class Stats
    {   
        public int _attack;
        public int _defence;
        public int _max_hp;
        public Stats(int attack,
                       int defence,
                       int max_hp)
        {
            _attack = attack;
            _defence = defence;
            _max_hp = max_hp;
        }

    }

    public class Card
    {
        public string _card_name;
        public string _description;
        public Stats _stats;
        public int _hp;
     //   Image card_image = Image.FromFile("geralt-igni.jpg");


        public Card(string card_name,
                    string description,
                    Stats stats
                    )
        {
            _card_name = card_name;
            _description = description;
            _stats = stats;
            _hp = stats._max_hp;

        }
    }
}
