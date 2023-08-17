using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpline : MonoBehaviour
{
    public Transform[] points;

    public Transform StartPoint => points[0];
    public Transform EndPoint => points[points.Length - 1];

    public virtual Vector3 GetTangentAtEndPoint()
{
    if (points.Length < 2)
        return Vector3.forward; // or whatever default direction you want

    if (this is BezierSpline)
    {
        return (points[points.Length - 1].position - points[points.Length - 2].position).normalized;
    }
    else if (this is LineSpline)
    {
        return (points[1].position - points[0].position).normalized;
    }

    return Vector3.forward;
}


      public virtual Vector3 GetTangentAtStartPoint()
    {
        if (points.Length < 2)
            return Vector3.forward; // or whatever default direction you want

        return (points[1].position - points[0].position).normalized;
    }

    // Returns the position on the spline at the given t value (0 to 1).
    public virtual Vector3 GetPoint(float t)
    {
        // Simple linear interpolation between StartPoint and EndPoint
        return Vector3.Lerp(StartPoint.position, EndPoint.position, t);
    }

    public virtual float GetLength()
    {
        // Implement the logic to calculate the length of the spline.
        // This method will be overridden in the derived classes (BezierSpline and LineSpline).
        return 0f; // Replace this with the actual calculation of the spline length.
    }



    // Rest of the BaseSpline code...
}

