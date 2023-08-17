using System.Linq;
using UnityEngine;

public class BezierCurveManager : MonoBehaviour
{

    public enum AnimationMode
    {
        Loop,
        PingPong
    }

    [SerializeField]
    private AnimationMode animationMode = AnimationMode.Loop;


    //[SerializeField] private BezierSpline[] curves;
    [SerializeField] private BaseSpline[] curves = new BaseSpline[0]; // Initialize the array with zero elements


    [SerializeField]
    private BaseSpline bezierSplinePrefab;  // The BezierSpline prefab

    [SerializeField]
    private BaseSpline lineSplinePrefab;  // The LineSpline prefab

    // Rest of your script...

    public BaseSpline BezierSplinePrefab { get { return bezierSplinePrefab; } }
    public BaseSpline LineSplinePrefab { get { return lineSplinePrefab; } }

    public float t = 0f;
    public Vector3 positionOnPath;
    public GameObject cubePrefab;
    private float totalPathLength = 0f; // Total length of the combined curves

    [SerializeField]
    private bool autoDrive = false; // Whether to enable auto-driving
    public float animationSpeed = 1.0f; // Adjust this value to control the speed of the animation


    public BaseSpline GetCurrentSpline(float t, ref float tWithinSpline)
    {
        int splineIndex = 0;
        tWithinSpline = GetTWithinSpline(ref splineIndex);
        return curves[splineIndex];
    }

    private void Start()
    {
        // Calculate the total length of the combined curves
        CalculateTotalPathLength();
    }
 private void Update()
{
    ConnectCurves();

    // Handle the case when theincrementanimationProgressre are no splines in the curves array
    if (curves.Length == 0)
    {
        Debug.LogError("No splines in the curves array. Please add splines before moving the cube.");
        return;
    }

    // Calculate the total path length only if it has not been calculated before
    if (totalPathLength <= 0f)
    {
        totalPathLength = CalculateTotalPathLength();
    }

    if (autoDrive)
    {
        float animationDuration = totalPathLength / animationSpeed;
        switch(animationMode)
        {
            case AnimationMode.Loop:
                // Loop mode: repeat the animation indefinitely
                t = Mathf.Repeat(Time.time / animationDuration, 1f);
                break;
            case AnimationMode.PingPong:
                // PingPong mode: alternate between forward and reverse
                t = Mathf.PingPong(Time.time / animationDuration, 1f);
                break;
        }
    }else{
        
    }

    // Get the current spline index and the corresponding t value within that spline
    int splineIndex = 0;
    float tWithinSpline = GetTWithinSpline(ref splineIndex);

    // Get the current spline
    BaseSpline spline = curves[splineIndex];

    // Get the position on the spline path
    positionOnPath = spline.GetPoint(tWithinSpline);

    // Update the cube prefab's position and orientation along the path
    cubePrefab.transform.position = positionOnPath;
    cubePrefab.transform.rotation = GetRotationOnSpline(spline, tWithinSpline);
}


public Quaternion GetRotationOnSpline(BaseSpline spline, float t)
{
    // Get the previous position on the spline
    float tPrev = Mathf.Clamp01(t - 0.01f); // Go back a little to get a previous position
    Vector3 positionPrev = spline.GetPoint(tPrev);

    // Get the current position on the spline
    Vector3 positionCurrent = spline.GetPoint(t);

    // Calculate the forward direction from the previous position to the current position
    Vector3 forwardDirection = (positionCurrent - positionPrev).normalized;

    // Calculate the orientation (rotation) using the forward direction and up vector (e.g., Vector3.up)
    Quaternion rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);

    return rotation;
}


public float CalculateTotalPathLength()
{
    float totalLength = 0f;

    foreach (BaseSpline spline in curves)
    {
        totalLength += spline.GetLength();
    }

    return totalLength;
}


public float GetTWithinSpline(ref int splineIndex)
{
    float accumulatedLength = 0f;
    float normalizedT = t * totalPathLength;

    // Find the current spline and its corresponding t value
    for (int i = 0; i < curves.Length; i++)
    {
        BaseSpline currentSpline = curves[i];
        float splineLength = currentSpline.GetLength();

        if (normalizedT >= accumulatedLength && normalizedT <= accumulatedLength + splineLength)
        {
            splineIndex = i;
            break;
        }

        accumulatedLength += splineLength;
    }

    // Get the t value within the current spline
    BaseSpline spline = curves[splineIndex];
    float tWithinSpline;

    if (spline is BezierSpline)
    {
        tWithinSpline = (normalizedT - accumulatedLength) / spline.GetLength();
    }
    else
    {
        // For LineSpline, scale t based on its own length
        tWithinSpline = (normalizedT - accumulatedLength) / spline.GetLength();
    }

    return tWithinSpline;
}




   public Vector3 GetTangentAtPosition(BaseSpline spline, float t)
{
    int index = Mathf.FloorToInt(t * (spline.points.Length - 1));
    float tInSegment = t * (spline.points.Length - 1) - index;

    if (index == spline.points.Length - 1)
    {
        // For the last segment, use the tangent at the end point
        return spline.GetTangentAtEndPoint();
    }
    else
    {
        // Use the tangent based on the current segment (Bezier or Line)
        Vector3 tangent;

        if (spline is BezierSpline)
        {
            tangent = ((BezierSpline)spline).GetBezierTangent(tInSegment);
        }
        else
        {
            tangent = ((LineSpline)spline).GetTangentAtEndPoint();
        }

        return tangent.normalized;
    }
}

public void ConnectCurves()
{
    for (int i = 0; i < curves.Length - 1; i++)
    {
        BaseSpline currentSpline = curves[i];
        BaseSpline nextSpline = curves[i + 1];

        if (currentSpline is BezierSpline && nextSpline is BezierSpline)
        {
            // Connect the end point of the first BezierSpline to the start point of the next BezierSpline.
            currentSpline.EndPoint.position = nextSpline.StartPoint.position;
        }
        else if (currentSpline is BezierSpline && nextSpline is LineSpline)
        {
            // Connect the end point of the BezierSpline to the start point of the LineSpline.
            
        }
        else if (currentSpline is LineSpline && nextSpline is BezierSpline)
        {

            currentSpline.EndPoint.position = nextSpline.StartPoint.position;

        }
        else if (currentSpline is LineSpline && nextSpline is LineSpline)
        {
            // Connect the end point of the first LineSpline to the start point of the next LineSpline.
            currentSpline.EndPoint.position = nextSpline.StartPoint.position;
        }
    }
}






  public void AddSpline(BaseSpline newSpline)
{
    if (curves.Length > 0)
    {
        newSpline.transform.position = curves[curves.Length - 1].EndPoint.position;
        newSpline.StartPoint.position = newSpline.transform.position;

        if (newSpline is BezierSpline)
        {
            if (curves[curves.Length - 1] is BezierSpline)
            {
                newSpline.transform.rotation = curves[curves.Length - 1].EndPoint.rotation;

                newSpline.points[1].position = newSpline.StartPoint.position + (newSpline.StartPoint.position - curves[curves.Length - 1].points[2].position);
            }
            else if (curves[curves.Length - 1] is LineSpline)
            {
                Vector3 tangent = curves[curves.Length - 1].GetTangentAtEndPoint();
                newSpline.transform.rotation = Quaternion.LookRotation(tangent);

                newSpline.points[1].position = newSpline.StartPoint.position + tangent.normalized * (curves[curves.Length - 1].EndPoint.position - curves[curves.Length - 1].StartPoint.position).magnitude;
            }
        }
        else if (newSpline is LineSpline)
        {
            if (curves[curves.Length - 1] is BezierSpline)
            {
                Vector3 tangent = curves[curves.Length - 1].GetTangentAtEndPoint();
                newSpline.transform.rotation = Quaternion.LookRotation(tangent);

                newSpline.StartPoint.position = curves[curves.Length - 1].EndPoint.position;
                newSpline.EndPoint.position = newSpline.StartPoint.position + tangent.normalized * (curves[curves.Length - 1].EndPoint.position - curves[curves.Length - 1].StartPoint.position).magnitude;
            }
            else if (curves[curves.Length - 1] is LineSpline)
            {
                Vector3 tangent = curves[curves.Length - 1].GetTangentAtEndPoint();
                newSpline.transform.rotation = Quaternion.LookRotation(tangent);

                newSpline.StartPoint.position = curves[curves.Length - 1].EndPoint.position;

                newSpline.EndPoint.position = newSpline.StartPoint.position + tangent.normalized * (curves[curves.Length - 1].EndPoint.position - curves[curves.Length - 1].StartPoint.position).magnitude;
            }
        }
    }

    BaseSpline[] newCurves = new BaseSpline[curves.Length + 1];

    for (int i = 0; i < curves.Length; i++)
    {
        newCurves[i] = curves[i];
    }

    newCurves[newCurves.Length - 1] = newSpline;

    curves = newCurves;
}

    public void RemoveLastSpline()
{
    if (curves.Length > 0)
    {
        DestroyImmediate(curves[curves.Length - 1].gameObject);

        BaseSpline[] newCurves = new BaseSpline[curves.Length - 1];

        for (int i = 0; i < newCurves.Length; i++)
        {
            newCurves[i] = curves[i];
        }

        curves = newCurves;
    }
}


    private void OnDrawGizmos()
{
    if (curves != null && curves.Length > 1)
    {
        for (int i = 0; i < curves.Length - 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(curves[i].EndPoint.position, curves[i + 1].StartPoint.position);
        }
    }
}

}
