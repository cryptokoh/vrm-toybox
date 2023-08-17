using System.Collections;
using UnityEngine;

public class MaterialFade : MonoBehaviour
{
    public float duration = 2f; // Duration in seconds
    private Material[] materials;
    
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materials = renderer.materials;
        }

        if (materials == null || materials.Length == 0)
        {
            Debug.LogError("Material component missing from this gameobject. Please add one.");
        }
        else
        {
            foreach (Material material in materials)
            {
                StartCoroutine(FadeMaterialToZeroAlpha(duration, material));
            }
        }
    }

    public IEnumerator FadeMaterialToZeroAlpha(float t, Material material)
    {
        material.color = new Color(material.color.r, material.color.g, material.color.b, 1);
        while (material.color.a > 0.0f)
        {
            material.color = new Color(material.color.r, material.color.g, material.color.b, material.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
