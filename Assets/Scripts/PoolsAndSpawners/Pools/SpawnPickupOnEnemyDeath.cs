using UnityEngine;

public class SpawnPickupOnEnemyDeath : MonoBehaviour
{
    public Pickupable.PickupType pickupType; // Choose the type of pickup to spawn in the inspector
    public PickupPool objectPool; // Reference to your ObjectPool
    public Vector3 yOffset;
    private HealthBar healthBarScript; // Add reference to HealthBar script

    private void Awake()
    {
        // Get reference to HealthBar script on this object or children
        healthBarScript = GetComponentInChildren<HealthBar>();
    }

    private void OnEnable()
    {
        if (healthBarScript != null)
        {
            healthBarScript.OnEnemyDied += SpawnPickup; // Subscribe to event on this instance's HealthBar script
        }
        else
        {
            Debug.LogWarning("No HealthBar script found on this GameObject or its children.");
        }
        objectPool = FindObjectOfType<PickupPool>();
    }

    private void OnDisable()
    {
        if (healthBarScript != null)
        {
            healthBarScript.OnEnemyDied -= SpawnPickup; // Unsubscribe from event on this instance's HealthBar script
        }
    }

    private void SpawnPickup(Vector3 position)
    {
        // Instantiate a pickup from the ObjectPool
        GameObject pickup = objectPool.SpawnFromPool(pickupType, position + yOffset, Quaternion.identity);
        pickup.SetActive(true);

        // If a pickup was successfully spawned, set its pickupType
        if (pickup != null)
        {
            Pickupable pickupable = pickup.GetComponent<Pickupable>();
            if (pickupable != null)
            {
                pickupable.pickupType = pickupType;
            }
        }
    }
}
