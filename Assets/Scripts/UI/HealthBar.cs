using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBar : MonoBehaviour
{
    //public static event Action<GameObject> OnEnemyDeath;

    public delegate void EnemyDiedEventHandler(Vector3 position);
    public event EnemyDiedEventHandler OnEnemyDied; // Make this non-static

    public GameObject agent;
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;  // Add this field

    [SerializeField]
    private Slider healthBarSlider;

    public SkinnedMeshRenderer[] meshesToFadeOnDeath; // Array of meshes to fade out on death

    private Dictionary<SkinnedMeshRenderer, Material> originalMaterials = new Dictionary<SkinnedMeshRenderer, Material>();
        private List<MeshMaterialSet> meshMaterialSets = new List<MeshMaterialSet>();


    public DisableComponentsOnHit disableComponentsScript;

    private GameObject shootObject; 

    private FadeOutAndDisable fadeOutAndDisable; 
    private ObjectPool objectPool;

    public Color flashColor = Color.red;
    public float flashDuration = 1f;
    private Coroutine flashCoroutine;


    void OnEnable(){
        //healthBarSlider.gameObject.SetActive(true);
        healthBarSlider.value = maxHealth;
        isDead = false;
        originalMaterials.Clear();
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBarSlider.minValue = 0;
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = maxHealth;
        healthBarSlider.handleRect.gameObject.SetActive(false);

        shootObject = GameObject.Find("_Shoot");
        if (shootObject != null)
        {
            disableComponentsScript = shootObject.GetComponent<DisableComponentsOnHit>();
        }

        fadeOutAndDisable = GetComponent<FadeOutAndDisable>();
        objectPool = FindObjectOfType<ObjectPool>();

        meshesToFadeOnDeath = GetComponentsInChildren<SkinnedMeshRenderer>();
        
    }

    void Update()
    {
        healthBarSlider.transform.LookAt(healthBarSlider.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    public void ApplyDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        float fillValue = currentHealth / maxHealth;
        healthBarSlider.value = fillValue * maxHealth;

        if (currentHealth <= 0)
        {
    
            isDead = true;  // The enemy is now dead
             

            if (disableComponentsScript != null)
            {
                disableComponentsScript.DisableComponents(agent);
            }

            if (gameObject.activeSelf)
            {
                OnEnemyDied?.Invoke(transform.position);

                if (gameObject.activeInHierarchy) // Check if GameObject is active before starting Coroutine
            {
                StartCoroutine(fadeOutAndDisable.FadeOutAndDisableCoroutine());
            }
                
            }

            
        }

        FlashMeshOnHit();
    }


    private IEnumerator FadeOutAndDisable()
    {
        float fadeDuration = 2f;
        float currentFadeTime = 0f;

        // Store the initial opacity for the health bar slider
        float initialOpacity = healthBarSlider.GetComponent<CanvasGroup>().alpha;

        // Fade out the health bar over the specified duration
        while (currentFadeTime < fadeDuration)
        {
            currentFadeTime += Time.deltaTime;

            // Calculate the new opacity based on the current fade time and duration
            float fadePercentage = currentFadeTime / fadeDuration;
            float newOpacity = Mathf.Lerp(initialOpacity, 0f, fadePercentage);

            // Update the opacity of the health bar slider
            //healthBarSlider.GetComponent<CanvasGroup>().alpha = newOpacity;

            yield return null;
        }

        // Disable the health bar GameObject after fading out
        healthBarSlider.gameObject.SetActive(false);

        // Game over or respawn logic can be added here
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

        float elapsedTime = 0f;
        float flashSpeed = 5f; // Adjust this value to control the speed of the flash effect

        while (elapsedTime < flashDuration)
        {
            // Calculate the alpha value for the flashing effect using Mathf.SmoothStep
            float alpha = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(elapsedTime * flashSpeed, 1f));
            flashMaterial.color = Color.Lerp(originalMaterial.color, flashColorWithAlpha, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the mesh renderer's material to the original one after flashing
        meshRenderer.material = originalMaterial;
    }




}

public class MeshMaterialSet
{
    public Material originalMaterial;
    public Material flashMaterial;

    public MeshMaterialSet(Material original, Material flash)
    {
        originalMaterial = original;
        flashMaterial = flash;
    }
}

