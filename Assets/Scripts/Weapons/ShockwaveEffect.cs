using UnityEngine;

public class ShockwaveEffect : MonoBehaviour 
{
    public Material shockwaveMaterial; // Assign this in the Inspector.
    public float duration = 1.0f; // Duration of the shockwave.
    public Vector2 center = new Vector2(0.5f, 0.5f); // Center of the shockwave in normalized screen coordinates.

    private float elapsed; // Elapsed time since the shockwave started.

     public int width = 256;
    public int height = 256;
    public float scale = 20f;


    


    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.SetTexture("_MainTex", GenerateTexture());

        // Make the plane a child of the camera.
        transform.parent = Camera.main.transform;
        
        // Position the plane in front of the camera.
        transform.localPosition = new Vector3(0, 0, 1);
        
        // Match the plane's aspect ratio to the camera's aspect ratio.
        transform.localScale = new Vector3(Camera.main.aspect, 1, 1);
        
        // Set the shockwave material on the plane.
        GetComponent<Renderer>().material = shockwaveMaterial;

        // Set the shockwave center.
        shockwaveMaterial.SetVector("_Center", center);
        
        // Trigger the shockwave at the start.
        TriggerShockwave();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                texture.SetPixel(x, y, new Color(sample, sample, sample));
            }
        }

        texture.Apply();
        return texture;
    }

    void Update()
    {
        // Update the elapsed time.
        elapsed += Time.deltaTime;
        
        // Compute the current radius based on the elapsed time and duration.
        float radius = Mathf.Lerp(0, 1, elapsed / duration);
        
        // Set the radius in the material.
        shockwaveMaterial.SetFloat("_Radius", radius);
        
        // If the shockwave has ended, restart it.
        if (elapsed >= duration)
        {
            TriggerShockwave();
        }
    }

    void TriggerShockwave()
    {
        // Enable the effect and reset the elapsed time when a shockwave is triggered.
        shockwaveMaterial.SetFloat("_Distortion", 1);
        elapsed = 0;
        
        // Disable the effect after the duration.
        Invoke("ResetDistortion", duration);
    }

    void ResetDistortion()
    {
        shockwaveMaterial.SetFloat("_Distortion", 0);
    }
}
