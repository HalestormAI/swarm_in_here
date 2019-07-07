using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RobotIRSignalWrapper", menuName = "ScriptableObjects/RobotIRSignalWrapper", order = 1)]

public class RobotIRSignalWrapper : ScriptableObject
{
    private Robot _robot;
    private List<IRobotIRSignalListener> _listeners = new List<IRobotIRSignalListener>();

    public void RegisterRobot(Robot robot)
    {
        _robot = robot;
        _robot.OnNearbyRobot += OnNearbyRobot;
    }

    private void OnNearbyRobot(Robot robot, IrDirection drn, bool isReceiving, bool force)
    {
        foreach (var listener in _listeners)
        {
            listener.OnRobotNearby(robot, drn, isReceiving, force);
        }
    }

    public void RegisterListener(IRobotIRSignalListener listener)
    {
        _listeners.Add(listener);
    }
    public void UnRegisterListener(IRobotIRSignalListener listener)
    {
        _listeners.Remove(listener);
    }
}
