using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEvent : MonoBehaviour
{

    // Define the event delegate
    public delegate void ClickEventHandler(GameObject tile);
    public event ClickEventHandler OnClick;

    private void OnMouseDown()
    {
        if (OnClick != null)
        {
            OnClick.Invoke(gameObject);
        }
    }
}
