using UnityEngine;

public class RigidbodyFollowPlayer : MonoBehaviour
{
    public Transform lookAtTarget;
    public Rigidbody rb;
    public float thrust = 1f;
    public float revthrust = -1f;
    public float rotationSpeed = 5f;
    public float maxDist = 10f;
    public float minDist = 5f;
    public float maxHorizontalDist = 5f;
    public float targetDist = 7f;
    public bool enableOnStart = true;

    public float radiusMin = 5f;
    public float radiusMax = 10f;
    private Vector3 randomTarget;
    public float targetChangeInterval = 5f;

    private Transform player;
    private float distance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lookAtTarget = player;
        if (enableOnStart)
        {
            enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            

            distance = Vector3.Distance(rb.position, player.position);

            if (distance > minDist && distance < maxDist)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 5);
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 5);
            }
            else if (distance < minDist)
            {
                rb.AddForce(-rb.velocity.normalized * revthrust, ForceMode.Force);
            }
            else if (distance > maxDist || IsOutsideHorizontalRange())
            {
                rb.AddForce(rb.velocity.normalized * thrust, ForceMode.Force);
            }

            UpdateRandomTarget();

            UpdateRotation();
        }
    }

    private void UpdateRotation()
    {
        Vector3 targetPosition = lookAtTarget.position + (lookAtTarget.forward * targetDist);
        Vector3 directionToTarget = targetPosition - rb.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        float step = rotationSpeed * Time.fixedDeltaTime;
        Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, step);
        rb.MoveRotation(newRotation);
    }

    private bool IsOutsideHorizontalRange()
    {
        Vector3 targetPosition = lookAtTarget.position + (lookAtTarget.forward * targetDist);
        Vector3 directionToTarget = targetPosition - rb.position;
        float horizontalDistance = new Vector3(directionToTarget.x, 0f, directionToTarget.z).magnitude;
        return horizontalDistance > maxHorizontalDist;
    }

    private void UpdateRandomTarget()
    {
        Vector3 offsetFromPlayerToTarget = randomTarget - player.position;

        if (offsetFromPlayerToTarget.magnitude > radiusMax || offsetFromPlayerToTarget.magnitude < radiusMin)
        {
            float distance = Random.Range(radiusMin, radiusMax);
            float angle = Random.Range(0, 2 * Mathf.PI);
            randomTarget = player.position + new Vector3(distance * Mathf.Cos(angle), 0, distance * Mathf.Sin(angle));
        }

        if (Time.fixedTime % targetChangeInterval == 0)
        {
            randomTarget = player.position + Random.insideUnitSphere * radiusMax;
        }

        rb.AddForce((randomTarget - rb.position).normalized * thrust, ForceMode.Force);
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, radiusMin);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, radiusMax);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(randomTarget, 0.5f);
        }
    }
}
