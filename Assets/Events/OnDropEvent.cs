using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDropEvent : MonoBehaviour
{
    //red color to change the color of the tile
    private Color redColor = new Color(255, 0, 0);
    private Color standardColor;
    private MeshRenderer mesh;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        standardColor = mesh.material.color;
    }

    private void OnMouseEnter()
    {
        mesh.material.color=redColor;
    }
    private void OnMouseOver()
    {
        
    }
    private void OnMouseExit()
    {
        mesh.material.color = standardColor;
    }
}
