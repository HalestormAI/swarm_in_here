﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class RobotSpawnController : MonoBehaviour
{
    [SerializeField] private int _numberToSpawn;
    [SerializeField] private GameObject _robotPrefab;
    [SerializeField] private Transform _robotPool;
    [SerializeField] private MeshCollider _planeObject;

    private static List<Robot> _spawnedRobots = new List<Robot>();

    public static List<Robot> SpawnedRobots => _spawnedRobots;

    // Start is called before the first frame update
    void Start()
    {
        var planeSize = _planeObject.bounds.size - 2 * Vector3.one;
        var initPos = _planeObject.bounds.min + Vector3.one;
        initPos.y = 0;

        var robotGridSize = (int) Mathf.Ceil(Mathf.Sqrt(_numberToSpawn));

        var positionDelta = planeSize / robotGridSize;

        int rowId = 0;
        for (int i = 0; i < _numberToSpawn; ++i)
        {
            var columnId = i % robotGridSize;
            if (i > 0 && columnId == 0)
            {
                ++rowId;
            }

            var spawnPosition = initPos + new Vector3(positionDelta.x * rowId, 0, positionDelta.z * columnId);

            var randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            var robotGO = Instantiate(_robotPrefab, spawnPosition, randomRotation, _robotPool);
            robotGO.name = string.Format("Robot {0}: ({0}, {1})", i, rowId, columnId);
            var robot = robotGO.GetComponent<Robot>();
            robot.Name = $"Robot {i}";
            robot.IsReceivingIR = true;
            robot.SetHeadColour(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f));
            _spawnedRobots.Add(robot);
        }
    }
}