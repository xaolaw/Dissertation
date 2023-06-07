using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    private Button button;
    private Arena arena;
    private float turn_time = 10.0f;
    private float time_left = 10.0f;
    public bool timer_started = false;

    // Start is called before the first frame update
    void Start()
    {
        arena = FindObjectOfType<Arena>();
        if (!arena)
            Debug.Log("TurnButton: Arena not found");

        button = GetComponent<Button>();
        button.onClick.AddListener(EndTurn);
        timer_started = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(timer_started){

            if(time_left > 0){
                time_left -= Time.deltaTime;
            }
            else
            {
                timer_started = false;
                EndTurn();
            }
        }
        
    }

    public void EndTurn()
    {
        timer_started = false;
        reset_timer();
        arena.EndTurn();
    }
    public void reset_timer(){
        time_left = turn_time;
    }
}
