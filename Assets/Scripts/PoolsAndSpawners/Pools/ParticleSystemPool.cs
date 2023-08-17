using UnityEngine;
using System.Collections.Generic;

public class ParticleSystemPool : MonoBehaviour
{
    public static ParticleSystemPool Instance { get; private set; }

    public ParticleSystem prefab; // ParticleSystem Prefab to pool
    public int poolSize = 10; // Size of the pool

    private List<ParticleSystem> pool; // The pool of ParticleSystems

        private Queue<ParticleSystem> particlesPool = new Queue<ParticleSystem>();


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

        pool = new List<ParticleSystem>();

        // Populate the pool
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem newParticle = Instantiate(prefab, this.transform);
            newParticle.gameObject.SetActive(false);
            pool.Add(newParticle);
        }
    }

    // Method to get a ParticleSystem from the pool
    public ParticleSystem Get()
    {
        foreach (ParticleSystem ps in pool)
        {
            if (!ps.gameObject.activeInHierarchy)
            {
                ps.gameObject.SetActive(true);
                return ps;
            }
        }

        // If no inactive particles are available, expand the pool
        ParticleSystem newParticle = Instantiate(prefab, this.transform);
        pool.Add(newParticle);
        return newParticle;
    }

    // Method to return a ParticleSystem to the pool
    public void ReturnToPool(ParticleSystem particles)
    {
        // Stop the particle system and clear it
        particles.Stop();
        particles.Clear();

        // Set the particle system to inactive
        particles.gameObject.SetActive(false);

        // Add the particles to the pool
        particlesPool.Enqueue(particles);
    }
}
