using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyFollow : MonoBehaviour
{
    public enum DroneMode
    {
        Follow
    }

    public DroneMode droneMode = DroneMode.Follow;

    public Transform target;
    public Rigidbody rb;
    public float thrust = 1f;
    public float revthrust = -1f;
    public float rotationSpeed = 5f;
    public float MaxDist = 10f;
    public float MinDist = 5f;
    public float torqueMultiplier = 1f;
    public float heightOffset = 0f;
    public float heightOffsetRandomness = 1f;
    public float heightOffsetChangeSpeed = 0.1f;

    public float forceStrength = 10f;

    public float radiusMin = 5f;
    public float radiusMax = 10f;
    private Vector3 randomTarget;
    public float targetChangeInterval = 5f;
    public float offsetChangeInterval = 0.5f;
    private float offsetTimer = 0f;
    private Vector3 currentOffset = Vector3.zero;
    public float targetPositionRandomnessXZ = 1f;
    public float targetPositionRandomnessY = 1f;
    public float lerpSpeed = 5f;
    public float dampingFactor = 0.5f;
    private float timer = 0f;

    public bool showDebugGizmos = true;

    private Vector3 currentTargetPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (droneMode == DroneMode.Follow)
        {
            ChangeRandomTarget();
            float distance = Vector3.Distance(transform.position, currentTargetPosition);

            // Apply force to maintain height and dampen oscillations
            ApplyHeightAndDampingForce();

            if (distance >= MinDist && distance <= MaxDist)
            {
                RotateTowardsTarget(currentTargetPosition);

                if (distance > MinDist && distance < MaxDist)
                {
                    MoveForward();
                }
                else
                {
                    StopMovement();
                }
            }
            else if (distance < MinDist)
            {
                RotateTowardsTarget(transform.position - currentTargetPosition);
                MoveBackward();
            }
            else if (distance > MaxDist)
            {
                RotateTowardsTarget(currentTargetPosition - transform.position);
                MoveForward();
            }

            timer += Time.deltaTime;

            if (timer > targetChangeInterval)
            {
                timer = 0f;
            }

            // Draw a ray to the original target position
            Debug.DrawRay(transform.position, currentTargetPosition - transform.position, Color.magenta);
            Debug.DrawRay(transform.position, rb.velocity, Color.cyan);
        }
    }

    private void ApplyHeightAndDampingForce()
    {
        // Calculate the target height with the player's height and offset
        float targetHeight = CalculateTargetHeight();

        // Modify currentTargetPosition with the desired height
        currentTargetPosition = new Vector3(currentTargetPosition.x, targetHeight, currentTargetPosition.z);

        // Calculate the force needed to maintain the height and dampen oscillations
        Vector3 force = CalculateForceWithDamping();

        // Apply the force to the Rigidbody
        rb.AddForce(force);
    }

    private float CalculateTargetHeight()
    {
        // Smoothly transition to a new height offset over time
        heightOffset = Mathf.Lerp(heightOffset, CalculateRandomHeightOffset(), heightOffsetChangeSpeed * Time.deltaTime);
        return target.position.y + heightOffset;
    }


    private float CalculateRandomHeightOffset()
    {
        return heightOffset + Random.Range(-heightOffsetRandomness, heightOffsetRandomness);
    }

    private Vector3 CalculateForceWithDamping()
    {
        Vector3 forceDirection = (currentTargetPosition - transform.position);
        Vector3 damping = -rb.velocity * dampingFactor;
        return forceStrength * (forceDirection + damping);
    }

    private void MoveForward()
    {
        Vector3 forwardForce = transform.forward * thrust;
        rb.AddForce(forwardForce);
    }

    private void MoveBackward()
    {
        Vector3 reverseForce = transform.forward * revthrust;
        rb.AddForce(reverseForce);
    }

    private void StopMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        Vector3 torque = Vector3.Cross(transform.forward, targetDirection);
        rb.AddTorque(torque * rotationSpeed * torqueMultiplier);
    }

   
   /*  private void RotateTowardsTarget(Vector3 targetPosition)
{
    Vector3 targetDirection = (targetPosition - transform.position).normalized;
    Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
    
    Quaternion currentRotation = transform.rotation;
    Quaternion targetRotation = Quaternion.Slerp(currentRotation, toRotation, rotationSpeed * Time.deltaTime);

    Vector3 torque = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * torqueMultiplier).eulerAngles;

    // Since the eulerAngles returns values between 0 and 360, we have to convert them to -180 to 180.
    for (int i = 0; i < 3; i++) 
    {
        if (torque[i] > 180) torque[i] -= 360;
    }

    // Apply the torque to the rigidbody
    rb.AddTorque(torque);
} */


    private void ChangeRandomTarget()
{
    Vector3 offsetFromPlayerToTarget = randomTarget - target.position;

    if (offsetFromPlayerToTarget.magnitude > radiusMax || offsetFromPlayerToTarget.magnitude < radiusMin || timer > targetChangeInterval)
    {
        float distance = Random.Range(radiusMin, radiusMax);
        float angle = Random.Range(0, 2 * Mathf.PI);
        randomTarget = target.position + new Vector3(distance * Mathf.Cos(angle), 0, distance * Mathf.Sin(angle));
        timer = 0f;
    }
    else
    {
        randomTarget = Vector3.Lerp(randomTarget, target.position + offsetFromPlayerToTarget, lerpSpeed * Time.deltaTime);
    }

    offsetTimer += Time.deltaTime;
    if(offsetTimer > offsetChangeInterval)
    {
        // Only change the offset at a fixed interval
        currentOffset = new Vector3(Random.Range(-targetPositionRandomnessXZ, targetPositionRandomnessXZ), Random.Range(0, targetPositionRandomnessY), Random.Range(-targetPositionRandomnessXZ, targetPositionRandomnessXZ));
        offsetTimer = 0f;
    }

    // Apply smoothly changing offset to the target's position
    randomTarget += currentOffset;

    currentTargetPosition = Vector3.Lerp(currentTargetPosition, randomTarget, lerpSpeed * Time.deltaTime);
}



    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(target.position, radiusMin);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position, radiusMax);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(randomTarget, 0.5f);
    }
}
