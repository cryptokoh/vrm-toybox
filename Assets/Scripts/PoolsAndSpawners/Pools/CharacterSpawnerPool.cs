using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterPrefab
{
    public GameObject prefab;
    public int count;
    public bool spawnable = true;
    public Transform spawnTransform; // Add Transform field here
    public float spawnRadius = 10f;  // Add Radius field here
}
    public class CharacterSpawnerPool : MonoBehaviour
{
    public CharacterPrefab[] characterPrefabs;
    public float spawnRadius = 10f;

    public List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        SpawnWave();
        //HealthBar.OnEnemyDeath += RemoveSpawnedEnemy;
    }

    void Update()
    {
        if (spawnedEnemies.Count == 0)
        {
            //HealthBar.OnEnemyDeath -= RemoveSpawnedEnemy;
            //Debug.Log("All enemies killed. Respawning...");
            SpawnWave();
        }
    }

        public void SpawnWave()
        {
            spawnedEnemies.Clear();

            foreach (var characterPrefab in characterPrefabs)
            {
                if (!characterPrefab.spawnable)
                    continue;

                for (int i = 0; i < characterPrefab.count; i++)
                {
                    Vector3 spawnPosition = GetRandomSpawnPosition(characterPrefab.spawnTransform.position, characterPrefab.spawnRadius);
                    GameObject spawnedCharacter = ObjectPool.Instance.SpawnFromPool(characterPrefab.prefab.name, spawnPosition, Quaternion.identity);
                    spawnedEnemies.Add(spawnedCharacter);
                }
            }
        }

        Vector3 GetRandomSpawnPosition(Vector3 spawnCenter, float radius)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * radius;
            randomOffset.y = 0f; // Ensure enemies spawn on the ground
            Vector3 spawnPosition = spawnCenter + randomOffset;
            return spawnPosition;
        }

        public void SetSpawnableStatus(GameObject prefab, bool status)
    {
        foreach (var characterPrefab in characterPrefabs)
        {
            if (characterPrefab.prefab == prefab)
            {
                characterPrefab.spawnable = status;
                break;
            }
        }
    }


    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0f; // Ensure enemies spawn on the ground
        Vector3 spawnPosition = transform.position + randomOffset;
        return spawnPosition;
    }

    public void RemoveSpawnedEnemy(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
            //Debug.Log("Enemy " + enemy.name + " removed. Current enemy count: " + spawnedEnemies.Count);
        }
    }
}
