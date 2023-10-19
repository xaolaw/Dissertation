using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotation : MonoBehaviour
{
    
    float elapsed = 0f;

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= .1f)
        {
            elapsed = 0;
            Rotate();
        }
    }

    void Rotate()
    {
        transform.Rotate(Vector3.right, .1f);
    }


}
