using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WaypointAgent : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float distanceThreshold = 0.1f;
    public float rotationSpeed = 10f;
    public float rayDistance = 3f;
    public float avoidForce = 5f;
    public float separationRadius = 2f; // Separation radius for avoiding other agents
    [SerializeField] private bool randomWaypoints = true; // Checkbox to enable/disable random waypoints

    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private Vector3 currentVelocity;
    private CharacterController characterController;

    private void Start()
    {
        // Find all objects with the "Waypoint" tag
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");

        // Convert GameObject array to Transform array
        waypoints = new Transform[waypointObjects.Length];
        for (int i = 0; i < waypointObjects.Length; i++)
        {
            waypoints[i] = waypointObjects[i].transform;
        }

        // Set a random starting waypoint
        if (waypoints.Length > 0)
        {
            currentWaypointIndex = Random.Range(0, waypoints.Length);
        }

        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned to the agent.");
            return;
        }

        Vector3 directionToWaypoint = waypoints[currentWaypointIndex].position - transform.position;

        if (directionToWaypoint.magnitude < distanceThreshold)
        {
            SetNextWaypoint();
        }

        Vector3 movementDirection = directionToWaypoint.normalized;

        // Add the avoidance force to the movement direction
        movementDirection += AvoidObstacles() * avoidForce;

        // Smoothly change the movement direction
        movementDirection = Vector3.SmoothDamp(movementDirection, movementDirection.normalized, ref currentVelocity, 0.1f);

        // Move the agent using CharacterController
        characterController.SimpleMove(movementDirection * movementSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void SetNextWaypoint()
    {
        //Debug.Log("Agent has reached waypoint " + currentWaypointIndex);

        if (randomWaypoints)
        {
            // Choose a new random waypoint index
            int newWaypointIndex = Random.Range(0, waypoints.Length);

            // Make sure the new waypoint index is different from the current one
            while (newWaypointIndex == currentWaypointIndex)
            {
                newWaypointIndex = Random.Range(0, waypoints.Length);
            }

            currentWaypointIndex = newWaypointIndex;
        }
        else
        {
            // Increment the waypoint index
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private Vector3 AvoidObstacles()
    {
        Vector3 avoidanceDirection = Vector3.zero;

        // Cast two rays in front of the agent
        Ray leftRay = new Ray(transform.position, Quaternion.Euler(0, -30, 0) * transform.forward);
        Ray rightRay = new Ray(transform.position, Quaternion.Euler(0, 30, 0) * transform.forward);

        RaycastHit hit;

        // Visualize the rays in the scene view
        Debug.DrawRay(leftRay.origin, leftRay.direction * rayDistance, Color.red);
        Debug.DrawRay(rightRay.origin, rightRay.direction * rayDistance, Color.red);

        // Check for obstacles
        if (Physics.Raycast(leftRay, out hit, rayDistance))
        {
            if (!hit.collider.CompareTag("Agent")) // Avoid non-agent obstacles
            {
                avoidanceDirection += Vector3.Cross(hit.normal, Vector3.up);
            }
        }

        if (Physics.Raycast(rightRay, out hit, rayDistance))
        {
            if (!hit.collider.CompareTag("Agent")) // Avoid non-agent obstacles
            {
                avoidanceDirection -= Vector3.Cross(hit.normal, Vector3.up);
            }
        }

        return avoidanceDirection;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the avoidance rays
        Ray leftRay = new Ray(transform.position, Quaternion.Euler(0, -30, 0) * transform.forward);
        Ray rightRay = new Ray(transform.position, Quaternion.Euler(0, 30, 0) * transform.forward);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(leftRay.origin, leftRay.direction * rayDistance);
        Gizmos.DrawRay(rightRay.origin, rightRay.direction * rayDistance);

        // Draw the separation sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
