using UnityEngine;

public class EmissionFade : MonoBehaviour
{
    public int materialIndex = 0;
    public Color color1;
    public Color color2;
    public float fadeDuration = 2f;

    public Material material;
    private bool isFading = true;
    private float t = 0f;
    //private bool forward = true;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && materialIndex < renderer.materials.Length)
        {
            material = renderer.materials[materialIndex];
            material.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogWarning("Material index is out of range or no Renderer component found!");
        }
    }

    private void Update()
{
    if (isFading)
    {
        t += Time.deltaTime / fadeDuration;
        float lerpFactor = Mathf.PingPong(t, 1f);

        material.SetColor("_EmissionColor", Color.Lerp(color1, color2, lerpFactor));
    }
}



    public void StartFade()
    {
        isFading = true;
        t = 0f;
        //forward = true;
    }
}
