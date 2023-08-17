using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ComboSystem : MonoBehaviour
{
    public int comboCount = 0;
    public float timingWindow = 0.1f; // Timing window in seconds
    public float comboTimeout = 2.0f;  // Timeout in seconds
    public float sliderLerpTime = 0.5f; // Time it takes for the slider to lerp

    public bool beatHit = false;
    private int missedBeats = 0;
    private Coroutine comboTimeoutCoroutine;  // Coroutine for combo timeout
    private Coroutine blinkCoroutine; // Coroutine for blinking

    public TMP_Text comboCountUITextRef;
    public Slider comboCooldownUISliderRef;

    public Color defaultColor = Color.white;
    public Color maxComboColor = Color.red;
    public float blinkInterval = 0.5f;  // Blink interval in seconds
    public float sliderSpeed = 0.1f; // Speed of slider transition. Adjust as needed.
    public int maxComboCountForColorChange = 20;  // Combo count at which the color will be fully red


    public float lastBeatTime { get; private set; } // Time of the last detected beat

    private void Start()
    {
        PlaylistManager.BPMChangedEvent += OnBPMChanged;
    }


    private void Update()
    {
        UpdateComboSlider();
        UpdateComboColor();
    }

    private void OnDestroy()
    {
        PlaylistManager.BPMChangedEvent -= OnBPMChanged;
    }

    private void OnBPMChanged(float bpm)
    {
            lastBeatTime = Time.time; // Store the time of the beat
            beatHit = true;

        if (!beatHit)
        {
            missedBeats++;
            if (missedBeats >= 4)
            {
                comboCount = 0;
                missedBeats = 0;
            }
        }
        else
        {
            missedBeats = 0;
            beatHit = false;

            // If the combo count has reached max, start blinking
            if (comboCount >= 20)
            {
                if (blinkCoroutine == null)
                {
                    blinkCoroutine = StartCoroutine(BlinkCoroutine());
                }
            }
            else if (blinkCoroutine != null)
            {
                // Stop blinking if combo count has dropped below max
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
                comboCooldownUISliderRef.fillRect.GetComponent<Image>().color = defaultColor;
            }

            // Stop the running combo timeout coroutine if it exists, and start a new one
            if (comboTimeoutCoroutine != null)
            {
                StopCoroutine(comboTimeoutCoroutine);
            }
            comboTimeoutCoroutine = StartCoroutine(ComboTimeoutCoroutine());
        }
    }

    private void UpdateComboSlider()
    {
        comboCooldownUISliderRef.value = Mathf.MoveTowards(comboCooldownUISliderRef.value, comboCount, sliderSpeed * Time.deltaTime);
    }

    private void UpdateComboColor()
    {
        if (comboCount >= 20)
        {
            float t = (Mathf.Sin(Time.time - blinkInterval * Mathf.PI * 2) + 1) / 2;
            comboCooldownUISliderRef.fillRect.GetComponent<Image>().color = Color.Lerp(defaultColor, maxComboColor, t);
        }
        else
        {
            float t = Mathf.Clamp01((float)comboCount / maxComboCountForColorChange);
            Color color = Color.Lerp(defaultColor, maxComboColor, t);
            comboCooldownUISliderRef.fillRect.GetComponent<Image>().color = color;
        }
    }

    private IEnumerator ComboTimeoutCoroutine()
    {
        // Wait for the timeout period
        yield return new WaitForSeconds(comboTimeout);

        // If this coroutine wasn't stopped before the timeout, reset the combo counter
        comboCount = 0;
        comboCountUITextRef.text = comboCount.ToString();
    }

    private IEnumerator BlinkCoroutine()
    {
        Image fillImage = comboCooldownUISliderRef.fillRect.GetComponent<Image>();
        while (true)
        {
            fillImage.color = (fillImage.color == defaultColor) ? maxComboColor : defaultColor;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
