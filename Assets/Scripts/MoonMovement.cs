using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonMovement : MonoBehaviour
{
    public float rotationTime = 360.0f;
    public float elipseX = 10.0f;    
    public float elipseY = 3.0f;
    public Vector2 elipseCenter= Vector2.zero;

    private float _time = -120f;
    private float time {
        get { return _time; }
        set {
            if (_time == value) return;
            _time = value;
            if (OnNightChange != null) { 
                if (!_isNight && Mathf.Abs(time-(1f/8*rotationTime)) % rotationTime > 3f / 4 * rotationTime) { _isNight = true; OnNightChange(true); }
                if (_isNight && Mathf.Abs(time - (1f / 8 * rotationTime)) % rotationTime <= 3f / 4 * rotationTime) { _isNight = false; OnNightChange(false); }

            }

        }
    }

    private bool _isNight = false;
    public delegate void OnNightChangeDelegate(bool newVal);
    public event OnNightChangeDelegate OnNightChange;

    private void Update()
    {
        time += Time.deltaTime;

        float angle = (time / rotationTime) * 360.0f;
        float x = elipseX * Mathf.Cos(Mathf.Deg2Rad * angle);
        float y = elipseY * Mathf.Sin(Mathf.Deg2Rad * angle);

        Vector3 position = new Vector3(-x, y, transform.localPosition.z) + (Vector3)elipseCenter;
        transform.localPosition = position;

    }


}
