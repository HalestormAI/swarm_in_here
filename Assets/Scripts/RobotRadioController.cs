using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class RobotRadioController : MonoBehaviour
    {
        [SerializeField] private float _radioRange;

        public void BCast(Robot source, MockPacket message)
        {
            Debug.LogFormat("<color={0}>{1}: {2} [{3}]</color>", message.LogColor, source.Name, message.type, message.guid);

            foreach (var robot in RobotSpawnController.SpawnedRobots)
            {
                if (robot.Name == source.Name)
                {
                    continue;
                }

                if (Vector3.Distance(source.transform.position, robot.transform.position) < _radioRange)
                {
                    robot.ReceiveBCast(source, message);
                }
            }
        }
    }
}
