using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMesh))]
public class CarController : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab = null;

    [SerializeField]
    private Transform centerOfMass = null;

    [Range(0, 100)]
    public float damage = 10;

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

    public float maxSteer = 30;
    public float maxAccel = 25;
    public float maxBrake = 50;

    public float wheelOffset = 0.1f;
    public float wheelRadius = 0.13f;

    public PlayerController Target { get; set; }
    public NavMeshAgent NavMeshAgent { get; set; }
    
    public Vector3 position => centerOfMass.position;

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
        if (prevVelocity - rigidbody.velocity.magnitude > 10)
            Destroy();
        else
            prevVelocity = rigidbody.velocity.magnitude;

        if (NavMeshAgent.isOnNavMesh && Target.IsAlive)
            NavMeshAgent.SetDestination(Target.transform.position);
    }

    private void OnDestroy()
    {
        if (!LevelManager.Instance)
            return;

        LevelManager.Instance.Remove(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<ProjectileMoveScript>())
            Destroy();
        else if (collision.collider.GetComponent<PlayerController>() is PlayerController player)
        {
            // if (player.IsAlive)
                Destroy();
        }
    }
}