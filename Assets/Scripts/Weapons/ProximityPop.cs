using UnityEngine;
using System.Collections;

public class ProximityPop : MonoBehaviour
{
    public Transform playerTransform;
    public float distanceThreshold = 2f;
    public float delayBeforeDestroy = 0.5f;
    public float explosionForce = 10f;
    public float explosionRadius = 2f;
    public GameObject particlePrefab;
    public float damageOnPop;
    public float upwardForceOnPop = 1f;
    public float kinematicDelay = 0.5f;

    private PlayerHealthBar playerHealthBar;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    private bool isNearPlayer = false;
    private float originalSpeed;
    private HealthBar healthBarScript;
    private CharacterSpawnerPool characterSpawnerPool;
    private FadeOutAndDisable fadeOutAndDisable;
    public DisableComponentsOnHit disableComponentsScript;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        characterSpawnerPool = FindObjectOfType<CharacterSpawnerPool>();
        fadeOutAndDisable = GetComponent<FadeOutAndDisable>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        originalSpeed = navMeshAgent.speed;

        playerHealthBar = playerTransform.GetComponent<PlayerHealthBar>();
        healthBarScript = GetComponent<HealthBar>();
        disableComponentsScript = FindObjectOfType<DisableComponentsOnHit>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= distanceThreshold && !isNearPlayer)
        {
            isNearPlayer = true;
            navMeshAgent.speed = 0f;

            if (playerHealthBar != null)
            {
                playerHealthBar.ApplyDamage(damageOnPop);
            }

            StartCoroutine(DestroyObjectAndSpawnParticles());
        }
    }

    IEnumerator DestroyObjectAndSpawnParticles()
    {
        yield return new WaitForSeconds(delayBeforeDestroy);

        SpawnParticles();
        ApplyForceAndDamageToNearbyObjects();
        ResetObject();
    }

    void SpawnParticles()
    {
        ParticleSystem explosionParticles = ParticleSystemPool.Instance.Get();
        explosionParticles.transform.position = transform.position;
        explosionParticles.Play();
    }

    void ApplyForceAndDamageToNearbyObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            
            if (hit.CompareTag("Enemy"))
            {
                ApplyDamageToEnemy(hit);
                ApplyForceToRigidBody(rb);
            }

        }
    }

    void ApplyDamageToEnemy(Collider enemy)
    {
        HealthBar healthBar = enemy.GetComponent<HealthBar>();
        if (healthBar != null)
        {
            healthBar.ApplyDamage(damageOnPop);
        }
    }

    void ApplyForceToRigidBody(Rigidbody rb)
    {
        FreezeRotation(rb);
        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardForceOnPop, ForceMode.VelocityChange);
        StartCoroutine(ToggleKinematic(rb, kinematicDelay));
        UnfreezeRotation(rb);
        rb.AddForce(Vector3.up * upwardForceOnPop, ForceMode.VelocityChange);
        rb.drag = 1f;
    }

    void ResetObject()
    {
        ObjectPool.Instance.ReturnToPool(gameObject);
        characterSpawnerPool.RemoveSpawnedEnemy(gameObject);
        disableComponentsScript.EnableComponents(gameObject);
        navMeshAgent.speed = 10f;
        isNearPlayer = false;
    }

    void FreezeRotation(Rigidbody rb)
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void UnfreezeRotation(Rigidbody rb)
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    IEnumerator ToggleKinematic(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = true;
        yield return new WaitForSeconds(delay);
        rb.isKinematic = false;
    }
}
