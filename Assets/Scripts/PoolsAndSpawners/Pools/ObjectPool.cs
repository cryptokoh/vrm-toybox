using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPool Instance;

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<string, List<PooledObject>> pooledObjectsDictionary = new Dictionary<string, List<PooledObject>>(); // Initialize here

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            List<PooledObject> pooledObjects = new List<PooledObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);

                PooledObject pooledObject = obj.GetComponent<PooledObject>();
                if (pooledObject != null)
                {
                    pooledObject.Tag = pool.tag;
                    pooledObjects.Add(pooledObject);
                }
            }
            poolDictionary.Add(pool.tag, objectPool);
            pooledObjectsDictionary.Add(pool.tag, pooledObjects);
        }
    }

   public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
{
    if (!poolDictionary.ContainsKey(tag))
    {
        Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
        return null;
    }

    // Find an inactive object to spawn
    GameObject objectToSpawn = null;
    foreach (var pooledObj in pooledObjectsDictionary[tag])
    {
        if (!pooledObj.gameObject.activeInHierarchy)
        {
            objectToSpawn = pooledObj.gameObject;
            break;
        }
    }

    // If no inactive object is found, return null
    if (objectToSpawn == null)
    {
        Debug.LogWarning("No available objects in pool with tag " + tag + ".");
        return null;
    }

    objectToSpawn.transform.position = position;
    objectToSpawn.transform.rotation = rotation;
    objectToSpawn.SetActive(true);

    // Assign the tag to the PooledObject script on the spawned object
    PooledObject pooledObject = objectToSpawn.GetComponent<PooledObject>();
    if (pooledObject != null)
    {
        pooledObject.Tag = tag;
    }

    return objectToSpawn;
}



    public void ReturnToPool(GameObject objectToReturn)
    {
        //Debug.Log("Returning " + objectToReturn.name + " with instance ID " + objectToReturn.GetInstanceID() + " to the pool.");    
        PooledObject returnPooledObject = objectToReturn.GetComponent<PooledObject>();

        if (returnPooledObject != null)
        {
            string tag = returnPooledObject.Tag;

            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return;
            }

            poolDictionary[tag].Enqueue(objectToReturn);
            //pooledObjectsDictionary[tag].Remove(returnPooledObject);

            objectToReturn.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Object does not have PooledObject component.");
        }
    }




    // Get the array of PooledObjects for a specific tag
    public PooledObject[] GetPooledObjects(string tag)
    {
        if (pooledObjectsDictionary.ContainsKey(tag))
        {
            return pooledObjectsDictionary[tag].ToArray();
        }

        return null;
    }
}
