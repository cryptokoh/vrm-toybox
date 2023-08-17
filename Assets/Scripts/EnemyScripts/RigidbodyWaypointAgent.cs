using UnityEngine;
using System.Linq;
using System.Collections;

public class RigidbodyWaypointAgent : MonoBehaviour
{
    public float speed;
    public float vaultDetectionDistance = 2.0f;
    public Animator anim;

    public Transform[] waypoints;
    private int currentWaypoint = 0;

    private enum State { Walk, Vault }
    private State currentState = State.Walk;

    private Rigidbody rb;
    private GameObject vaultObject;
    private Vector3 vaultStartPosition;
    private Vector3 vaultEndPosition;

     public   float vaultAnimationDuration = .5f; // Adjust this value to match the duration of the vaulting animation.
    public float clearanceHeightMultiplier = 1;

    public float initialJumpSpeed  = 5f; // Adjust this value to control the initial upward force during vaulting.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Walk:
                Walk();
                anim.SetFloat("Speed", 5.0f);
                CheckForObstacles();
                break;
            case State.Vault:
                // Play the vault animation.
                anim.SetBool("isVaulting", true);
                break;
        }
    }

    void Walk()
    {
        Vector3 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, speed * Time.deltaTime);
        }
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void CheckForObstacles()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(ray, 0.5f, out var hit, vaultDetectionDistance) && hit.collider.gameObject.CompareTag("Vault"))
        {
            currentState = State.Vault;
            vaultObject = hit.collider.gameObject;
            StartCoroutine(VaultOverObstacle(hit.point, vaultObject));
        }
    }

 IEnumerator VaultOverObstacle(Vector3 hitPoint, GameObject obstacle)
{
    currentState = State.Vault;

    // Record the start position for the vault.
    vaultStartPosition = transform.position;

    // Calculate the height and width of the vault.
    float height = obstacle.GetComponent<Collider>().bounds.size.y;
    float width = obstacle.GetComponent<Collider>().bounds.size.x; // Use width instead of depth.

    // Calculate the desired clearance height during the vault.
    float clearanceHeight = height * clearanceHeightMultiplier; // Adjust this value as needed for the desired clearance height.

    // Calculate the up position and down position during the vault.
    Vector3 upPosition = hitPoint + Vector3.up * clearanceHeight;
    Vector3 downPosition = upPosition + transform.forward * width; // Use width instead of depth.

    // Calculate the end position of the vaulting trajectory.
    Vector3 vaultEndDirection = (downPosition - vaultStartPosition).normalized;
    float vaultEndDistance = Vector3.Distance(vaultStartPosition, downPosition);
    vaultEndPosition = downPosition + vaultEndDirection * vaultEndDistance;

    // Disable physics during the animation.
    rb.isKinematic = true;
    rb.velocity = Vector3.zero;

    float startTime = Time.time;
    float endTime = startTime + vaultAnimationDuration;

    // Apply the initial upward force.
    float initialJumpSpeed = 3.0f; // Adjust this value to control the initial height of the vault.

    // Play the vault animation.
    anim.SetBool("isVaulting", true);

    while (Time.time < endTime)
    {
        // Interpolate the AI agent's position from start to end over the duration of the vaulting animation.
        float t = (Time.time - startTime) / vaultAnimationDuration;

        // Apply the initial upward force to simulate the jump.
        float currentVerticalSpeed = initialJumpSpeed - (Physics.gravity.y * t);
        vaultStartPosition.y += currentVerticalSpeed * Time.deltaTime;

        transform.position = Vector3.Lerp(vaultStartPosition, vaultEndPosition, t);

        // Raycast downwards to detect the ground's height.
        RaycastHit groundHit;
        Vector3 raycastOrigin = transform.position + Vector3.up * clearanceHeight;
        if (Physics.Raycast(raycastOrigin, Vector3.down, out groundHit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            float currentHeight = groundHit.point.y + clearanceHeight;
            rb.MovePosition(new Vector3(transform.position.x, currentHeight, transform.position.z));
        }

        yield return null;
    }

    // Enable physics after the animation.
    rb.isKinematic = false;

    anim.SetBool("isVaulting", false);

    currentState = State.Walk;
    vaultObject = null;
}







   void OnDrawGizmos()
{
    // Draw a wire sphere to visualize the detection range for vaulting.
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, vaultDetectionDistance);

    // Draw a line from the AI agent to the vaultObject when detected.
    if (currentState == State.Vault && vaultObject != null)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, vaultObject.transform.position);
    }

    // Draw the trajectory of the vault when the AI is vaulting.
    if (currentState == State.Vault && anim.GetBool("isVaulting"))
    {
        Gizmos.color = Color.cyan;

        // Calculate the height and width of the vault.
        float height = vaultObject.GetComponent<Collider>().bounds.size.y;
        float width = vaultObject.GetComponent<Collider>().bounds.size.x; // Use width instead of depth.

        // Calculate the up position and down position during the vault.
        Vector3 upPosition = vaultStartPosition + Vector3.up * height;
        Vector3 downPosition = upPosition + transform.forward * width; // Use width instead of depth.

        // Draw lines for the vault trajectory.
        Gizmos.DrawLine(vaultStartPosition, upPosition);
        Gizmos.DrawLine(upPosition, downPosition);
        Gizmos.DrawLine(downPosition, vaultEndPosition);

        // Draw a line for the ground level during the vault.
        Vector3 groundLevel = new Vector3(vaultStartPosition.x, vaultStartPosition.y - clearanceHeightMultiplier * height, vaultStartPosition.z);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(vaultStartPosition, groundLevel);
        Gizmos.DrawLine(vaultEndPosition, groundLevel);
    }
}

}
