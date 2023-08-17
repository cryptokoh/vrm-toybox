using UnityEngine;

public class EnableRigidbodyFollowPlayer : MonoBehaviour
{
    public RigidbodyFollowPlayer rigidbodyFollowPlayer;

    private void Start()
    {
        rigidbodyFollowPlayer.enabled = true;
    }
}
