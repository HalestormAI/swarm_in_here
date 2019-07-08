using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum IrDirection
{
    Forward,
    Backward,
    Left,
    Right
}

public class Robot : MonoBehaviour
{
    public Action<Robot, IrDirection, bool, bool> OnNearbyRobot;

    [SerializeField] private RobotIRSignalWrapper _signalWrapper;

    [Header("Actuators")]
    [SerializeField] private LightController _lightController;
    [SerializeField] private MotionController _motionController;

    [Header("IR/Collisions")]
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _irRange = 5;
    [SerializeField] private GameObject IR_Front;
    [SerializeField] private GameObject IR_Back;
    [SerializeField] private GameObject IR_Left;
    [SerializeField] private GameObject IR_Right;

    public bool IsReceivingIR { get; set; }
    public string Name { get; set; }
    public bool ShouldEmit = true;


    // Start is called before the first frame update
    void Start()
    {
        _signalWrapper = ScriptableObject.CreateInstance<RobotIRSignalWrapper>();
        _signalWrapper.RegisterRobot(this);
        _signalWrapper.RegisterListener(_lightController);
        _signalWrapper.RegisterListener(_motionController);

        _motionController.HitTarget += OnHitTarget;

        IsReceivingIR = true;
        if (ShouldEmit)
        {
            StartCoroutine(EmitIR());
        }
    }

    private void OnHitTarget()
    {
        _lightController.SetColour(Random.ColorHSV(0f,1f, 0.5f, 1f, 0.5f, 1f));
    }

    private IEnumerator EmitIR()
    {
        while (ShouldEmit)
        {
            EmitDirectionalIR(transform.forward, IrDirection.Forward);
            EmitDirectionalIR(transform.right, IrDirection.Right);
            EmitDirectionalIR(-transform.forward, IrDirection.Backward);
            EmitDirectionalIR(-transform.right, IrDirection.Left);
            var delay = UnityEngine.Random.Range(0.001f, 0.1f);
            yield return new WaitForSeconds(delay);
        }
    }

    private void EmitDirectionalIR(Vector3 direction, IrDirection sensorDirection)
    {
        Vector3 originPosition = transform.position;

        switch (sensorDirection)
        {
            case IrDirection.Forward:
                originPosition = IR_Front.transform.position;
                break;
            case IrDirection.Backward:
                originPosition = IR_Back.transform.position;
                break;
            case IrDirection.Left:
                originPosition = IR_Left.transform.position;
                break;
            case IrDirection.Right:
                originPosition = IR_Right.transform.position;
                break;
        }

        Ray ray = new Ray(originPosition + new Vector3(0, 0.5f, 0), direction);
        Debug.DrawRay(ray.origin, _irRange * ray.direction, Color.red, 0.01f);
        if (Physics.Raycast(ray, out RaycastHit hit, _irRange, _layerMask))
        {
            var robot = hit.collider.gameObject.GetComponentInParent<Robot>();
            if (robot == null)
            {
                robot = hit.collider.transform.parent.GetComponentInParent<Robot>();
            }

            if (robot == null)
            {
                return;
            }

            if (robot.IsReceivingIR)
            {
                Debug.LogFormat("{0}: Hello {1} ({2})", Name, robot.Name, sensorDirection);
                OnNearbyRobot?.Invoke(robot, sensorDirection, true, false);
            }
            OnNearbyRobot?.Invoke(robot, sensorDirection, robot.IsReceivingIR, true);
        }
    }

    public void Stop()
    {
        _motionController.CancelMovement();
    }

    public void SetHeadColour(Color colour)
    {
        _lightController.SetColour(colour);
    }
    public Color GetHeadColour()
    {
        return _lightController.GetColour();
    }
}