using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideCamera : MonoBehaviour
{
    public Arena arena;
    public Camera main_camera;
    public Image black_out_image;

    public Canvas start_canvas;
    public Canvas main_canvas;

    float delta_time = 0f;

    int speed = 4;
    float show_time = 10.5f; //s

    float start_fov = 180; //max value btw
    float end_fov; // should be < then start_fov
    float fov_time = 3f;
    float fov_delay_time = 1f;

    float black_out_time = 3f;//at the end

    Vector3 start_position;
    Vector3 end_position;
    Vector3 look_at;


    private void Start()
    {
        start_position = arena.ArenaCenterPoint() + new Vector3(-10, 0, 0);
        end_position = arena.ArenaCenterPoint() + new Vector3(0, 5, 0);
        look_at = arena.ArenaCenterPoint();
        end_fov = this.GetComponent<Camera>().fieldOfView;
        this.GetComponent<Camera>().fieldOfView = start_fov;

        GetComponent<Camera>().enabled = true;
        main_camera.enabled = false;

        start_canvas.enabled = true;
        main_canvas.enabled = false;

        transform.position = start_position;
    }

    void Update()
    {
        delta_time += Time.deltaTime;

        if (ShowTimeLeft() < 0)
        {
            main_camera.enabled = true;

            start_canvas.enabled = false;
            main_canvas.enabled = true;

            Destroy(this.gameObject);
            return;
        }


        if (delta_time- fov_delay_time>0  && delta_time - fov_delay_time <= fov_time)
        {
            float multiplaier = Mathf.Pow((delta_time-fov_delay_time) / fov_time,2);
            this.GetComponent<Camera>().fieldOfView = start_fov + ( multiplaier * (end_fov-start_fov));
        }


        if (ShowTimeLeft() < black_out_time) {
            black_out_image.color = new Color(black_out_image.color.r, black_out_image.color.g, black_out_image.color.b, 1- Mathf.Pow((ShowTimeLeft()) / black_out_time,2));
        }
        

        //przyblizanie
        transform.position = Vector3.Lerp(transform.position, end_position, Time.deltaTime * speed / 20);
        //obracanie
        float curr_distance = Vector3.Distance(transform.position, end_position);

        transform.position += transform.right * 0.5f * curr_distance * Time.deltaTime;


        //patrzenie
        transform.LookAt(look_at);
       
    }

    private float ShowTimeLeft() { 
    return show_time-delta_time;
    }

}
