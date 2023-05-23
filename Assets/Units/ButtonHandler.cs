using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private Button button;
    private UnitSpawn unitSpawn;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleSpawning);
        unitSpawn = FindObjectOfType<UnitSpawn>();
    }

    private void HandleSpawning()
    {
        if (!unitSpawn.Spawn())
        {
            Debug.LogError("Button error");
        }
        
    }
}
