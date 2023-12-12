using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBloom : MonoBehaviour
{
    public Material defaultMaterial;
    public Material bloomMaterial;
    public Material redMaterial;
    private MoonMovement moonmove;

    private void Start()
    {
        moonmove = GameObject.FindAnyObjectByType<MoonMovement>();
        moonmove.OnNightChange += OnNightHandler;

    }

    private void OnNightHandler(bool isNight)
    {
        GetComponent<Renderer>().material = isNight ? bloomMaterial : defaultMaterial;
    }
}
