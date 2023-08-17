using UnityEngine;
using System;

public class InteractableObject : MonoBehaviour
{
    public delegate void InteractAction(GameObject interactableObject);
    public event InteractAction OnInteract; // Event to be triggered when interaction occurs

    private Animator animator; // Reference to the Animator component

    private void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    public void Interact()
    {
        OnInteract?.Invoke(gameObject);

        // Trigger the "Activate" parameter in the animator
        animator?.SetTrigger("Activate");
    }
}
