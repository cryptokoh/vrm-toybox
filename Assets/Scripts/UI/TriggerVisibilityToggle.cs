using UnityEngine;

public class TriggerVisibilityToggle : MonoBehaviour
{
    public GameObject objectToToggle;
    private GameObject player;
    private MonoBehaviour[] playerScripts;
    public DialogManager dialogManagerRef;
    private ThirdPersonOrbitCamBasic thirdPersonCamera;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerScripts = player.GetComponents<MonoBehaviour>();
        }

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            thirdPersonCamera = mainCamera.GetComponent<ThirdPersonOrbitCamBasic>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            // Toggle visibility ON when player enters the collider
            objectToToggle.SetActive(true);

            // Disable all scripts on player
            foreach (var script in playerScripts)
            {
                script.enabled = false;
            }

            // Disable the ThirdPersonOrbitCamBasic script on the main camera
            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }

            // Find the Avatar child and get its animator
            GameObject avatarChild = GameObject.FindGameObjectWithTag("Avatar");
            if (avatarChild != null)
            {
                Animator anim = avatarChild.GetComponent<Animator>();
                if (anim != null)
                {
                    dialogManagerRef.anim = anim;  // update the anim reference in dialogManagerRef
                    dialogManagerRef.anim.SetFloat("Speed", 0f);  // Set Speed to 0
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            // Toggle visibility OFF when player exits the collider
            objectToToggle.SetActive(false);

            // Disable all scripts on player
            foreach (var script in playerScripts)
            {
                script.enabled = true;
            }

            // Disable the ThirdPersonOrbitCamBasic script on the main camera
            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = true;
            }

        }
    }
}
