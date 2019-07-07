using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public Action<Vector3> GroundPlaneTargetChanged;

    [SerializeField] private GroundEventWrapper _eventWrapper;
    private SelectTargetPoint _pointSelector;

    void Start()
    {
        _eventWrapper.RegisterArena(this);

        _pointSelector = GetComponentInChildren<SelectTargetPoint>();
        if (_pointSelector != null)
        {
            _pointSelector.TargetChanged += TargetChanged;
        }
    }

    private void TargetChanged(Vector3 obj)
    {
        // Might want to do more?
        GroundPlaneTargetChanged?.Invoke(obj);
    }
}
