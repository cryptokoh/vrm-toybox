using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretShooting : MonoBehaviour
{
    public GameObject prefabToSpawn; // The prefab to spawn

    public void ShootPrefab()
    {
        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}