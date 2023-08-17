using System.Collections;
using UnityEngine;

public class AttractToPlayer : MonoBehaviour
{
    public float attractRadius = 5f; 
    public float attractSpeed = 1f;

    private Transform playerTransform;
    private Rigidbody objectRigidbody;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        objectRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(AttractCoroutine());
    }

    private void OnDisable()
    {
        StopCoroutine(AttractCoroutine());
    }

    private IEnumerator AttractCoroutine()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

            if (distanceToPlayer <= attractRadius)
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                float forceMagnitude = attractSpeed * (1 - (distanceToPlayer / attractRadius));
                Vector3 newVelocity = Vector3.Lerp(objectRigidbody.velocity, direction * forceMagnitude, Time.deltaTime);
                objectRigidbody.velocity = newVelocity;
            }

            yield return null;
        }
    }

    // Draw a gizmo in the editor to visualize the attract radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attractRadius);
    }
}
