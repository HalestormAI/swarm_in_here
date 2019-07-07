using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GroundEventWrapper", order = 1)]

public class GroundEventWrapper : ScriptableObject
{
    private Arena _arena;
    private List<IGroundEventListener> _listeners = new List<IGroundEventListener>();

    public void RegisterArena(Arena ar)
    {
        _arena = ar;
        _arena.GroundPlaneTargetChanged += GroundPlaneTargetChanged;
    }

    private void GroundPlaneTargetChanged(Vector3 obj)
    {
        foreach (var listener in _listeners)
        {
            listener.OnTargetChanged(obj);
        }
    }

    public void RegisterListener(IGroundEventListener listener)
    {
        _listeners.Add(listener);
    }
    public void UnRegisterListener(IGroundEventListener listener)
    {
        _listeners.Remove(listener);
    }
}
