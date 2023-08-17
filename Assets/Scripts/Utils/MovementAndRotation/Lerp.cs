using UnityEngine;

public class Lerp : MonoBehaviour
{
    public void PerformLerp(GameObject objectToLerp, Vector3 startPos, Vector3 endPos, float moveDistance, float lerpTime)
    {
        float currentLerpTime = 0f;

        // Reset start position and end position
        objectToLerp.transform.position = startPos;
        endPos = startPos + (endPos - startPos).normalized * moveDistance;

        while (currentLerpTime < lerpTime)
        {
            currentLerpTime += Time.deltaTime;
            float perc = currentLerpTime / lerpTime;
            objectToLerp.transform.position = Vector3.Lerp(startPos, endPos, perc);
        }

        // Ensure the object reaches the final position exactly
        objectToLerp.transform.position = endPos;
    }
}
