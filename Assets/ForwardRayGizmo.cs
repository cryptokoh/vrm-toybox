using UnityEngine;

public class ForwardRayGizmo : MonoBehaviour
{

    public enum RayAxis
    {
        Forward,
        Up,
        Right
    }

    public RayAxis rayDirection = RayAxis.Forward; // Default ray direction is Forward
    public float rayLength = 10f; // Length of the ray

    private Vector3 GetRayDirection()
    {
        switch (rayDirection)
        {
            case RayAxis.Forward:
                return transform.forward;
            case RayAxis.Up:
                return transform.up;
            case RayAxis.Right:
                return transform.right;
            default:
                return transform.forward;
        }
    }

    private void OnDrawGizmos()
    {
        // Set the Gizmo color
        Gizmos.color = Color.green;

        // Draw the selected axis ray Gizmo
        Gizmos.DrawRay(transform.position, GetRayDirection() * rayLength);
    }
}
