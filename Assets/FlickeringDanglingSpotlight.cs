using UnityEngine;

public class FlickeringDanglingSpotlight : MonoBehaviour
{
    // Set these values to adjust the speed and amount of flickering.
    public float flickerSpeed = 0.1f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;

    // Set these values to adjust the speed and amount of rotation.
    public float rotationSpeed = 1.0f;
    public float maxRotation = 5.0f;

    private Light spotLight;
    private Quaternion originalRotation;

    void Start()
    {
        // Get the light component.
        spotLight = GetComponent<Light>();

        // Store the original rotation.
        originalRotation = transform.rotation;
    }

    void Update()
    {
        // Change the light intensity.
        FlickerLight();

        // Rotate the light.
        RotateLight();
    }

    private void FlickerLight()
    {
        // Create a random flicker speed and apply it to the light intensity.
        float flicker = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f));
        spotLight.intensity = flicker;
    }

    private void RotateLight()
    {
        // Create a random rotation and apply it.
        float rotationX = maxRotation * Mathf.Sin(Time.time * rotationSpeed);
        float rotationY = maxRotation * Mathf.Sin(Time.time * rotationSpeed * 0.7f);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        transform.rotation = originalRotation * rotation;
    }
}
