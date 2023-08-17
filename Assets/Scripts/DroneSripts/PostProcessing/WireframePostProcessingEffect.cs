using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class WireframePostProcessingEffect : MonoBehaviour
{
    public CameraClearFlags originalClearFlags; // Stores the original clear flags of the camera

    // Attach this script to a camera, this will make it render in wireframe
    void OnEnable()
    {
        originalClearFlags = GetComponent<Camera>().clearFlags;
        GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
    }

    void OnPreRender()
    {
        GL.wireframe = true;
    }

    void OnPostRender()
    {
        GL.wireframe = false;
    }

    void OnDisable()
    {
        GetComponent<Camera>().clearFlags = originalClearFlags;
    }
}
