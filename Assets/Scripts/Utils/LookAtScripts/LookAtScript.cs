using UnityEngine;

public class LookAtScript : MonoBehaviour
{
    public void LookAtTarget(Transform target)
    {
        Vector3 relativePos = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(relativePos);
    }
}
