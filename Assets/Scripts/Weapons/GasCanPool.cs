using UnityEngine;
using System.Collections.Generic;

public class GasCanPool : MonoBehaviour
{
    public static GasCanPool Instance { get; private set; }

    public GameObject gasCanPrefab; // Gas can GameObject Prefab to pool
    public int poolSize = 10; // Size of the pool
    public float throwForce = 500f; // Force applied when throwing the gas can
        public Transform gasCanSpawnPoint; // Reference to the gas can spawn point


    private List<GameObject> pool; // The pool of GameObjects

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        pool = new List<GameObject>();

        // Populate the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newGasCan = Instantiate(gasCanPrefab, this.transform);
            newGasCan.gameObject.SetActive(false);
            pool.Add(newGasCan);
        }
    }

    public GameObject Get()
{
    foreach (GameObject gc in pool)
    {
        if (!gc.gameObject.activeInHierarchy)
        {
            Rigidbody rb = gc.GetComponent<Rigidbody>();
            if(rb != null)
            {
                // Make the gas can non-kinematic before activating it.
                rb.isKinematic = false;
            }
            gc.gameObject.SetActive(true);
            return gc;
        }
    }

    // If no inactive gas cans are available, expand the pool
    GameObject newGasCan = Instantiate(gasCanPrefab, this.transform);
    pool.Add(newGasCan);
    return newGasCan;
}


   public void ReturnToPool(GameObject gasCan)
    {
        gasCan.SetActive(false);

        // Reset the position to the spawn point's position
        gasCan.transform.position = gasCanSpawnPoint.position;
        gasCan.transform.rotation = Quaternion.identity;

        Rigidbody rb = gasCan.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Gas Can reset to: " + gasCan.transform.position);
    }




    public void ThrowGasCan(GameObject gasCan, Vector3 direction)
{
    Rigidbody rb = gasCan.GetComponent<Rigidbody>();

    if (rb != null)
    {
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );

        rb.AddForce(direction * throwForce);
        rb.AddTorque(randomTorque * throwForce, ForceMode.Impulse);

        // Debug line for throw position
        Debug.Log("Gas Can thrown from: " + gasCan.transform.position);
    }
    else
    {
        Debug.LogError("No Rigidbody attached to the gas can prefab.");
    }
}


}
