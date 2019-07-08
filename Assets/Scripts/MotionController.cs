using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MotionController : MonoBehaviour, IGroundEventListener, IRobotIRSignalListener
{
    public Action HitTarget;

    [SerializeField] private RobotDrivingParameters _drivingParameters;

    [SerializeField] private GroundEventWrapper _eventWrapper;

//    [SerializeField] private float _maxRotationPerFrame = 6f;
    [SerializeField] private LayerMask _layerMask;
    private Coroutine _coroutine;
    private Coroutine _tempTargetCoroutine;
    private Vector3 _target;

    private bool shouldWait = false;
    private bool _reversing = false;

    private Robot _robot;

    // Start is called before the first frame update
    void Start()
    {
        _eventWrapper.RegisterListener(this);
        _robot = GetComponent<Robot>();
        Assert.IsNotNull(_robot);
    }

    void OnDestroy()
    {
        _eventWrapper.UnRegisterListener(this);
    }

    public void SetTarget(Vector3 pos)
    {
        _target = pos;
        _target.y = transform.position.y;
//        Debug.Log("Setting target: " + _target);
        if (_coroutine != null)
        {
//            Debug.Log("Stopping coroutine");
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(MoveToTarget());
    }

//
    IEnumerator MoveToTarget()
    {
        var distance = Vector3.Distance(transform.position, _target);
        while (distance > 0)
        {
            if (shouldWait)
            {
                yield return new WaitForSeconds(Random.Range(1f, 3f));
                shouldWait = false;
            }

            var drnToTarget = _target - transform.position;
            float step = _drivingParameters.maxSteeringAngle * Time.deltaTime;
            var newDir = Vector3.RotateTowards(transform.forward, drnToTarget, step, 1f);

            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
            PerformMoveStep();
            distance = Vector3.Distance(transform.position, _target);

            yield return null;
        }

        HitTarget?.Invoke();
    }

    private void PerformMoveStep()
    {
        float moveStep = _drivingParameters.maxVelocity * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, _target, moveStep);
    }

    public void Reverse(float amount)
    {
        if (_reversing)
        {
            return;
        }

        _reversing = true;
        CancelMovement(false);
        
        _coroutine = StartCoroutine(ReverseCoroutine(amount));
    }

    private IEnumerator ReverseCoroutine(float amount)
    {
        var originalTarget = _target;
        _target = transform.position - amount * transform.forward;

        float distance = Single.MaxValue;

        do
        {
            PerformMoveStep();
            distance = Vector3.Distance(transform.position, _target);
            yield return null;
        } while (distance > 0.1);

        _target = originalTarget;
        _reversing = false;
    }

    public void OnTargetChanged(Vector3 pos)
    {
        SetTarget(pos);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_layerMask != (_layerMask | (1 << collision.gameObject.layer)))
        {
            return;
        }

        CancelMovement(true);
    }

    public void CancelMovement(bool raiseSpaceReqBroadcast)
    {
        // If we're close, we might want to get everyone to backup so we can try to get to it
        if (raiseSpaceReqBroadcast && Vector3.Distance(transform.position, _target) < 2)
        {
            var packet = new RequestSpacePacket(1);
            packet.lifetimeSeconds = 4;
            _robot.SendBCast(packet);
            shouldWait = true;
        } else if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }

    public void OnRobotNearby(Robot robot, IrDirection drn, bool isReceiving, bool force)
    {
        if (drn == IrDirection.Forward)
        {
            var distance = Vector3.Distance(transform.position, robot.transform.position);

            if (distance < 1)
            {
                CancelMovement(true);
            }

//            if (_tempTargetCoroutine == null)
//            {
//                _tempTargetCoroutine = StartCoroutine(SetTemporaryTarget());
//            }

//            if (_brakingCoroutine == null)
//            {
//                StartCoroutine(ApplyBrakes());
//            }   
        }
    }

    private IEnumerator SetTemporaryTarget()
    {
        Debug.Log("Redirecting the robot!");
        transform.Rotate(Vector3.up, Random.Range(45f, 180f));
        var originalTarget = _target;
        _target = transform.forward * 10;
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        _target = originalTarget;
        Debug.Log("Resetting the robot!");
    }
}