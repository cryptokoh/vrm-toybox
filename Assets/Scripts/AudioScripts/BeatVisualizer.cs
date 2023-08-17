using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BeatVisualizer : MonoBehaviour
{
    public Slider beatSlider;
    public Color normalColor = Color.white;
    public Color beatColor = Color.red;

    private Coroutine beatCoroutine;
    private int currentBeat = 0;
    private Image fillImage;

    private float bpm;

    private void Start()
    {
        fillImage = beatSlider.fillRect.GetComponent<Image>();
        PlaylistManager.BPMChangedEvent += HandleBPMChanged;
    }

    private void OnDestroy()
    {
        PlaylistManager.BPMChangedEvent -= HandleBPMChanged;
    }

    private void HandleBPMChanged(float newBPM)
    {
        bpm = newBPM;
        StartBeatVisual();
    }

    public void SetBPM(float newBPM)
    {
        bpm = newBPM;
    }

    public void StartBeatVisual()
    {
        if (beatCoroutine != null)
        {
            StopCoroutine(beatCoroutine);
        }

        beatCoroutine = StartCoroutine(BeatVisualCoroutine());
    }

    private IEnumerator BeatVisualCoroutine()
    {
        float beatDuration = 60f / bpm; // Time for a single beat

        while (true)
        {
            float beatStart = Time.time;
            float beatEnd = beatStart + beatDuration;

            while (Time.time < beatEnd)
            {
                yield return null;
            }

            currentBeat = (currentBeat % 4) + 1; // Cycle through beats 1, 2, 3, and 4
            beatSlider.value = currentBeat;

            // Update the fill color
            fillImage.color = beatColor;

            float colorDuration = beatDuration * 0.5f; // Half the beat duration for color change
            float colorStart = Time.time;
            float colorEnd = colorStart + colorDuration;

            while (Time.time < colorEnd)
            {
                float progress = (Time.time - colorStart) / colorDuration;
                fillImage.color = Color.Lerp(beatColor, normalColor, progress);
                yield return null;
            }

            fillImage.color = normalColor;
        }
    }
}
