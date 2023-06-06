using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideCamera : MonoBehaviour
{

    public Arena arena;
    float offset = 0.2f;
    public int speed = 10;


    Vector3 start_position;
    Vector3 end_position;
    Vector3 look_at;
    

    private void Start()
    {
        start_position = arena.GetBaseSpawnPoint(true) + new Vector3(-10, 0,0);
        end_position = arena.GetSpawnPoint() + new Vector3(0,7,0);
        look_at = arena.GetSpawnPoint();

        transform.position = start_position;
    }

    void Update()
    {
        if (!IsClose(transform.position, end_position))
        {
            //przyblizanie
            transform.position = Vector3.Lerp(transform.position, end_position, Time.deltaTime * speed / 20);
            //obracanie
            float curr_distance = Vector3.Distance(transform.position, end_position);

            transform.position += transform.right * 0.5f * curr_distance * Time.deltaTime;
            
            
            //patrzenie
            transform.LookAt(look_at);
        }
        else
        {
            transform.position = end_position;
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                -90,
                transform.eulerAngles.z
            );
            Destroy(GetComponent<SlideCamera>());
        }
    }

    private bool IsClose(Vector3 v1, Vector3 v2) 
    {
        return Vector3.Distance(v1,v2) < offset;
    }
}
