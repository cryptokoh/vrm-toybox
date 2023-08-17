using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{
    public InteractableObject interactableObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.currentInteractableObject = interactableObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null && playerInput.currentInteractableObject == interactableObject)
            {
                playerInput.currentInteractableObject = null;
            }
        }
    }
}
