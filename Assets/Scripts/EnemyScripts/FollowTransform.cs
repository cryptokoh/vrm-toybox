using UnityEngine;
using System.Text.RegularExpressions;

public class FollowTransform : MonoBehaviour
{
    public string targetObjectNameRegex = "(?i).*head.*";
    public Vector3 offset = new Vector3(0f, 0f, 1f); // Offset from the target object

    public Transform targetTransform;
    private Vector3 initialOffset;

    private void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Avatar");
        if (playerObject != null)
        {
            targetTransform = FindObjectByRegex(playerObject.transform, targetObjectNameRegex);
            if (targetTransform == null)
            {
                Debug.LogError("Could not find object matching regex: " + targetObjectNameRegex);
            }
            else
            {
                initialOffset = targetTransform.InverseTransformDirection(transform.position - targetTransform.position);
            }
        }
        else
        {
            Debug.LogError("Player object not found.");
        }
    }

    private void FixedUpdate()
    {
        if (targetTransform != null)
        {
            Vector3 targetPosition = targetTransform.position + targetTransform.rotation * offset;
            transform.position = targetTransform.TransformPoint(initialOffset) + targetTransform.rotation * offset;
            transform.rotation = targetTransform.rotation;
        }
    }

    private Transform FindObjectByRegex(Transform parent, string regexPattern)
    {
        Transform result = null;
        Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

        foreach (Transform child in parent)
        {
            if (regex.IsMatch(child.gameObject.name))
            {
                result = child;
                break;
            }

            result = FindObjectByRegex(child, regexPattern);
            if (result != null)
            {
                break;
            }
        }

        return result;
    }
}
