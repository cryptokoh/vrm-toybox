using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed = 5f; // Speed of rotation towards the player
    public float yOffset = 1f; // Y offset from the player position to aim at a specific point

    private Quaternion targetRotation;

    void Start()
    {
        // Assign the player GameObject reference here
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Set the player as the target to look at
            Transform targetToLookAt = player.transform;

            // Apply the Y offset to the target position
            Vector3 targetPosition = targetToLookAt.position + new Vector3(0f, yOffset, 0f);

            // Calculate the target rotation towards the player
            Vector3 targetDirection = targetPosition - transform.position;
            targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            // Rotate towards the target rotation over time
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
