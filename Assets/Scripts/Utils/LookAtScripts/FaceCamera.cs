using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        // Assign the main camera.
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Make the text face the camera.
        transform.LookAt(mainCamera.transform);
        
        // Then rotate the text 180 degrees around Y-axis
        transform.Rotate(0, 180, 0);
    }
}
