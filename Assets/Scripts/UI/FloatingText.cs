using UnityEngine;
using System.Collections;
using TMPro;

public class FloatingText : MonoBehaviour
{
    // Define the event type
    public delegate void FloatingTextEvent(Vector3 position, int amount);
    public delegate void FloatingTextEventWithColor(Vector3 position, int amount, Color color);

    // Create the event
    private static event FloatingTextEvent OnFloatingText;
    private static event FloatingTextEventWithColor OnFloatingTextWithColor;

    public Color defaultColor = Color.white;
    public float fadeTime = 1.5f; // In seconds
    public float moveSpeed = 1f; // Units per second
    public Vector3 yOffset;

    // Subscribe to the event
    private void OnEnable()
    {
        OnFloatingText += DisplayDefaultColor;
        OnFloatingTextWithColor += Display;
    }

    private void OnDisable()
    {
        OnFloatingText -= DisplayDefaultColor;
        OnFloatingTextWithColor -= Display;
    }

    private void DisplayDefaultColor(Vector3 position, int amount)
    {
        // Removed the color calculation here
        Display(position, amount, defaultColor);
    }

    private void Display(Vector3 position, int amount, Color color)
    {
        // Get a text object from the pool
        GameObject floatingTextObject = FloatingTextPooler.Instance.Get();
        floatingTextObject.transform.position = position + yOffset;

        TMP_Text floatingText = floatingTextObject.GetComponent<TMP_Text>();
        floatingText.text = amount.ToString();
        floatingText.color = color;

        // Debugging line: Print amount and color to the console
        //Debug.Log("Amount: " + amount + " Color: " + color);

        // Start moving and fading
        StartCoroutine(MoveUp(floatingTextObject.transform));
        StartCoroutine(FadeOut(floatingText));
    }

    private IEnumerator MoveUp(Transform transform)
    {
        float timer = 0;

        while(timer < fadeTime)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut(TMP_Text floatingText)
    {
        Color originalColor = floatingText.color;
        float timer = 0;

        while(timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            floatingText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t));
            yield return null;
        }

        // Return the text object back to the pool
        FloatingTextPooler.Instance.ReturnToPool(floatingText.gameObject);
    }

    // New methods to invoke events
    public static void TriggerFloatingText(Vector3 position, int amount)
    {
        OnFloatingText?.Invoke(position, amount);
    }

    public static void TriggerFloatingTextWithColor(Vector3 position, int amount, Color color)
    {
        OnFloatingTextWithColor?.Invoke(position, amount, color);
    }

    private Color GetAmountColor(int amount)
    {
        // Convert amount into a percentage of the maximum possible damage.
        float percentage = Mathf.Clamp((float)amount / 50f, 0f, 1f);
        // Use the percentage to interpolate between orange and red.
        return Color.Lerp(Color.green, Color.red, percentage);
    }
}
