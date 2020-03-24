using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMesh))]
public class CarController : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private float prevVelocity;

    private class WheelData
    {
        public Transform wheelTransform;
        public WheelCollider col;
        public Vector3 wheelStartPos;
        public float rotation = 0.0f;
    }

    private WheelData[] wheels;

    public WheelCollider[] WColForward;
    public WheelCollider[] WColBack;

    public Transform[] wheelsF;
    public Transform[] wheelsB;

    public float wheelOffset = 0.1f;
    public float wheelRadius = 0.13f;

    public float maxSteer = 30;
    public float maxAccel = 25;
    public float maxBrake = 50;

    public Transform centerOfMass;

    public GameObject explosionPrefab;
    public PlayerController Target { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();

        if (centerOfMass)
            rigidbody.centerOfMass = centerOfMass.localPosition;

        wheels = new WheelData[WColForward.Length + WColBack.Length];

        for (int i = 0; i < WColForward.Length; i++)
        {
            wheels[i] = SetupWheels(wheelsF[i], WColForward[i]);
        }

        for (int i = 0; i < WColBack.Length; i++)
        {
            wheels[i + WColForward.Length] = SetupWheels(wheelsB[i], WColBack[i]);
        }
    }

    private void Destroy()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(explosion, 3f);
    }

    private WheelData SetupWheels(Transform wheel, WheelCollider col)
    {
        WheelData result = new WheelData
        {
            wheelTransform = wheel,
            col = col,
            wheelStartPos = wheel.transform.localPosition
        };

        return result;
    }

    private void FixedUpdate()
    {
        /*Move(InputManager.Instance.GetAxis(InputManager.VERTICAL_AXIS),
            InputManager.Instance.GetAxis(InputManager.HORIZONTAL_AXIS));*/
        // UpdateWheels();

        if (prevVelocity - rigidbody.velocity.magnitude > 10)
            Destroy();
        else
            prevVelocity = rigidbody.velocity.magnitude;

        if (NavMeshAgent && Target)
            NavMeshAgent.SetDestination(Target.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<PlayerController>()
            || collision.collider.GetComponent<ProjectileMoveScript>())

            Destroy();
    }

    private void Move(float accel, float steer)
    {
        foreach (WheelCollider col in WColForward)
        {
            col.steerAngle = steer * maxSteer;
        }

        if (accel == 0)
        {
            foreach (WheelCollider col in WColBack)
            {
                col.brakeTorque = maxBrake;
            }
        }
        else
        {
            foreach (WheelCollider col in WColBack)
            {
                col.brakeTorque = 0;
                col.motorTorque = -accel * maxAccel;
            }
        }
    }

    private void UpdateWheels()
    {
        float delta = Time.fixedDeltaTime;

        if (wheels != null)
        {
            foreach (WheelData w in wheels)
            {
                w.rotation = Mathf.Repeat(w.rotation + delta * w.col.rpm * 360.0f / 60.0f, 360.0f);
                w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.col.steerAngle, 0.0f);
            }
        }
    }
}