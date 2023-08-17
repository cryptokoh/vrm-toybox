using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAndDisable : MonoBehaviour
{
    public float fadeDuration = 2f;
    private float currentFadeTime = 0f;
    public Scorekeeping scoreKeepingRef;
    private GameObject scoreKeepingObect;

    // Array of meshes to fade out on death
    public SkinnedMeshRenderer[] meshesToFadeOnDeath;

    // Store the initial opacity for each mesh
    private Dictionary<SkinnedMeshRenderer, float> initialOpacities = new Dictionary<SkinnedMeshRenderer, float>();

    public DisableComponentsOnHit disableComponentsScript;

    private GameObject shootObject; // Reference to the "_Shoot" gameObject
    private CharacterSpawnerPool characterSpawnerPool;

    public float minY = -10f; // Set this to whatever Y value you want
    private bool isFadingOut = false;


    public void Start()
    {
        // Find all SkinnedMeshRenderer components on this GameObject and its children
        meshesToFadeOnDeath = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        characterSpawnerPool = FindObjectOfType<CharacterSpawnerPool>();

        // Calculate initial opacities
        foreach (SkinnedMeshRenderer meshRenderer in meshesToFadeOnDeath)
        {
            initialOpacities.Add(meshRenderer, meshRenderer.material.color.a);
        }

        scoreKeepingObect = GameObject.Find("_Scorekeeping"); // Find the "_Scorekeeping" gameObject
        if (scoreKeepingObect != null)
        {
            scoreKeepingRef = scoreKeepingObect.GetComponent<Scorekeeping>(); // Get the script from the "_Scorekeeping" gameObject
        }

        shootObject = GameObject.Find("_Shoot"); // Find the "_Shoot" gameObject
        if (shootObject != null)
        {
            disableComponentsScript = shootObject.GetComponent<DisableComponentsOnHit>(); // Get the DisableComponentsOnHit script from the "_Shoot" gameObject
        }

    }

    void Update()
    {
        //Check Y Put object back in pool if goes below Y
        if (transform.position.y < minY && !isFadingOut)
        {
            StartFadeOut();
        }
    }


    public void StartFadeOut()
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndDisableCoroutine());
        }
    }

    public IEnumerator FadeOutAndDisableCoroutine()
{
    
    while (currentFadeTime < fadeDuration)
    {
        currentFadeTime += Time.deltaTime;

        // Calculate the new opacity based on the current fade time and duration
        float fadePercentage = currentFadeTime / fadeDuration;
        float newOpacity = Mathf.Lerp(1f, 0f, fadePercentage);

        // Update the opacity of each mesh renderer
        foreach (SkinnedMeshRenderer meshRenderer in meshesToFadeOnDeath)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(initialOpacities[meshRenderer], newOpacity, fadePercentage);
            meshRenderer.material.color = color;
        }

        yield return null;
    }

    // Reset currentFadeTime
    currentFadeTime = 0f;

    ObjectPool.Instance.ReturnToPool(gameObject);
    characterSpawnerPool.RemoveSpawnedEnemy(gameObject);
   
    scoreKeepingRef.score++;
    //Debug.Log(scoreKeepingRef.score);
    disableComponentsScript.EnableComponents(gameObject);

    // Reset the flag
    isFadingOut = false;
}

}
