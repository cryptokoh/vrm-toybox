using UnityEngine;
using System.Collections;

public class ParticleCleanUpToPool : MonoBehaviour
{
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckIfAlive());
    }

    IEnumerator CheckIfAlive()
    {
        // Wait until the ParticleSystem is no longer playing
        while (particles.isPlaying)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Return the ParticleSystem to the pool
        ParticleSystemPool.Instance.ReturnToPool(particles);
    }
}
