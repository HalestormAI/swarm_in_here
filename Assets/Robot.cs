using System;
using System.Collections;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [Header("Actuators")] [SerializeField] private LightController _lightController;
    [SerializeField] private MotionController _motionController;

    [Header("IR/Collisions")] [SerializeField]
    private LayerMask _layerMask;

    [SerializeField] private float _irRange = 5;

    public bool IsReceivingIR { get; set; }
    public string Name { get; set; }
    public bool ShouldEmit = true;


    // Start is called before the first frame update
    void Start()
    {
        IsReceivingIR = true;
        if (ShouldEmit)
        {
            StartCoroutine(EmitIR());
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator EmitIR()
    {
        while (true)
        {
            EmitDirectionalIR(transform.forward);
            EmitDirectionalIR(transform.right);
            EmitDirectionalIR(-transform.forward);
            EmitDirectionalIR(-transform.right);
            var delay = UnityEngine.Random.Range(0.001f, 0.1f);
            yield return new WaitForSeconds(delay);
        }
    }

    private void EmitDirectionalIR(Vector3 direction)
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.5f, 0), direction);
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
                Debug.LogFormat("{0}: Hello {1}", Name, robot.Name);
                robot.Stop();
                Stop();
                robot.IsReceivingIR = false;
            }
            _lightController.SetColour(robot.GetHeadColour());
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