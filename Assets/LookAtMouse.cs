using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    public float horizontalSensitivity = 10.0f;
    public float verticalSensitivity = 10.0f;
    public float horizontalLimit = 30.0f;
    public float verticalLimit = 30.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity;

        yaw += mouseX;
        pitch -= mouseY;

        yaw = ClampAngle(yaw, -horizontalLimit, horizontalLimit);
        pitch = ClampAngle(pitch, -verticalLimit, verticalLimit);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = rotation;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
