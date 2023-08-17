using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    public Transform target;
    public Transform headObject;

    void Start()
    {
        //GameObject headObject = GameObject.Find("head");  // Finds the object with name "head"

        // If no object with "head" in its name is found
        if (headObject == null)
        {
            Debug.LogWarning("No object named 'head' found.");
            return;
        }

        // Set the target transform to the found object
        target = headObject.transform;
    }

    void Update()
    {
        if (target != null)
        {
            transform.rotation = target.rotation;
        }
    }
}
