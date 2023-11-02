using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    private Arena arena;
   

    void Start()
    {
        arena = FindObjectOfType<Arena>();
        if (!arena)
            Debug.Log("TurnButton: Arena not found");
    }

    public void EndTurn()
    {
        // you can only end your own turn
        if (arena.playerTurn)
            arena.EndTurn();
    }
}
