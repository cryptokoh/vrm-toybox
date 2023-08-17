using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMManager : MonoBehaviour
{
    private GameObject avatar;
    private Animator avatarAnimator;
    private Avatar avatarProfile;
    private PlayerController playerController; // Added reference to MoveComponent
    private HeadController headController;

    // Start is called before the first frame update
    void Start()
    {
        VRMLoader.OnAvatarChanged.AddListener(HandleAvatarChanged);
        
        // Try to find the initial avatar and its Animator
        avatar = GameObject.FindGameObjectWithTag("Avatar");
        if (avatar != null)
        {
            avatarAnimator = avatar.GetComponent<Animator>();
            avatarProfile = avatarAnimator.avatar; // Assuming that the animator already has a profile
        }
        else
        {
            Debug.LogError("No game object with tag 'Avatar' found!");
        }

         // Get the PlayerController
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("No playerController found on this GameObject!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleAvatarChanged(GameObject newAvatar)
{
    headController = GameObject.Find("_HeadController").GetComponent<HeadController>();

    // Update your reference to the avatar here
    avatar = newAvatar;

    // Get the animator from the new avatar
    Animator newAvatarAnimator = avatar.GetComponent<Animator>();
    
    // Check if animator exists
    if (newAvatarAnimator != null)
    {
        // Assuming you need to copy over the runtimeAnimatorController
        newAvatarAnimator.runtimeAnimatorController = avatarAnimator.runtimeAnimatorController;

        // Update the avatarAnimator reference
        avatarAnimator = newAvatarAnimator;

        // Assign the animator in your MoveComponent
        if (playerController != null)
        {
            playerController.anim = newAvatarAnimator;
        }


        // Create a bounding box from the avatar's mesh renderers
         Bounds bounds = new Bounds(avatar.transform.position, Vector3.zero);
        foreach (Renderer renderer in avatar.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        } 

        // Set the height and center of the parent's capsule collider to match the bounding box
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            capsule.height = bounds.size.y;

            // Adjust the capsule center so that the bottom of the collider aligns with the bottom of the avatar.
            // This assumes that your avatar's pivot is at the bottom of the model. If it isn't, you may need to adjust this.
            float yOffset = capsule.height / 2f; // half of the capsule's height
            capsule.center = new Vector3(0, yOffset-.01f, 0);
        }


    }
    else
    {
        Debug.LogError("New avatar does not have an Animator component.");
    }
}

}
