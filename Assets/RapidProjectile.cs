using UnityEngine;
using System.Collections;
public class RapidProjectile : MonoBehaviour
{
    public float moveForce = 10f;
    public int maxBounces = 2;
    public float damageAmount = 10f;
    public float timeAlive = 5f;
    public string[] tagsToDamage;

    private Rigidbody rb;
    private int bounceCount = 0;
    private Vector3 impactDirection;

    private void OnEnable()
    {
        bounceCount = 0;
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        StartCoroutine(DeactivateAfterTimeAlive());
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
            ReturnToPool();
        }

        impactDirection = collision.GetContact(0).normal.normalized;

        Rigidbody collidedRb = collidedObject.GetComponent<Rigidbody>();
        if (collidedRb != null)
        {
            collidedRb.AddForce(impactDirection * moveForce / 3750, ForceMode.Impulse);
        }
    }

    private void ReturnToPool()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DeactivateAfterTimeAlive()
    {
        yield return new WaitForSeconds(timeAlive);
        ReturnToPool();
    }


}
