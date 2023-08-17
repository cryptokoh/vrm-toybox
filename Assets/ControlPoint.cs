using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    public Transform target; // assign the corresponding point from your spline in the inspector

    void Update()
    {
        if (target)
        {
            target.position = transform.position;
        }
    }
}
