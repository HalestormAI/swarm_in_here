using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "RobotDrivingParameters", menuName = "ScriptableObjects/RobotDrivingParameters", order = 1)]
class RobotDrivingParameters : ScriptableObject
{
    public float maxSteeringAngle = 70; // maximum steer angle the wheel can have
    public float maxTorque = 1f;
}
