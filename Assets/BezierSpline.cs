using UnityEngine;

public class BezierSpline : BaseSpline
{

    [SerializeField]
    private Transform controlPoint1; // First control point

    [SerializeField]
    private Transform controlPoint2; // Second control point

    private void OnDrawGizmos()
{
    if (points.Length == 4)
    {
        Vector3 p0 = points[0].position;
        Vector3 p1 = points[1].position;
        Vector3 p2 = points[2].position;
        Vector3 p3 = points[3].position;

        Vector3 previousPoint = p0;
        for (float t = 0.01f; t <= 1; t += 0.01f)
        {
            Vector3 point = GetBezierPoint(p0, p1, p2, p3, t);
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        // Draw gizmo spheres at each control point
        float sphereRadius = 0.1f;  // Adjust this value as needed
        Gizmos.color = Color.red;

        // Draw gizmos for all points, not just start and end
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i].position, sphereRadius);
        }
    }
}

public override Vector3 GetTangentAtEndPoint()
    {
        if (points.Length < 4)
            return base.GetTangentAtEndPoint();

        return (points[3].position - points[2].position).normalized;
    }


    private Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 B = new Vector3();
        B = uuu * p0; // (1-t) ^ 3 * P0
        B += 3 * uu * t * p1; // 3(1-t)^2 * t * P1
        B += 3 * u * tt * p2; // 3(1-t) * t^2 * P2
        B += ttt * p3; // t^3 * P3

        return B;
    }

    public Vector3 GetBezierTangent(float t)
    {
        Vector3 p0 = points[0].position;
        Vector3 p1 = points[1].position;
        Vector3 p2 = points[2].position;
        Vector3 p3 = points[3].position;

        return -3 * (1 - t) * (1 - t) * p0
            + 3 * (1 - t) * (1 - t) * p1
            - 6 * t * (1 - t) * p1
            - 3 * t * t * p2
            + 6 * t * (1 - t) * p2
            + 3 * t * t * p3;
    }

    public override Vector3 GetPoint(float t)
    {
        // Calculate the position on the Bezier curve using the control points and t value.
        // The actual formula for calculating a point on a cubic Bezier curve is more complex,
        // but for demonstration purposes, we'll use a basic implementation.

        // Clamp t value to [0, 1]
        t = Mathf.Clamp01(t);

        // Calculate blending factors
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        // Calculate position on the Bezier curve using the formula
        Vector3 point = uuu * StartPoint.position +
                        3f * uu * t * controlPoint1.position +
                        3f * u * tt * controlPoint2.position +
                        ttt * EndPoint.position;

        return point;
    }

    public override float GetLength()
{
    // The Gauss-Legendre Quadrature coefficients for n=5
    Vector2[] gaussCoefficients =
    {
        new Vector2(-0.9061798459f, 0.2369268850f),
        new Vector2(-0.5384693101f, 0.4786286705f),
        new Vector2( 0.0000000000f, 0.5688888889f),
        new Vector2( 0.5384693101f, 0.4786286705f),
        new Vector2( 0.9061798459f, 0.2369268850f)
    };

    float length = 0f;

    // The points of the bezier curve
    Vector3 p0 = points[0].position;
    Vector3 p1 = points[1].position;
    Vector3 p2 = points[2].position;
    Vector3 p3 = points[3].position;

    // Compute each step of the integration
    for (int i = 0; i < gaussCoefficients.Length; i++)
    {
        float t = 0.5f * ((1f + gaussCoefficients[i].x));
        Vector3 derivative = GetBezierTangent(t);
        length += gaussCoefficients[i].y * derivative.magnitude;
    }

    length *= 0.5f;

    return length;
}
public void SetTangentAtStartPoint(Vector3 tangent)
{
    // Here we will adjust the controlPoint1 position such that the tangent 
    // at the start of the curve aligns with the provided tangent.

    Vector3 startPoint = StartPoint.position;

    // Calculate the desired position for the controlPoint1.
    // You might need to adjust the magnitude of the tangent vector to place controlPoint1 at a suitable distance.
    Vector3 desiredControlPoint1Position = startPoint + tangent;

    // Update controlPoint1's position
    controlPoint1.position = desiredControlPoint1Position;
}

}
