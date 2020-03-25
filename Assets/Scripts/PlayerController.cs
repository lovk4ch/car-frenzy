using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMesh))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private ProjectileMoveScript projectilePrefab = null;

    [SerializeField]
    private HealthBar healthBarPrefab = null;

    [SerializeField]
    private ParticleSystem aimPrefab = null;

    [SerializeField]
    [Range(3, 30)]
    private float speed = 10;

    private KeyAction move, jump;
    private Vector3 waypoint;

    private bool isJumping;

    private new Rigidbody rigidbody;

    private HealthBar healthBar = null;
    private float maxHealth = 100, health;

    public float HealthPercentage => health / maxHealth;

    private void Awake()
    {
        move = new KeyAction(KeyInputMode.KeyDown, KeyCode.Mouse0, Action);
        InputManager.Instance.AddKeyAction(move);

        jump = new KeyAction(KeyInputMode.KeyDown, KeyCode.Space, Jump);
        InputManager.Instance.AddKeyAction(jump);

        health = maxHealth;
        waypoint = transform.position;
        rigidbody = GetComponent<Rigidbody>();

        healthBar = Instantiate(healthBarPrefab) as HealthBar;
        healthBar.Initialize(this);
    }

    private void OnDestroy()
    {
        if (healthBar)
            Destroy(healthBar.gameObject);
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        Vector3 projection = Consts.GetProjection(waypoint - transform.position);
        if (projection.magnitude > 1)
        {
            transform.rotation = Quaternion.LookRotation(projection);
            transform.Translate(Vector3.forward * delta * speed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isJumping && collision.collider.name.ToLower().Contains("road"))
        {
            isJumping = false;
        }
        else if (collision.collider.GetComponent<CarController>() is CarController car)
        {
            Hit(car.Damage);
        }
    }

    private void Hit(float damage)
    {
        if (damage < health)
            health -= damage;
        else
            health = 0;
    }

    private void Jump()
    {
        if (!isJumping)
        {
            rigidbody.AddForce(Vector3.up * 20, ForceMode.VelocityChange);
            isJumping = true;
        }
    }

    private void Action()
    {
        Ray MyRay;
        MyRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(MyRay, out RaycastHit hit, 100))
        {
            if (hit.collider)
            {
                if (hit.collider.name.ToLower().Contains("road"))
                {
                    waypoint = hit.point;
                    Instantiate(aimPrefab, waypoint, Quaternion.identity);
                }
                else if (hit.collider.GetComponent<CarController>() is CarController car)
                    Attack(car);
            } 
        }
    }

    private void Attack(CarController car)
    {
        Vector3 direction = car.transform.position - transform.position;
        Vector3 startPos = transform.position + Vector3.up * 1.5f;
        ProjectileMoveScript pms = Instantiate(projectilePrefab, startPos, Quaternion.LookRotation(direction));

        pms.SetTarget(car);
    }
}