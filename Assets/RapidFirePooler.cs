using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFirePooler : MonoBehaviour
{
    public static RapidFirePooler Instance;

    public GameObject pooledObject;
    public int pooledAmount = 10;
    public bool willGrow = true;

    private List<GameObject> pooledObjects;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i] != null && !pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (willGrow)
        {
            Debug.Log("Growing the pool. Instantiating a new pooled object."); // Log here
            GameObject obj = Instantiate(pooledObject);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }
}
