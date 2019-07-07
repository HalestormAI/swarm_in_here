using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Action<Color> ColourChanged;

    // Start is called before the first frame update
    Material _mat;
    void Start()
    {
        _mat = gameObject.GetComponent<Renderer>().material;
    }

    public void SetColour(Color color)
    {
        _mat.color = color;
        ColourChanged?.Invoke(color);
    }
}
