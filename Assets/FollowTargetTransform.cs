using UnityEngine;

public class FollowTargetTransform : MonoBehaviour
{
    public Transform targetTransform; // Assign the target transform directly in the Inspector

    private void FixedUpdate()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }
}
