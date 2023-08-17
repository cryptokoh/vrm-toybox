using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDisable : MonoBehaviour
{
    public float radius = 5f;
    public float power = 10f;
    public float damage = 10f; // The amount of damage to apply
    public string[] damageableTags; // The tags of the objects that can take damage

    public ParticleSystemPool particleSystemPool; // Add this

    private HealthBar healthBar; // Add this

    void Awake() 
    {
        healthBar = GetComponent<HealthBar>();
        if (healthBar != null)
        {
            healthBar.OnEnemyDied += HandleEnemyDeath;
        }
    }

    void OnDestroy() // Add this
    {
        if (healthBar != null)
        {
            healthBar.OnEnemyDied -= HandleEnemyDeath;
        }
    }

    void HandleEnemyDeath(Vector3 explosionPosition)
    {
        // Instantiate a particle system at the explosion position
        ParticleSystem explosionParticles = ParticleSystemPool.Instance.Get();
        explosionParticles.transform.position = explosionPosition;
        explosionParticles.Play();

        Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            // Check if the collider's tag is in the array of damageable tags
            if (rb != null && System.Array.IndexOf(damageableTags, hit.tag) >= 0)
            {
                rb.AddExplosionForce(power, explosionPosition, radius, 3.0F);

                


                // Apply damage
                HealthBar healthBar = hit.GetComponent<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.ApplyDamage(damage);
                }
            }
        }
    }
}
