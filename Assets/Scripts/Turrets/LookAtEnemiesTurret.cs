using UnityEngine;

public class LookAtEnemiesTurret : MonoBehaviour
{
    public GameObject[] enemies;
    public int currentEnemyIndex = 0;
    public float minSwitchDelay = 3f; // Minimum duration to stay on each target
    public float maxSwitchDelay = 6f; // Maximum duration to stay on each target
    public float rotationSpeed = 5f; // Speed of rotation towards the target
    public float minShootInterval = 2f; // Minimum time between shots
    public float maxShootInterval = 4f; // Maximum time between shots
    public float yOffset = 1f; // Y offset from the enemy position to aim at a specific point

    public GameObject xAxisObject; // Reference to the object responsible for X-axis rotation
    public GameObject yAxisObject; // Reference to the object responsible for Y-axis rotation
    public float xRotationSpeed = 5f; // Speed of rotation around the X-axis
    public float yRotationSpeed = 5f; // Speed of rotation around the Y-axis

    private float currentSwitchTime = 0f;
    private float currentShootTime = 0f;
    private Quaternion targetRotation;
    public ShootAtTarget shootAtTargetRef;
    public bool isTurret;
    public bool isDrone;
    public turretShooting turretShootingReference;
    private LazerSoundPlayer lazerSoundPlayer;

    void Start()
    {

        lazerSoundPlayer = FindObjectOfType<LazerSoundPlayer>();

        InvokeRepeating(nameof(FindEnemiesWithTag), 0f, 1f); // Call FindEnemiesWithTag every 1 second
        ResetSwitchTime();
        ResetShootTime();
    }

    void LateUpdate()
    {
        if (enemies != null && enemies.Length > 0)
        {
            if (currentEnemyIndex >= enemies.Length)
                currentEnemyIndex = 0;

            GameObject currentEnemy = enemies[currentEnemyIndex];

            if (currentEnemy != null && currentEnemy.activeSelf)
            {
                // Set the current enemy as the target to look at
                Transform targetToLookAt = currentEnemy.transform;

                // Apply the Y offset to the target position
                Vector3 targetPosition = targetToLookAt.position + new Vector3(0f, yOffset, 0f);

                // Calculate the target rotation towards the enemy
                Vector3 targetDirection = targetPosition - transform.position;
                targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

                // Rotate towards the target rotation over time
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Check if the enemy is within shooting distance
                if (shootAtTargetRef != null && Vector3.Distance(transform.position, currentEnemy.transform.position) <= shootAtTargetRef.shootDistance)
                {
                    // Decrease the remaining shoot time
                    currentShootTime -= Time.deltaTime;

                    if (currentShootTime <= 0f)
                    {
                        // Shoot at the current target
                        if (isTurret)
                        {
                            shootAtTargetRef.Shoot(currentEnemy);
                            turretShootingReference.ShootPrefab();

                            // play sound
                            lazerSoundPlayer.PlayRandomSound();
                        }
                        else if (isDrone)
                        {
                            shootAtTargetRef.Shoot(currentEnemy);
                            // play sound
                            lazerSoundPlayer.PlayRandomSound();
                        }

                        // Reset the shoot time with a random delay within the specified range
                        ResetShootTime();
                    }
                }
            }
            else
            {
                // Current enemy is destroyed or inactive, move to the next enemy
                currentEnemyIndex++;
            }

            // Decrease the remaining switch time
            currentSwitchTime -= Time.deltaTime;

            if (currentSwitchTime <= 0f)
            {
                // Move to the next enemy in the list
                currentEnemyIndex++;

                // Reset the switch time with a random delay within the specified range
                ResetSwitchTime();
            }
        }

        // Rotate the turret around the X-axis
        if (xAxisObject != null)
        {
            xAxisObject.transform.Rotate(Vector3.right, xRotationSpeed * Time.deltaTime);
        }

        // Rotate the turret around the Y-axis
        if (yAxisObject != null)
        {
            yAxisObject.transform.Rotate(Vector3.up, yRotationSpeed * Time.deltaTime);
        }
    }

    void ResetSwitchTime()
    {
        currentSwitchTime = Random.Range(minSwitchDelay, maxSwitchDelay);
    }

    void ResetShootTime()
    {
        currentShootTime = Random.Range(minShootInterval, maxShootInterval);
    }

    void FindEnemiesWithTag()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }
}
