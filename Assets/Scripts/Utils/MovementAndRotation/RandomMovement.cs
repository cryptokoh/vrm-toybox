using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public float speed = 1.0f;
    public float range = 20.0f;
    private float timeOfLastNearCollision;
    private float turningTime;

    public Transform[] randomTargets;
    private int currentTargetIndex = 0;

    private void Start()
    {
        timeOfLastNearCollision = -Mathf.Infinity;
        turningTime = 1.0f;

        if (randomTargets.Length == 0)
        {
            GenerateRandomTargets(5); // Generate 5 random targets
        }
    }

    private void Update()
{
    RaycastHit hit;

    if (Time.time - timeOfLastNearCollision < turningTime)
    {
        transform.eulerAngles += transform.up * Time.deltaTime * 300.0f; // Increase rotation speed
    }
    else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, range))
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        timeOfLastNearCollision = Time.time;
        turningTime = Random.Range(0.5f, 2.0f);
    }
    else
    {
        float distanceFromTarget = Vector3.Distance(transform.position, randomTargets[currentTargetIndex].position);
        if (distanceFromTarget < 1.0f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % randomTargets.Length;
        }

        Vector3 directionToTarget = (randomTargets[currentTargetIndex].position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 10.0f * turningTime * Time.deltaTime); // Increase rotation speed

        transform.position += transform.forward * speed * Time.deltaTime;
    }
}


    private void GenerateRandomTargets(int count)
    {
        randomTargets = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * range;
            randomPosition.y = transform.position.y; // Keep the targets at the same height as the object
            GameObject targetObject = new GameObject("RandomTarget" + i);
            targetObject.transform.position = transform.position + randomPosition;
            randomTargets[i] = targetObject.transform;
        }
    }

    private void OnDrawGizmos()
{
    // Draw random targets as spheres
    Gizmos.color = Color.cyan;
    foreach (var target in randomTargets)
    {
        Gizmos.DrawSphere(target.position, 0.5f);
    }

    // Draw the range sphere
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, range);
}

}
