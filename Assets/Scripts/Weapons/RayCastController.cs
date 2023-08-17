using UnityEngine;
using System.Collections;

public class RayCastController : MonoBehaviour
{
    public GameObject player;
    public float distance = 10f;
    public float power = 10f;
    public float damage = 10f;
    public ParticleSystem shockwaveParticles;
    public float drag = 0.5f;
    public string[] hitTags;
    public bool debugEditorGizmos = true;
    public bool debugGameGizmos = true;
    public GameObject impactPrefab;

    private LineRenderer lineRenderer;
    private Vector3 hitPoint;

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.002f;
        lineRenderer.endWidth = 0.002f;
    }

    public void ShootRay()
    {
        if (shockwaveParticles != null)
        {
            shockwaveParticles.Play();
        }

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (System.Array.IndexOf(hitTags, hit.collider.tag) >= 0)
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 hitDirection = (hit.point - player.transform.position).normalized;
                    rb.AddForce(hitDirection * power, ForceMode.Impulse);
                    rb.drag = drag;

                    HealthBar healthBar = hit.collider.GetComponent<HealthBar>();
                    if (healthBar != null)
                    {
                        healthBar.ApplyDamage(damage);
                    }
                }

                hitPoint = hit.point;
                //StartCoroutine(ShowHitPoint());

                if (debugGameGizmos)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.SetPositions(new Vector3[] { Camera.main.transform.position, hit.point });

                    GameObject impact = Instantiate(impactPrefab, hit.point, Quaternion.identity);
                    Destroy(impact, 2f); // Destroy after 2 seconds
                }
            }
        }
    }

    IEnumerator ShowHitPoint()
    {
        yield return new WaitForSeconds(2);
        hitPoint = Vector3.zero;
        lineRenderer.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (debugEditorGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * distance);
            if (hitPoint != Vector3.zero)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hitPoint, 0.05f);
            }
        }
    }
}
