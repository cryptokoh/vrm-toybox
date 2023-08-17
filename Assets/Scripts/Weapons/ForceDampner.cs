using UnityEngine;

public class ForceDampner : MonoBehaviour
{
    public float dampeningFactor = 0.95f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb.velocity != Vector3.zero)
        {
            rb.velocity *= dampeningFactor;
        }
    }
}
