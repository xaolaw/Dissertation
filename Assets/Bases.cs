using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bases : MonoBehaviour
{
    //base object assigned in Inspector
    public GameObject basePrefab;

    //variables reserved for base position
    public Vector3 basePositionOne;
    public Vector3 basePositionTwo;

    //scale
    public Vector3 scale;

    // Start is called before the first frame update
    void Start()
    {
        // starting position of the base One
        transform.localScale = scale;
        Instantiate(basePrefab, basePositionOne, Quaternion.identity,transform);
        Instantiate(basePrefab, basePositionTwo, Quaternion.identity,transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
