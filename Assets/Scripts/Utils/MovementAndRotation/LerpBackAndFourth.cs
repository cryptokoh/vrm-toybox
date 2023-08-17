using UnityEngine;

public class LerpBackAndFourth : MonoBehaviour
{
    public float speed = 1f;
    public float distance = 2f;
    public float lerpTime = 1f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool isMovingForward = true;
    private bool isLerping = false;
    private float currentLerpTime;

    private Lerp lerp;

    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + Vector3.right * distance;

        lerp = GetComponent<Lerp>();
    }

    void Update()
    {
        if (!isLerping)
        {
            if (isMovingForward)
                StartCoroutine(PerformLerp(startPosition, endPosition));
            else
                StartCoroutine(PerformLerp(endPosition, startPosition));
        }
    }

    private System.Collections.IEnumerator PerformLerp(Vector3 start, Vector3 end)
    {
        isLerping = true;
        currentLerpTime = 0f;

        while (currentLerpTime < lerpTime)
        {
            currentLerpTime += Time.deltaTime;
            float perc = currentLerpTime / lerpTime;
            transform.position = Vector3.Lerp(start, end, perc);
            yield return null;
        }

        transform.position = end;
        isMovingForward = !isMovingForward;
        isLerping = false;
    }
}
