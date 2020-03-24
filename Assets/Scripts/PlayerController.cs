using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMesh))]
public class PlayerController : MonoBehaviour
{
    private KeyAction move, jump;
    private Vector3 target;

    private bool isJumping;

    [SerializeField]
    [Range(3, 30)]
    private float speed = 10;

    [SerializeField]
    private GameObject projectilePrefab = null;

    private new Rigidbody rigidbody;

    // private NavMeshAgent navMesh;

    private void Awake()
    {
        move = new KeyAction(KeyInputMode.KeyDown, KeyCode.Mouse0, Move);
        InputManager.Instance.AddKeyAction(move);

        jump = new KeyAction(KeyInputMode.KeyDown, KeyCode.Space, Jump);
        InputManager.Instance.AddKeyAction(jump);

        target = transform.position;
        rigidbody = GetComponent<Rigidbody>();
        // navMesh = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        Vector3 projection = Consts.GetProjection(target - transform.position);
        if (projection.magnitude > 1)
        {
            transform.rotation = Quaternion.LookRotation(projection);
            transform.Translate(Vector3.forward * delta * speed);
        }
        // navMesh.SetDestination(target);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isJumping && collision.collider.name.ToLower().Contains("road"))
        {
            // navMesh.enabled = true;
            isJumping = false;
        }
    }

    private void Jump()
    {
        if (!isJumping)
        {
            // navMesh.enabled = false;
            rigidbody.AddForce(Vector3.up * 20, ForceMode.VelocityChange);
            isJumping = true;
        }
    }

    private void Move()
    {
        Ray MyRay;
        MyRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(MyRay, out RaycastHit hit, 100))
        {
            if (hit.collider)
            {
                if (hit.collider.name.ToLower().Contains("road"))
                    target = hit.point;
                else if (hit.collider.name.ToLower().Contains("seperate"))
                    Hit(hit.point);
            } 
        }
    }

    private void Hit(Vector3 position)
    {
        Vector3 direction = Consts.GetProjection(position - transform.position);
        Instantiate(projectilePrefab, transform.position + Vector3.up * 1.5f, Quaternion.LookRotation(direction));
    }
}