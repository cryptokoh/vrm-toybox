using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public float maxHealth = 100f;

    [SerializeField]
    public float currentHealth;

    [SerializeField]
    private Slider healthBarSlider;

    public SkinnedMeshRenderer[] meshesToFadeOnDeath; // Array of meshes to fade out on death

    public Color flashColor = Color.red;
    public float flashDuration = 1f;

    private Coroutine flashCoroutine;

    private Dictionary<SkinnedMeshRenderer, Material> originalMaterials = new Dictionary<SkinnedMeshRenderer, Material>();

    private Color originalFillColor;

    public Image flashImage;

    private Coroutine flashImageCoroutine;



    //private GameObject shootObject; // Reference to the "_Shoot" gameObject

    void Start()
    {
        currentHealth = maxHealth;
        healthBarSlider.minValue = 0;
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = maxHealth;
        healthBarSlider.handleRect.gameObject.SetActive(false);

         meshesToFadeOnDeath = GetComponentsInChildren<SkinnedMeshRenderer>();
        // The above line will ensure all SkinnedMeshRenderer components in children are included in the array.

        Image fillImage = healthBarSlider.fillRect.GetComponent<Image>();
        originalFillColor = fillImage.color;
    }

    void Update()
    {
        healthBarSlider.value = currentHealth;
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Make sure currentHealth doesn't go below 0 or above maxHealth
        float fillValue = currentHealth / maxHealth;
        //healthBarSlider.value = fillValue * maxHealth; // Multiply fillValue by maxHealth to get the correct slider value

        if (currentHealth <= 0)
        {
            // Disable the health bar GameObject
            //healthBarSlider.gameObject.SetActive(false);

            // Start Fadeout
            StartCoroutine(FadeOutAndDisable());
        }
         FlashMeshOnHit();
         flashCoroutine = StartCoroutine(FlashHealthBar());
         
         if (flashImageCoroutine != null)
            {
                StopCoroutine(flashImageCoroutine);
            }
            flashImageCoroutine = StartCoroutine(FlashImage());
            }

    private IEnumerator FadeOutAndDisable()
{
    float fadeDuration = 2f;
    float currentFadeTime = 0f;

    
    Color originalColor = flashImage.color;

    // Set the initial alpha value to a higher value
    originalColor.a = 0.8f; // You can adjust this value to control the intensity of the flash

    // Store the initial opacity for each mesh
    Dictionary<SkinnedMeshRenderer, float> initialOpacities = new Dictionary<SkinnedMeshRenderer, float>();
    foreach (SkinnedMeshRenderer meshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
    {
        initialOpacities.Add(meshRenderer, meshRenderer.material.color.a);
    }

    // Fade out the meshes over the specified duration
    while (currentFadeTime < fadeDuration)
    {
        currentFadeTime += Time.deltaTime;

        // Calculate the new opacity based on the current fade time and duration
        float fadePercentage = currentFadeTime / fadeDuration;
        float newOpacity = Mathf.Lerp(1f, 0f, fadePercentage);

        // Update the opacity of each mesh renderer
        foreach (SkinnedMeshRenderer meshRenderer in initialOpacities.Keys)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(initialOpacities[meshRenderer], newOpacity, fadePercentage);
            meshRenderer.material.color = color;
        }

        yield return null;
    }

    // Reset the health bar color to the original one after flashing
    flashImage.color = originalColor;
    flashCoroutine = null;
}


   private IEnumerator FlashHealthBar()
    {
        float elapsedTime = 0f;
        Image fillImage = healthBarSlider.fillRect.GetComponent<Image>();

        // Store the original material of the fill image
        Material originalMaterial = fillImage.material;

        Material flashMaterial = new Material(originalMaterial);
        fillImage.material = flashMaterial;

        Color flashColorWithAlpha = new Color(flashColor.r, flashColor.g, flashColor.b, 0.5f); // Adjust the alpha value (0.5f) as needed

        while (elapsedTime < flashDuration)
        {
            // Calculate the alpha value for the flashing effect using Mathf.SmoothStep
            float alpha = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(elapsedTime * 5f, 1f));
            flashMaterial.color = Color.Lerp(originalFillColor, flashColorWithAlpha, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the health bar goes back to its original color exactly
        flashMaterial.color = originalFillColor;

        // Reset the health bar material to the original one after flashing
        fillImage.material = originalMaterial;
        flashCoroutine = null;
    }

    private IEnumerator FlashImage()
{
    // Set the flash color with alpha (start at 0.9f and fade to 0)
    Color flashColorWithAlpha = new Color(flashColor.r, flashColor.g, flashColor.b, 0.9f);

    // Start by setting the color of flashImage to the flash color
    flashImage.color = flashColorWithAlpha;

    float elapsedTime = 0f;

    // Fade out the flash over the duration of flashDuration
    while (elapsedTime < flashDuration)
    {
        // Calculate the new alpha value for the flash color
        float newAlpha = Mathf.Lerp(flashColorWithAlpha.a, 0f, elapsedTime / flashDuration);

        // Assign the new color to the flashImage
        flashImage.color = new Color(flashColorWithAlpha.r, flashColorWithAlpha.g, flashColorWithAlpha.b, newAlpha);

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Reset the flash image color to the original one after flashing
    flashImage.color = new Color(flashColorWithAlpha.r, flashColorWithAlpha.g, flashColorWithAlpha.b, 0f);
}





    
    private void FlashMeshOnHit()
    {
        foreach (SkinnedMeshRenderer meshRenderer in meshesToFadeOnDeath)
        {
            StartCoroutine(FlashMesh(meshRenderer));
        }
    }
    
   private IEnumerator FlashMesh(SkinnedMeshRenderer meshRenderer)
{
    if (!originalMaterials.TryGetValue(meshRenderer, out Material originalMaterial))
    {
        // Save the original material for the mesh renderer if it hasn't been saved before
        originalMaterial = meshRenderer.material;
        originalMaterials[meshRenderer] = originalMaterial;
    }

    Material flashMaterial = new Material(originalMaterial);
    meshRenderer.material = flashMaterial;

    Color flashColorWithAlpha = new Color(flashColor.r, flashColor.g, flashColor.b, 0.5f); // Adjust the alpha value (0.5f) as needed

    // Start by setting the color of the mesh to the flash color
    flashMaterial.color = flashColorWithAlpha;

    float elapsedTime = 0f;

    // Fade out the flash over the duration of flashDuration
    while (elapsedTime < flashDuration)
    {
        // Calculate the new color value for the flash color
        Color newColor = Color.Lerp(flashColorWithAlpha, originalMaterial.color, elapsedTime / flashDuration);

        // Assign the new color to the mesh
        flashMaterial.color = newColor;

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Reset the mesh renderer's material to the original one after flashing
    meshRenderer.material = originalMaterial;
}




}
