using UnityEngine;

public class projectileForward : MonoBehaviour
{
    public float moveForce = 10f;
    public int maxBounces = 2;
    public float damageAmount = 10f;
    public float timeAlive = 5f;
    public string[] tagsToDamage; // Array of tags to consider for damage

    private Rigidbody rb;
    private int bounceCount = 0;
    private Vector3 impactDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, timeAlive);
    }

    private void FixedUpdate()
    {
        Vector3 force = transform.forward * moveForce;
        rb.AddForce(force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        foreach (string tag in tagsToDamage)
        {
            if (collidedObject.CompareTag(tag))
            {
                HealthBar healthBar = collidedObject.GetComponent<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.ApplyDamage(damageAmount);
                }

                PlayerHealthBar playerHealthBar = collidedObject.GetComponent<PlayerHealthBar>();
                if (playerHealthBar != null)
                {
                    playerHealthBar.ApplyDamage(damageAmount);
                }

                break;
            }
        }

        bounceCount++;

        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
        
        impactDirection = collision.GetContact(0).normal.normalized;

        Rigidbody collidedRb = collidedObject.GetComponent<Rigidbody>();
        if (collidedRb != null)
        {
            collidedRb.AddForce(impactDirection * moveForce / 3750, ForceMode.Impulse);
        }
    }
}
