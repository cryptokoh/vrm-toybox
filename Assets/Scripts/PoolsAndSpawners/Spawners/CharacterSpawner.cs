using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;   // Array of character prefabs
    public float[] characterWeights;        // Array of weights corresponding to each character prefab
    public int totalEnemies = 10;           // Total number of enemies to spawn per wave
    public float spawnRadius = 10f;         // Radius within which enemies can spawn

    private List<GameObject> spawnedEnemies = new List<GameObject>();  // List to keep track of spawned enemies

    void Start()
    {
        SpawnWave();
    }

    void Update()
{
    // Update the list of spawned enemies
    spawnedEnemies.RemoveAll(enemy => enemy == null);

    if (spawnedEnemies.Count == 0)
    {
        // All enemies are gone, spawn a new wave
        SpawnWave();
    }

    // Display the current number of enemies on the screen
    //Debug.Log("Current enemies on screen: " + spawnedEnemies.Count);
}


    void SpawnWave()
    {
        // Check if there are remaining "Enemy" tags
        int remainingEnemyTags = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (remainingEnemyTags > 0)
        {
            // There are remaining enemies, wait until they are defeated to spawn a new wave
            return;
        }

        for (int i = 0; i < totalEnemies; i++)
        {
            // Randomly select a character prefab from the array based on weight/percentage
            GameObject characterPrefab = GetRandomCharacterPrefab();

            // Generate a random position within the spawn radius
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // Spawn the character at the generated position
            GameObject spawnedCharacter = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);

            // Add the spawned character to the list
            spawnedEnemies.Add(spawnedCharacter);
        }
    }

    GameObject GetRandomCharacterPrefab()
    {
        // Calculate the total weight
        float totalWeight = 0f;
        foreach (float weight in characterWeights)
        {
            totalWeight += weight;
        }

        // Generate a random value between 0 and the total weight
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        // Select the character prefab based on the random value
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            cumulativeWeight += characterWeights[i];
            if (randomValue <= cumulativeWeight)
            {
                return characterPrefabs[i];
            }
        }

        // If no valid character prefab found, return the first one in the array
        return characterPrefabs[0];
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0f;  // Ensure enemies spawn on the ground
        Vector3 spawnPosition = transform.position + randomOffset;
        return spawnPosition;
    }
}
