using UnityEngine;

public class LineSpline : BaseSpline
{
    private void OnDrawGizmos()
    {
        if (points.Length == 2)
        {
            Vector3 p0 = points[0].position;
            Vector3 p1 = points[1].position;

            // Draw the line directly, as it's a straight line between the points
            Gizmos.DrawLine(p0, p1);

            // Draw gizmo spheres at each control point
            float sphereRadius = 0.1f;  // Adjust this value as needed
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p0, sphereRadius);
            Gizmos.DrawSphere(p1, sphereRadius);
        }
    }

    public override Vector3 GetTangentAtEndPoint()
    {
        if (points.Length < 2)
            return base.GetTangentAtEndPoint();

        return (points[1].position - points[0].position).normalized;
    }

    // Override the GetPoint method for LineSpline.
    public override Vector3 GetPoint(float t)
    {
        // Calculate the position on the line segment using the t value.
        // For a line spline, it's simply a linear interpolation between StartPoint and EndPoint.

        // Clamp t value to [0, 1]
        t = Mathf.Clamp01(t);

        // Calculate position on the line segment using linear interpolation
        Vector3 point = Vector3.Lerp(StartPoint.position, EndPoint.position, t);

        return point;
    }

        public override float GetLength()
    {
        // Calculate the length of the LineSpline, which is simply the distance between its start and end points.
        float length = Vector3.Distance(StartPoint.position, EndPoint.position);
        return length;
    }
}
