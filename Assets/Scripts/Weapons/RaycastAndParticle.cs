using UnityEngine;

public class RaycastAndParticle : MonoBehaviour
{
    public ParticleSystem particleSystemPrefab;
    public GameObject hitMarkerPrefab;
    public float hitMarkerSize = 0.1f;
    public LayerMask layerMask;
    public float raycastDistance = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Instantiate and play the particle system
            ParticleSystem newParticleSystem = Instantiate(particleSystemPrefab, transform.position, transform.rotation);
            newParticleSystem.Play();

            // Perform the raycast
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
            {
                // Output the location of the hit in the console
                Debug.Log($"Raycast hit: {hit.point}");

                // Create a small red cube at the hit location
                GameObject hitMarker = Instantiate(hitMarkerPrefab, hit.point, Quaternion.identity);
                hitMarker.transform.localScale = Vector3.one * hitMarkerSize;
                hitMarker.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }
}
