using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public NavMeshAgent enemy;
    public float aggroDistance = 10f;
    public float updateRate = 2;
    public float rotationSpeed = 3f;
    public Animator anim;

    private Transform player;
    private float nextUpdateTime = 0;

    private Rigidbody parentRb;
    private static int nextID = 0;
    public int ID { get; private set; }

    private enum State { Idle, Chase }
    private State currentState = State.Chase;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        parentRb = GetComponentInParent<Rigidbody>();
        enemy = GetComponentInParent<NavMeshAgent>();
        anim = GetComponentInParent<Animator>();
        ID = nextID++;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateRate;
            switch (currentState)
            {
                case State.Idle:
                    // Add any idle behavior here.
                    break;

                case State.Chase:
                    enemy.enabled = true;
                    ChaseBehavior(distanceToPlayer);
                    RotateTowardsPlayer();
                    MaintainSeparation();
                    break;
            }
        }
    }

    private void ChaseBehavior(float distanceToPlayer)
    {
        if (enemy.isOnNavMesh && distanceToPlayer <= aggroDistance)
        {
            Vector3 offset = new Vector3(Mathf.Cos(ID), 0, Mathf.Sin(ID));

            if (CheckGrounded())
            {
                enemy.SetDestination(player.position + offset * enemy.stoppingDistance);
            }
        }
        else
        {
            if (CheckGrounded())
            {
                enemy.ResetPath();
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        if (Quaternion.Angle(transform.rotation, lookRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void MaintainSeparation()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemy.stoppingDistance);
        foreach (var collider in colliders)
        {
            EnemyFollow otherEnemy = collider.GetComponent<EnemyFollow>();
            if (otherEnemy != null && otherEnemy != this)
            {
                Vector3 awayFromOther = transform.position - otherEnemy.transform.position;
                enemy.Move(awayFromOther.normalized * Time.deltaTime);
            }
        }
    }

    private bool CheckGrounded()
    {
        float raycastDistance = 1f;
        Ray ray = new Ray(parentRb.position + Vector3.up * 0.1f, Vector3.down);
        return Physics.Raycast(ray, raycastDistance);
    }

    void OnDrawGizmos()
    {
        // Gizmos drawing code remains the same
    }
}
