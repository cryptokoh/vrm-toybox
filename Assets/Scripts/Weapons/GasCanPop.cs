using UnityEngine;
using System.Collections;

public class GasCanPop : MonoBehaviour
{
    public float explosionForce = 10f;
    public float explosionRadius = 2f;
    public float damageOnPop;
    public float timeToExplode = 5f; // The time it takes for the gas can to explode.

    private float timer; // Timer to keep track of the time since the gas can was thrown.
    private Coroutine destroyCoroutine; // Reference to the destroy coroutine.

        public GameObject particlePrefab; // Reference to the particle prefab

 

    private void OnEnable()
    {
        timer = timeToExplode; // Reset the timer when the gas can is enabled.
    }

    private void OnDisable()
    {
        // Stop the destroy coroutine when the gas can is disabled.
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
    }

    void Update()
    {
        // Decrement the timer and check if it has reached zero.
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (destroyCoroutine == null)
            {
                destroyCoroutine = StartCoroutine(DestroyObjectAndSpawnParticles());
            }
        }
    }

    IEnumerator DestroyObjectAndSpawnParticles()
{
    ApplyForceAndDamageToNearbyObjects();
    ParticleSystem explosionParticles = ParticleSystemPool.Instance.Get(); // Get a particle system from the pool
    explosionParticles.transform.position = transform.position; // Set its position to the current position of the gas can
    explosionParticles.Play(); // Play the particle system
    GasCanPool.Instance.ReturnToPool(gameObject);
    destroyCoroutine = null;
    yield return null;
}



    void SpawnParticles()
    {
        // Get a particle system from the pool
        ParticleSystem explosionParticles = ParticleSystemPool.Instance.Get();

        // Set the particle system's position and start it
        explosionParticles.transform.position = transform.position;
        explosionParticles.Play();
    }

    void ApplyForceAndDamageToNearbyObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                ApplyDamageToEnemy(hit);
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    ApplyForceToRigidBody(rb);
                }
            }
        }
    }

    void ApplyDamageToEnemy(Collider enemy)
    {
        HealthBar healthBar = enemy.GetComponent<HealthBar>();
        if (healthBar != null)
        {
            healthBar.ApplyDamage(damageOnPop);
        }
    }

    void ApplyForceToRigidBody(Rigidbody rb)
{
    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0f, ForceMode.VelocityChange);
}

void OnCollisionEnter(Collision collision)
{
    // Check if the gas can has collided with an enemy
    if (collision.gameObject.CompareTag("Enemy"))
    {
        // If the destroy coroutine is not already running, start it.
        if (destroyCoroutine == null)
        {
            destroyCoroutine = StartCoroutine(DestroyObjectAndSpawnParticles());
        }
    }
}


}
