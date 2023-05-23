using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bases : MonoBehaviour
{
    //base object assigned in Inspector
    public GameObject basePrefab;

    //variables reserved for base position
    private Vector3 basePositionOne = new Vector3(-1.75f,0,1.5f);
    private Vector3 basePositionTwo = new Vector3(5.75f, 0, 1.5f);

    //scale
    private Vector3 scale = new Vector3(1.5f, 0.4f, 5f);

    // Start is called before the first frame update
    void Start()
    {
        // starting position of the base One
        transform.localScale = scale;
        Instantiate(basePrefab, basePositionOne, Quaternion.identity,transform);
        Instantiate(basePrefab, basePositionTwo, Quaternion.identity,transform);
        //test
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
