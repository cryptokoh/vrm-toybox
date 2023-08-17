using UnityEngine;

public class WaypointSuccess : MonoBehaviour
{
    public GameObject agentPrefab; // Prefab to spawn
    public bool activateOnCollision = true; // Boolean to control activation

    private RigidbodyWaypointAgent agentScript; // Reference to the RigidbodyWaypointAgent script

    private void Start()
    {
        // Get the RigidbodyWaypointAgent script component from the agent
        agentScript = GetComponent<RigidbodyWaypointAgent>();

        if (agentScript == null)
        {
            Debug.LogError("WaypointSuccess script requires the RigidbodyWaypointAgent script on the same GameObject.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider is the agent and if the script is activated
        if (other.gameObject == agentScript.gameObject && activateOnCollision)
        {
            // Spawn a new agent prefab
            Vector3 spawnOffset = new Vector3(0f, 0f, 1f); // Offset the spawn position by 1 meter in the forward direction
            Instantiate(agentPrefab, transform.position + spawnOffset, transform.rotation);

            // Log waypoint success
            Debug.Log("Waypoint reached! New agent spawned.");

            // Optional: Destroy the current agent
            Destroy(other.gameObject);
        }
    }
}
