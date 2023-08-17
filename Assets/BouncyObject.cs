using System.Collections;
using UnityEngine;

public class BouncyObject : MonoBehaviour
{
    public float bounceForce = 5.0f;

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();

        if(rb != null && collision.relativeVelocity.y <= 0f)
        {
            StartCoroutine(ApplyBounceForce(rb));
        }
    }

    IEnumerator ApplyBounceForce(Rigidbody rb)
    {
        yield return new WaitForEndOfFrame();
        
        if (rb != null) // Check if the Rigidbody still exists
        {
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
    }

    

}
