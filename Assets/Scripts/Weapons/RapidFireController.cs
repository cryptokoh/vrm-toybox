using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;
public class RapidFireController : MonoBehaviour
{
    public GameObject player;
    public GameObject impactPrefab;
    public float distance = 10f;
    public float power = 10f;
    public float damage = 10f;
    public float drag = 0.5f;
    public string[] hitTags;
    public bool debugEditorGizmos = true;
    public bool debugGameGizmos = true;

    public Transform shootPoint; // This will be set in the inspector to the player's shooting point.
    public float shootForce = 10f;  // Adjust as necessary.

    private LineRenderer lineRenderer;
    private Vector3 hitPoint;

    // Variation in the rotation of the raycasts
    public float rotationVariation = 0.05f;

    void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.002f;
        lineRenderer.endWidth = 0.002f;
    }

    IEnumerator DeactivateAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (obj != null && obj.activeInHierarchy)
        {
            obj.SetActive(false);
        }
    }


    public void ShootProjectile()
    {
        GameObject projectile = RapidFirePooler.Instance.GetPooledObject();

        if (projectile != null)
        {
            // Set the position to shootPoint and activate
            projectile.transform.position = shootPoint.position;
            projectile.transform.rotation = shootPoint.rotation; // Ensure the projectile's forward aligns with the shootPoint's forward

            projectile.SetActive(true);

            // Optionally deactivate after some time
            StartCoroutine(DeactivateAfterSeconds(projectile, 2f));
        }
    }


}
