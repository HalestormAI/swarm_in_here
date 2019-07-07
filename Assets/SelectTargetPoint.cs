using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class SelectTargetPoint : MonoBehaviour
{
    public Action<Vector3> TargetChanged;

    [SerializeField] private GameObject _targetPrefab;
    [SerializeField] private Vector3 CurrentTarget;

    private GameObject _spawnedPrefab;
    private MeshCollider _collider;
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _collider = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Clicked();
        }
    }
    void Clicked()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (_collider.Raycast(ray,  out hit, 50))
        {
            CurrentTarget = hit.point;
            if (_spawnedPrefab == null)
            {
                _spawnedPrefab = Instantiate(_targetPrefab, CurrentTarget, Quaternion.identity);
            }

            _spawnedPrefab.transform.position = CurrentTarget;

            TargetChanged?.Invoke(CurrentTarget);
        }
    }
}
