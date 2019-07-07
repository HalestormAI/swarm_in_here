using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour, IRobotIRSignalListener
{
    public Action<Color> ColourChanged;

    [SerializeField] private RobotIRSignalWrapper _signalWrapper;

    private Color _currentColour;

    // Start is called before the first frame update
    Material _mat;

    void Awake()
    {
        _mat = gameObject.GetComponent<Renderer>().material;
    }

    public void SetColour(Color color)
    {
        _currentColour = color;
        _mat.SetColor("_EmissionColor", _currentColour);

        ColourChanged?.Invoke(color);
    }

    public Color GetColour()
    {
        return _currentColour;
    }

    public void OnRobotNearby(Robot robot, IrDirection drn, bool isReceiving, bool force)
    {
        SetColour(robot.GetHeadColour());
    }
}