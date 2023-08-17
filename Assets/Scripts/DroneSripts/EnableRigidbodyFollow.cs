using UnityEngine;

public class EnableRigidbodyFollow : MonoBehaviour
{
    public RigidbodyFollow rigidbodyFollow;

    private void Start()
    {
        rigidbodyFollow.enabled = true;
    }
}
