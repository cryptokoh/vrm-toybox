using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFade : MonoBehaviour
{
    public float duration = 2f; // Duration in seconds
    public TextMeshProUGUI textMeshProObject;
    
    void Start()
    {
        textMeshProObject = GetComponent<TextMeshProUGUI>();
            if (textMeshProObject == null)
            {
                Debug.LogError("TextMeshProUGUI component missing from this gameobject. Please add one.");
            }
            else
            {
                StartCoroutine(FadeTextToZeroAlpha(duration, textMeshProObject));
            }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
