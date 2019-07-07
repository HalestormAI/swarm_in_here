using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour, IGroundEventListener
{
    [SerializeField] private GroundEventWrapper _eventWrapper;
    [SerializeField] private float _isFacingTolerance = 0.1f;
    [SerializeField] private float _maxRotationPerFrame = 6f;
    [SerializeField] private float _maxTranslationPerFrame = 1f;
    [SerializeField] private LayerMask _layerMask;

    private Coroutine _coroutine;
    private Vector3 _target;


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
        Debug.Log("Setting target: " + _target);
        if (_coroutine != null)
        {
            Debug.Log("Stopping coroutine");
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(MoveToPosition());
    }

    IEnumerator MoveToPosition()
    {
        var distance = Vector3.Distance(transform.position, _target);
        while (distance > 0)
        {
            var drnToTarget = _target - transform.position;
            float step = _maxRotationPerFrame * Time.deltaTime;
            var newDir = Vector3.RotateTowards(transform.forward, drnToTarget,  step, 1f);


            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);

            float moveStep = _maxTranslationPerFrame * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, _target, moveStep);
            distance = Vector3.Distance(transform.position, _target);


            yield return null;
        }
    }

    private bool IsFacing(Vector3 pos)
    {
        // Direction from robot to point
        var drn = transform.position - pos;
        return Vector3.Angle(transform.forward, drn) < _isFacingTolerance;
    }

    public void OnTargetChanged(Vector3 pos)
    {
        SetTarget(pos);
    }

    void OnCollisionEnter()
    {
        CancelMovement();
    }

    public void CancelMovement()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }
}