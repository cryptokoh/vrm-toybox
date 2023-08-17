using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyMovement : MonoBehaviour
{
    // Enemy Stats
    public int Damage = 10;
    public float AttackDelay = 0.5f;
 public bool showGizmos = true;
    // NavMeshAgent Configs
    public float AIUpdateInterval = 0.1f;
    public float Acceleration = 8;
    public float AngularSpeed = 120;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;
    // Add a private boolean variable to track if the enemy can attack
    private bool canAttack = false;
    private bool isAttacking = false;

    // Distance threshold for attacking the player
    public float AttackDistanceThreshold = 2.0f;

    // Add a private Coroutine to handle the attack cooldown
    private Coroutine attackCooldownCoroutine;


    // References
    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private PlayerHealthBar PlayerHealthBar;
    private FloatingText floatingTextRef;

    public bool IsAttacking { get; private set; }


    // Constants
    private const string ATTACK_TRIGGER = "Attack";
    private const string IS_MOVING = "isMoving";
    private const string JUMP_TRIGGER = "Jump";
    private const string LANDED_TRIGGER = "Landed";

    private float distanceToPlayer; 

    private static int nextID = 0;
    public int ID { get; private set; }

    private void Start()
        {
            ID = nextID++;
        }
    
    private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            PlayerHealthBar = player.GetComponent<PlayerHealthBar>();
        
        }

    private void OnEnable()
        {
            SetupAgentFromConfiguration();
            StartChasing();
        }

    public void StartChasing()
        {
            StartCoroutine(FollowTarget());
        }

    private void MaintainSeparation()
    {
        Vector3 avoidanceForce = Vector3.zero;
        Collider[] colliders = Physics.OverlapSphere(transform.position, agent.stoppingDistance);

        foreach (var collider in colliders)
        {
            EnemyMovement otherEnemy = collider.GetComponent<EnemyMovement>();
            if (otherEnemy != null && otherEnemy != this)
            {
                Vector3 awayFromOther = transform.position - otherEnemy.transform.position;
                avoidanceForce += awayFromOther.normalized / awayFromOther.magnitude;
            }
        }

        if (avoidanceForce != Vector3.zero && !IsAttacking)
        {
            Vector3 newDestination = transform.position + avoidanceForce.normalized * agent.stoppingDistance;
            agent.SetDestination(newDestination);
        }
    }


    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(AIUpdateInterval);
        Ray groundCheckRay = new Ray();

        while (gameObject.activeSelf)
        {
            if (agent.isOnNavMesh && IsAgentOnGround() && !IsAttacking)
            {
                agent.SetDestination(player.position);
                //MaintainSeparation();
            }

            yield return wait;
        }
    }


    private bool IsAgentOnGround()
        {
            float raycastDistance = .5f; // Adjust this value as needed.
            Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
            return Physics.Raycast(ray, raycastDistance);
        }

    private void Update()
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.position);
            anim.SetBool(IS_MOVING, agent.velocity.magnitude > 0.01f);
        }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(Attack());
            anim.SetBool("isAttacking", false);

            agent.isStopped = false;  // Allow the agent to move again
            IsAttacking = false;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (distanceToPlayer <= AttackDistanceThreshold)
            {
                if (!IsAttacking)
                {
                    anim.SetBool("isAttacking", true);
                    anim.SetTrigger(ATTACK_TRIGGER);
                    agent.velocity = Vector3.zero;  // Halt the agent
                    IsAttacking = true;
                }
            }
            else if (distanceToPlayer > AttackDistanceThreshold + 1)
            {
                anim.SetBool("isAttacking", false);
                agent.velocity = Vector3.forward * Speed;  // Allow it to move
                IsAttacking = false;
            }
        }
    }





    public void StartAttack()
        {
            
                StartCoroutine(Attack());

                   // Start the attack cooldown coroutine
                    if (attackCooldownCoroutine != null)
                        StopCoroutine(attackCooldownCoroutine);

                attackCooldownCoroutine = StartCoroutine(AttackCooldown());
        }

    private IEnumerator Attack()
    {
        PlayerHealthBar.ApplyDamage(Damage);
        FloatingText.TriggerFloatingTextWithColor(player.transform.position, Damage, Color.red);
        yield return new WaitForSeconds(AttackDelay);
    }

        private IEnumerator Fire()
    {
        //PlayerHealthBar.ApplyDamage(Damage);
        //FloatingText.TriggerFloatingTextWithColor(player.transform.position, Damage, Color.red);
        yield return new WaitForSeconds(AttackDelay);
    }

    private IEnumerator AttackCooldown()
    {
        // Wait for the specified attack cooldown before allowing the enemy to attack again
        
        yield return new WaitForSeconds(AttackDelay);
        canAttack = true;
    }

    private void SetupAgentFromConfiguration()
    {
        agent.acceleration = Acceleration;
        agent.angularSpeed = AngularSpeed;
        agent.avoidancePriority = AvoidancePriority;
        agent.baseOffset = BaseOffset;
        agent.height = Height;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType;
        agent.radius = Radius;
        agent.speed = Speed;
        agent.stoppingDistance = StoppingDistance;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackDistanceThreshold);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, StoppingDistance);
        }
    }
}
