using UnityEngine;

public class HeadController : MonoBehaviour
{
    public Transform playerCamera;
    public Transform headTransform;
    //private Animator anim;
    private Vector3 lastDirection;

    void Start()
    {
        //anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        // Calculate the direction to look at based on the camera's position
        Vector3 lookDirection = playerCamera.transform.forward;
        
        // Project the direction onto the horizontal plane
        lookDirection.y = 0f;
        
        // If the look direction is not zero, look towards it
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        Repositioning();
    }

    public void Repositioning()
{
    // Get the camera's forward direction
    Vector3 cameraForward = playerCamera.forward;

    // Calculate the horizontal direction by projecting the camera's forward direction onto the horizontal plane
    Vector3 horizontalDirection = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

    // Rotate the head bone to look in the horizontal direction
    Transform headBone = headTransform;
    if (headBone != null)
    {
        headBone.rotation = Quaternion.LookRotation(horizontalDirection);

        // Calculate the vertical angle based on the camera's rotation
        float verticalAngle = playerCamera.transform.eulerAngles.x;
        if (verticalAngle > 180)
        {
            verticalAngle -= 360;
        }

        // Apply the vertical rotation to the head bone
        headBone.Rotate(Vector3.right, verticalAngle);
    }
}





}
