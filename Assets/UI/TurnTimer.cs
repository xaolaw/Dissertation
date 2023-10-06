using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimer : MonoBehaviour
{

    public Image player_bar;
    public Image opponent_bar;
    //public bool player_turn = true;

    
    public void set_time(float time, bool player_turn){
        if (0 > time || 1 < time)
            print("time to set in TurnTimer must be between 0 <= t =< 1");
        if (player_turn)
            player_bar.fillAmount = time;
        else
            opponent_bar.fillAmount = time;
    }
}
