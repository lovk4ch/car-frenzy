using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMesh))]
public class PlayerController : MonoBehaviour
{
    #region Animations

    private const int stateIdle = 0, stateRun = 1, stateJump = 2, stateDeath = 3;
    private const string actionStateName = "actionState";

    private Animator animator;

    private int actionState
    {
        get => animator.GetInteger(actionStateName);
        set => animator.SetInteger(actionStateName, value);
    }

    #endregion

    [SerializeField]
    private ProjectileMoveScript projectilePrefab = null;

    [SerializeField]
    private HealthBar healthBarPrefab = null;

    [SerializeField]
    private ParticleSystem aimPrefab = null;

    [SerializeField]
    [Range(5, 30)]
    private float jumpPower = 15;

    [SerializeField]
    [Range(3, 30)]
    private float speed = 10;

    private KeyAction move, jump;
    private Vector3 waypoint;

    private new Rigidbody rigidbody;

    private HealthBar healthBar = null;
    private float maxHealth = 100, health;

    public bool IsAlive => HealthPercentage > 0;
    public float HealthPercentage => health / maxHealth;

    private void Awake()
    {
        move = new KeyAction(KeyInputMode.KeyDown, KeyCode.Mouse0, Action);
        InputManager.Instance.AddKeyAction(move);

        jump = new KeyAction(KeyInputMode.KeyDown, KeyCode.Mouse1, Jump);
        InputManager.Instance.AddKeyAction(jump);

        animator = GetComponent<Animator>();
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
        if (actionState == stateRun || actionState == stateJump)
        {
            if (projection.magnitude > 1)
            {
                transform.rotation = Quaternion.LookRotation(projection);
                transform.Translate(Vector3.forward * delta * speed);
            }
            else if (actionState == stateRun)
                actionState = stateIdle;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (actionState == stateJump && collision.collider.name.ToLower().Contains("road"))
        {
            actionState = stateRun;
        }
        else if (collision.collider.GetComponent<CarController>() is CarController car)
        {
            if (Mathf.Abs(transform.position.y - car.transform.position.y) < 2.3f)
                Hit(car.damage);
        }
    }

    private void Hit(float damage)
    {
        if (damage < health)
            health -= damage;
        else
        {
            health = 0;
            actionState = stateDeath;
            InputManager.Instance.RemoveKeyActions(this);
        }
    }

    private void Jump()
    {
        if (actionState != stateJump)
        {
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            actionState = stateJump;
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
                    if (actionState != stateJump)
                    {
                        actionState = stateRun;
                        waypoint = hit.point;
                        Instantiate(aimPrefab, waypoint, Quaternion.identity);
                    }
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