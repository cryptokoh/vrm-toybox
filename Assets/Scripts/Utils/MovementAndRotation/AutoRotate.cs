using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotationSpeed = 50f; // Degrees per second

    // Update is called once per frame
    void LateUpdate()
    {
        // Rotate the object around its Y-axis at rotationSpeed degrees per second
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
