using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideCamera : MonoBehaviour
{
    public Vector3 start_position = new Vector3(6,1,0);
    public Vector3 end_position = new Vector3(0, 7, 0);
    public float close_offset = 0.5f;
    public Vector3 look_at = Vector3.zero;
    public int speed = 10;

    private void Start()
    {
        transform.position = start_position;
    }

    void Update()
    {
        if (!IsClose(transform.position, end_position))
        {
            transform.position = Vector3.Lerp(transform.position, end_position, Time.deltaTime * speed / 20);
            transform.LookAt(look_at);
        }
        else
        {
            transform.position = end_position;
            transform.LookAt(look_at);
            Destroy(GetComponent<SlideCamera>());
        }
    }

    private bool IsClose(Vector3 v1, Vector3 v2) 
    {
        return Vector3.Distance(v1,v2) < close_offset;
    }
}
