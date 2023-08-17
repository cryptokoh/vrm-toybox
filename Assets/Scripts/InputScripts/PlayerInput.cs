using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 10f;

    public InteractableObject currentInteractableObject;
    public LayerMask interactableLayer; // Layer mask for interactable objects
    public LayerMask ignoreLayer; // Layer mask for objects to ignore

    public Material defaultMatTest; // Default material for the interacted object
    public Material interactedMatTest; // Material to apply when the object is interacted with

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private void Interact()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
        RaycastHit hit;

        int layerMask = interactableLayer.value; // Use the layer mask for interactable objects

        if (Physics.Raycast(ray, out hit, interactDistance, layerMask))
        {
            InteractableObject interactableObject = hit.collider.GetComponent<InteractableObject>();
            if (interactableObject != null)
            {
                SetCurrentInteractable(interactableObject);
                Debug.Log("Interacting with object: " + interactableObject.gameObject.name);

                // Subscribe to the OnInteract event
                interactableObject.OnInteract += (obj) => HandleInteraction(obj);
                interactableObject.Interact(); // Invoke the Interact method on the interactable object
            }
            else
            {
                ClearCurrentInteractable();
                Debug.Log("No InteractableObject component found on object: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            ClearCurrentInteractable();
            Debug.Log("No object detected within interact distance.");
        }

        // Draw a debug line to visualize the raycast
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 0.5f);
    }

    private void HandleInteraction(GameObject interactableObject)
    {
        // Perform desired actions with the interacted object
        //Debug.Log("Interacted with object: " + interactableObject.name);

        // Check the tag or component of the interacted object to determine the functionality
        if (interactableObject.CompareTag("Chest"))
        {
            Debug.Log("You Opened A Chest");
        }
        else if (interactableObject.CompareTag("Collectible"))
        {
            Debug.Log("You Found A Collectible");
        }
        // Add more conditionals for other interactable object types

        // Change the material of the interacted object
        Renderer renderer = interactableObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material currentMaterial = renderer.sharedMaterial;
            Material newMaterial = (currentMaterial == defaultMatTest) ? interactedMatTest : defaultMatTest;
            renderer.material = newMaterial;
        }

        // Unsubscribe from the event to avoid memory leaks
        interactableObject.GetComponent<InteractableObject>().OnInteract -= (obj) => HandleInteraction(obj);
    }

    private void SetCurrentInteractable(InteractableObject interactableObject)
    {
        currentInteractableObject = interactableObject;
    }

    private void ClearCurrentInteractable()
    {
        currentInteractableObject = null;
    }
}
