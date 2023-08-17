using UnityEngine;

public class ShootAtTarget : MonoBehaviour
{
    public float shootDistance = 10f; // Distance for shooting
    public float yOffset; // Y offset from the target position
    public LookAtEnemies lookAtEnemies;

    private void Update()
    {
        if (lookAtEnemies != null && lookAtEnemies.enemies != null && lookAtEnemies.enemies.Length > 0)
        {
            // Check if the currentEnemyIndex is within the valid range of the enemies array
            if (lookAtEnemies.currentEnemyIndex >= 0 && lookAtEnemies.currentEnemyIndex < lookAtEnemies.enemies.Length)
            {
                GameObject currentEnemy = lookAtEnemies.enemies[lookAtEnemies.currentEnemyIndex];

                if (currentEnemy != null && currentEnemy.activeSelf)
                {
                    // Check if the enemy is within shooting distance
                    if (Vector3.Distance(transform.position, currentEnemy.transform.position) <= shootDistance)
                    {
                        // Shoot at the current target
                        Shoot(currentEnemy);
                    }
                }
            }
        }
    }

    public void Shoot(GameObject enemy)
    {
        // Apply the Y offset to the target position
        Vector3 targetPosition = enemy.transform.position + new Vector3(0f, yOffset, 0f);

        // Get the direction towards the current target
        Vector3 direction = targetPosition - transform.position;

        // Raycast to check if there's an obstacle between the shooter and the target
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, shootDistance))
        {
            // Check if the ray hits the target
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                // Perform shooting logic here
                // Example: Instantiate a bullet, apply force, etc.
                //Debug.Log("Shooting at the target!"+ hit.collider.gameObject.name);

                // Visualize the ray in the Scene view
                Debug.DrawRay(transform.position, direction, Color.red);

                // ... your other shooting code goes here ...
            }
        }
    }
}
