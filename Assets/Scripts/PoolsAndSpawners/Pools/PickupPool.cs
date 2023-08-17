using UnityEngine;
using System.Collections.Generic;

public class PickupPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public Pickupable.PickupType tag;
        public GameObject prefab;
        public int size;
    }

    public static PickupPool Instance;

    public int maxPoolSize = 50;

    public List<Pool> pools;
    public Dictionary<Pickupable.PickupType, Queue<GameObject>> poolDictionary;
    public Dictionary<Pickupable.PickupType, List<GameObject>> activePickupsDictionary;
    public Dictionary<GameObject, float> pickupSpawnTimes;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<Pickupable.PickupType, Queue<GameObject>>();
        activePickupsDictionary = new Dictionary<Pickupable.PickupType, List<GameObject>>();
        pickupSpawnTimes = new Dictionary<GameObject, float>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
            activePickupsDictionary.Add(pool.tag, new List<GameObject>());
        }
    }

    public GameObject SpawnFromPool(Pickupable.PickupType tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        if (activePickupsDictionary[tag].Count >= maxPoolSize)
        {
            GameObject oldestActivePickup = GetOldestPickup(tag);
            if (oldestActivePickup != null)
            {
                ReturnToPool(oldestActivePickup);
            }
            else
            {
                Debug.LogWarning("All pickups of tag " + tag + " are already inactive.");
                return null;
            }
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        activePickupsDictionary[tag].Add(objectToSpawn);
        pickupSpawnTimes[objectToSpawn] = Time.time;
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        //Debug.Log("Returning " + objectToReturn.name + " to the pool.");    
        Pickupable returnPickupObject = objectToReturn.GetComponent<Pickupable>();

        if (returnPickupObject != null)
        {
            Pickupable.PickupType tag = returnPickupObject.pickupType;

            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return;
            }

            poolDictionary[tag].Enqueue(objectToReturn);
            activePickupsDictionary[tag].Remove(objectToReturn);

            objectToReturn.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Object does not have Pickupable component.");
        }
    }

    private GameObject GetOldestPickup(Pickupable.PickupType tag)
    {
        GameObject oldest = null;
        float oldestTime = float.MaxValue;

        foreach (var pickup in activePickupsDictionary[tag])
        {
            float spawnTime = pickupSpawnTimes[pickup];
            if (spawnTime < oldestTime)
            {
                oldestTime = spawnTime;
                oldest = pickup;
            }
        }

        return oldest;
    }
}
