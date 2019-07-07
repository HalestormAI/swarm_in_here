using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MotionController : MonoBehaviour, IGroundEventListener, IRobotIRSignalListener
{
    public Action HitTarget;

    [SerializeField] private List<AxleInfo> _axleInfos; // the information about each individual axle
    [SerializeField] private RobotDrivingParameters _drivingParameters;
    [SerializeField] private GroundEventWrapper _eventWrapper;
//    [SerializeField] private float _maxRotationPerFrame = 6f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private AnimationCurve distanceCurve;
    [SerializeField] private Rigidbody _rigidbody;
    private Coroutine _coroutine;
    private Coroutine _tempTargetCoroutine;
    private Vector3 _target;

    private bool shouldWait = false;

    private bool _driving = false;

    // Start is called before the first frame update
    void Start()
    {
        _eventWrapper.RegisterListener(this);
    }

    void OnDestroy()
    {
        _eventWrapper.UnRegisterListener(this);
    }

    public void SetTarget(Vector3 pos)
    {
        _target = pos;
        _target.y = transform.position.y;
        _driving = true;
//        Debug.Log("Setting target: " + _target);
//        if (_coroutine != null)
//        {
//            Debug.Log("Stopping coroutine");
//            StopCoroutine(_coroutine);
//        }
//
//        _coroutine = StartCoroutine(MoveToPosition());
    }
//
//    IEnumerator MoveToPosition()
//    {
//        var distance = Vector3.Distance(transform.position, _target);
//        while (distance > 0)
//        {
//            if (shouldWait)
//            {
//                yield return new WaitForSeconds(Random.Range(1f, 3f));
//                shouldWait = false;
//            }
//
//            var drnToTarget = _target - transform.position;
//            float step = _maxRotationPerFrame * Time.deltaTime;
//            var newDir = Vector3.RotateTowards(transform.forward, drnToTarget, step, 1f);
//
//            // Move our position a step closer to the target.
//            transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
//
//            float moveStep = _maxTranslationPerFrame * Time.deltaTime; // calculate distance to move
//            transform.position = Vector3.MoveTowards(transform.position, _target, moveStep);
//            distance = Vector3.Distance(transform.position, _target);
//
//            yield return null;
//        }
//
//        HitTarget?.Invoke();
//    }


    public void FixedUpdate()
    {
        if (!_driving)
        {
            return;
        }

        var distance = Vector3.Distance(transform.position, _target);
        float v = _rigidbody.velocity.sqrMagnitude / 1;


        float motor =  distanceCurve.Evaluate(v) * _drivingParameters.maxTorque;

        var drnToTarget = _target - transform.position;
        var angle = Vector3.SignedAngle(transform.forward, drnToTarget, Vector3.up) + Random.Range(-10f, 10f);

        float steering = (angle < 0 ? -1 : 1) * Mathf.Min(Mathf.Abs(angle), _drivingParameters.maxSteeringAngle);


        if (distance < 0.2) 
        {
            _driving = false;
            HitTarget?.Invoke();
        }

        foreach (AxleInfo axleInfo in _axleInfos)
        {
            if (!_driving && axleInfo.handbrake)
            {
                axleInfo.leftWheel.brakeTorque = 1000000;
                axleInfo.rightWheel.brakeTorque = 1000000;
                continue;
            }

            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }

    public void OnTargetChanged(Vector3 pos)
    {
        SetTarget(pos);
    }

    void OnCollisionEnter()
    {
        if (_tempTargetCoroutine == null)
        {
            _tempTargetCoroutine = StartCoroutine(SetTemporaryTarget());
        }
    }

    public void CancelMovement()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }

    public void OnRobotNearby(Robot robot, IrDirection drn, bool isReceiving, bool force)
    {
        if (drn == IrDirection.Forward)
        {
//            shouldWait = true;

            if (_tempTargetCoroutine == null)
            {
                _tempTargetCoroutine = StartCoroutine(SetTemporaryTarget());
            }
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


    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor; // is this wheel attached to motor?
        public bool handbrake; // is this wheel attached to handbrake?
        public bool steering; // does this wheel apply steer angle?
    }
}