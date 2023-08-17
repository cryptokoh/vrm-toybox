using UnityEngine;
using UnityEngine.AI;

public class AiFlyBehaviour : MonoBehaviour
{
    public float flySpeed = 4.0f;                 // Default flying speed.
    public float flyMaxVerticalAngle = 60f;       // Angle to clamp camera vertical movement when flying.

    public EnemyFollow enemyFollow;               // Reference to the EnemyFollow script.

    private Transform player;                     // Reference to the player's transform.
    private Rigidbody rb;                         // Reference to the Rigidbody component.
    private Animator animator;                    // Reference to the Animator component.
    private NavMeshAgent agent; 
    private bool canFly = false;                  // Reference to the NavMeshAgent component.
    private bool fly = false;                      // Boolean to determine whether or not the AI agent is flying.
    private int flyBool;                           // Animator variable related to flying.

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player's transform.
        rb = GetComponent<Rigidbody>();           // Get the Rigidbody component.
        animator = GetComponent<Animator>();      // Get the Animator component.
        agent = GetComponent<NavMeshAgent>();     // Get the NavMeshAgent component.
        flyBool = Animator.StringToHash("Fly");   // Get the animator variable hash for flying.
    }

    private void Update()
{
    // Check if the player is flying.
    //FlyBehaviour playerFlyBehaviour = player.GetComponent<FlyBehaviour>();
    if (canFly)
    {
        // Enable flying mode.
        fly = true;
        animator.SetBool(flyBool, fly);
        agent.enabled = false; // Disable the NavMeshAgent component.
        rb.useGravity = false; // Disable gravity.
        if (enemyFollow != null)
        {
            enemyFollow.enabled = false; // Disable the EnemyFollow script.
        }
    }
    else if (fly)
    {
        // Disable flying mode.
        fly = false;
        animator.SetBool(flyBool, fly);
        rb.useGravity = true; // Enable gravity.
    }

    // Check if the AI agent is touching the ground.
    if (!fly && !agent.enabled && rb.velocity.y < 0.1f && IsGrounded())
    {
        // Enable the NavMeshAgent component and the EnemyFollow script.
        agent.enabled = true;
        if (enemyFollow != null)
        {
            enemyFollow.enabled = true;
        }
    }
}

// Check if the AI agent is grounded using a simple raycast.
private bool IsGrounded()
{
    float groundRayDistance = 1f;
    return Physics.Raycast(transform.position, Vector3.down, groundRayDistance);
}



    private void FixedUpdate()
    {
        if (fly)
        {
            // Rotate the AI agent towards the player's position.
            Vector3 direction = player.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * flyMaxVerticalAngle));

            // Apply force to move the AI agent towards the player's position.
            Vector3 flyDirection = direction.normalized;
            rb.AddForce(flyDirection * flySpeed, ForceMode.Force);
        }
    }
}
