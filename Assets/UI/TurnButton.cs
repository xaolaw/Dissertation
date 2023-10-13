using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    private Arena arena;
    private float turn_time = 10.0f;
    private float time_left = 10.0f;
    public bool timer_started = false;
    public TurnTimer turn_timer;

    void Start()
    {
        arena = FindObjectOfType<Arena>();
        if (!arena)
            Debug.Log("TurnButton: Arena not found");
        turn_timer.setBarActive(arena.playerTurn);    
        turn_timer.set_time(1.0f, arena.playerTurn);
    }

    private void Update()
    {   
        if(timer_started){
            if(time_left > 0){
                time_left -= Time.deltaTime;
                turn_timer.set_time(time_left/turn_time, arena.playerTurn);
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
        turn_timer.set_time(0, arena.playerTurn);
        turn_timer.setBarActive(!arena.playerTurn);
        reset_timer();
        arena.EndTurn();
    }
    
    public void reset_timer(){
        time_left = turn_time;
    }
}
