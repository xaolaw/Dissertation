using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{
    private Button button;
    private Arena arena;

    // Start is called before the first frame update
    void Start()
    {
        arena = (Arena)FindObjectOfType(typeof(Arena));
        if (!arena)
            Debug.Log("TurnButton: Arena not found");

        button = GetComponent<Button>();
        button.onClick.AddListener(EndTurn);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EndTurn()
    {
        arena.EndTurn();
    }
}
