using UnityEngine;

public class CoinSoundPlayer : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        if (clips.Length == 0)
        {
            Debug.LogError("No audio clips set for playing.");
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[randomIndex]);
    }
}
