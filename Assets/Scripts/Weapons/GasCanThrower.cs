using UnityEngine;
using System.Linq;

public class GasCanThrower : MonoBehaviour
{
    [Tooltip("Reference to the main camera.")]
    public Camera mainCamera; 

    [Tooltip("Initial number of gas cans.")]
    public int initialCansCount = 5; 

    [Tooltip("Number of gas cans to add when reloading.")]
    public int reloadCansCount = 5;

    [Tooltip("Length of the debug ray.")]
    public float rayLength = 5f; 

    [Tooltip("Offset for spawning GasCans.")]
    public Vector3 offset;

    [Tooltip("Reference to the player's right hand.")]
    public GameObject rightHand;

    [Tooltip("Reference to the player's Animator.")]
    public Animator playerAnimator;

    private int currentCansCount;

    private void Start()
{
    // Set the initial cans count.
    currentCansCount = initialCansCount;
    // Find the player's animator using the "Avatar" tag
        playerAnimator = GameObject.FindGameObjectWithTag("Avatar").GetComponent<Animator>();

        // Look for the right hand object in the children of the "Avatar" tagged object
        FindRightHandRecursive(GameObject.FindGameObjectWithTag("Avatar").transform);

    // Look for the right hand object at the start and store a reference
   
}
private void FindRightHandRecursive(Transform parentTransform)
    {
        // Check if the current transform's name contains "r" and "hand"
        string lowerCaseName = parentTransform.name.ToLower();
        if (lowerCaseName.Contains("r") && lowerCaseName.Contains("hand"))
        {
            rightHand = parentTransform.gameObject;
            return;
        }

        // Continue searching in the children of the current transform
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            FindRightHandRecursive(child);
        }
    }

    private void Update()
    {

        if (rightHand != null)
        {
            // Lock the rotation to the forward direction of the camera
            Quaternion rotation = Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up);

            // Follow the position of the right hand and maintain the rotation
            transform.SetPositionAndRotation(rightHand.transform.position, rotation);
        }
        // Get the throw direction from the main camera's forward direction
        Vector3 throwDirection = mainCamera.transform.forward;

        // Draw a debug ray for the throw direction.
        Debug.DrawRay(transform.position, throwDirection * rayLength, Color.red);

        // Check if the F key is pressed.
// Check if the F key is pressed.
if (Input.GetKeyDown(KeyCode.G) && currentCansCount > 0)
{
    // If the GasCanPool instance exists, call the ThrowGasCan method.
    if (GasCanPool.Instance != null)
    {
                if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("isThrowing");
        }
        else
        {
            Debug.LogWarning("No playerAnimator reference set in GasCanThrower.");
        }

        GameObject gasCan = GasCanPool.Instance.Get(); // Get a gas can from the pool
        gasCan.transform.position = transform.position + offset; // Set the position to the thrower's location

        GasCanPool.Instance.ThrowGasCan(gasCan, throwDirection);
        currentCansCount--; // Decrement the number of cans left.

        // If the playerAnimator reference isn't null, set the IsThrowing trigger.

    }
    else
    {
        Debug.LogError("No GasCanPool instance found.");
    }
}



        // Check if the R key is pressed for reloading.
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentCansCount += reloadCansCount; // Reload the cans.
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a gizmo at the spawn location.
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + offset, 0.2f);
    }
}
