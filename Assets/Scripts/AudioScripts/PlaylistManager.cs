using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlaylistManager : MonoBehaviour
{
    [Serializable]
    public class AudioTrack
    {
        public AudioClip audioClip;
        public float bpm;
    }

    public AudioTrack[] playlist;

    private int currentTrackIndex = 0;
    private AudioSource audioSource;

    public static event Action<float> BPMChangedEvent;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; // Disable auto-play on awake
        PlayCurrentTrack();
    }

    private void PlayCurrentTrack()
    {
        if (currentTrackIndex < playlist.Length)
        {
            AudioTrack currentTrack = playlist[currentTrackIndex];
            AudioClip audioClip = currentTrack.audioClip;
            float bpm = currentTrack.bpm;

            audioSource.clip = audioClip;
            audioSource.Play();

            // Send out the BPM event
            BPMChangedEvent?.Invoke(bpm);
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying && currentTrackIndex < playlist.Length)
        {
            currentTrackIndex++;
            PlayCurrentTrack();
        }
    }
}
