using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurveManager))]
public class BezierCurveManagerEditor : Editor
{
    // This variable will hold the type of spline to be created.
    private enum SplineType { BezierSpline, LineSpline }
    private SplineType splineType;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BezierCurveManager myScript = (BezierCurveManager)target;

        // Add a dropdown for the user to select the type of spline to add.
        splineType = (SplineType)EditorGUILayout.EnumPopup("Spline type", splineType);

        if (GUILayout.Button("Add Spline"))
        {
            BaseSpline newSplinePrefab;

            // Depending on the selected spline type, select the corresponding prefab.
            switch (splineType)
            {
                case SplineType.BezierSpline:
                    newSplinePrefab = myScript.BezierSplinePrefab;
                    break;
                case SplineType.LineSpline:
                default:
                    newSplinePrefab = myScript.LineSplinePrefab;
                    break;
            }

            // Instantiate the selected prefab.
            BaseSpline newSpline = (BaseSpline)PrefabUtility.InstantiatePrefab(newSplinePrefab, myScript.transform);

            // Now call the AddSpline method
            myScript.AddSpline(newSpline);
        }

        if (GUILayout.Button("Remove Last Spline"))
        {
            myScript.RemoveLastSpline();
        }
    }
}
