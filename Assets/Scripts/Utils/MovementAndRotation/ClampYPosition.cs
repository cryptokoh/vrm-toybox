using UnityEngine;
using UnityEngine.AI;

public class ClampYPosition : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void LateUpdate()
    {
        // Check if the object is not on the NavMesh
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        {
            // Clamp the y-position to never go below 0
            Vector3 clampedPosition = transform.position;
            clampedPosition.y = Mathf.Max(0f, clampedPosition.y);
            transform.position = clampedPosition;
        }
    }
}
