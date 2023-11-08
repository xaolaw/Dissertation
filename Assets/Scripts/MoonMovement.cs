using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonMovement : MonoBehaviour
{
    public float rotationTime = 360.0f;
    public float elipseX = 10.0f;    
    public float elipseY = 3.0f;
    public Vector2 elipseCenter= Vector2.zero;

    private float time = -120f;

    private void Update()
    {
        time += Time.deltaTime;

        float angle = (time / rotationTime) * 360.0f;
        float x = elipseX * Mathf.Cos(Mathf.Deg2Rad * angle);
        float y = elipseY * Mathf.Sin(Mathf.Deg2Rad * angle);

        Vector3 position = new Vector3(-x, y, transform.localPosition.z) + (Vector3)elipseCenter;
        transform.localPosition = position;
    }
}
