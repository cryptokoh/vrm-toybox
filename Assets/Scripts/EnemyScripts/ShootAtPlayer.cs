using UnityEngine;

public class ShootAtPlayer : MonoBehaviour
{
    public float shootDistance = 10f; // Distance for shooting
    public float yOffset = 1f; // Y offset from the target position
    public float minShootInterval = 2f; // Minimum time between shots
    public float maxShootInterval = 4f; // Maximum time between shots

    private GameObject player;
    private float currentShootTime = 0f; // Timer for tracking time between shots
    public turretShooting turretShootingReference;
    private LazerSoundPlayer lazerSoundPlayer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lazerSoundPlayer = FindObjectOfType<LazerSoundPlayer>();
        ResetShootTime();
    }

    private void Update()
    {
        if (player != null)
        {
            // Check if the player is within shooting distance
            if (Vector3.Distance(transform.position, player.transform.position) <= shootDistance)
            {
                // Decrease the remaining shoot time
                currentShootTime -= Time.deltaTime;

                if (currentShootTime <= 0f)
                {
                    // Shoot at the player and reset the shoot timer
                    Shoot(player);
                    turretShootingReference.ShootPrefab();
                    //lazerSoundPlayer.PlayRandomSound();
                    

                    ResetShootTime();
                }
            }
        }
    }

    public void Shoot(GameObject target)
    {
        // Apply the Y offset to the target position
        Vector3 targetPosition = target.transform.position + new Vector3(0f, yOffset, 0f);

        // Get the direction towards the current target
        Vector3 direction = targetPosition - transform.position;

        // Raycast to check if there's an obstacle between the shooter and the target
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, shootDistance))
        {
            // Check if the ray hits the player
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                // Perform shooting logic here
                // Example: Instantiate a bullet, apply force, etc.
                //Debug.Log("Shooting at the player!");

                // Visualize the ray in the Scene view
                Debug.DrawRay(transform.position, direction, Color.red);

                // ... your other shooting code goes here ...
            }
        }
    }

    void ResetShootTime()
    {
        currentShootTime = Random.Range(minShootInterval, maxShootInterval);
    }
}
