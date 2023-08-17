using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class InputController : MonoBehaviour
{
    public ComboSystem comboSystem;
    //public AudioSource audioSource; // Reference to the AudioSource component
    public GameObject player;
    private FloatingText floatingTextRef;
    public Vector3 comboTextOffset;
    public int maxComboCountForColorChange = 20;  // Combo count at which the color will be fully red
    public TMP_Text comboCountUITextRef;

    private void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
        comboCountUITextRef.text = comboSystem.comboCount.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsWithinTimingWindow())
            {
                comboSystem.comboCount++;
                comboSystem.beatHit = true;
                // Calculate color based on combo count
                float t = Mathf.Clamp01((float)comboSystem.comboCount / maxComboCountForColorChange);
                Color color = Color.Lerp(Color.white, Color.red, t);
                FloatingText.TriggerFloatingTextWithColor(player.transform.position + comboTextOffset, comboSystem.comboCount, color);
                comboCountUITextRef.text = comboSystem.comboCount.ToString();
                comboCountUITextRef.color = color;
            }
            else
            {
                // Reset combo count or perform combo break actions
                comboSystem.comboCount = 0;
                float t = Mathf.Clamp01((float)comboSystem.comboCount / maxComboCountForColorChange);
                Color color = Color.Lerp(Color.white, Color.red, t);
                FloatingText.TriggerFloatingTextWithColor(player.transform.position + comboTextOffset, comboSystem.comboCount, color);
                comboCountUITextRef.text = comboSystem.comboCount.ToString();
                comboCountUITextRef.color = color;
            }
        }
    }

    private bool IsWithinTimingWindow()
    {
        // Check if the current time is within the timing window of the last beat
        return Mathf.Abs(Time.time - comboSystem.lastBeatTime) <= comboSystem.timingWindow;
    }
}
