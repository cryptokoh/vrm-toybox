using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DisableComponentsOnHit : MonoBehaviour
{
    public float ResetDragOnDeath = 0.0f;

    public void DisableComponents(GameObject hitObject)
    {
        // Disable Nav Mesh Agent component
        NavMeshAgent navMeshAgent = hitObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        // Disable AI Animation Controller component
        AIAnimationController aiAnimationController = hitObject.GetComponent<AIAnimationController>();
        if (aiAnimationController != null)
        {
            aiAnimationController.enabled = false;
        }

        // Disable EnemyMovement.cs
        EnemyMovement EnemyMovement = hitObject.GetComponent<EnemyMovement>();
        if (EnemyMovement != null)
        {
            EnemyMovement.enabled = false;
        }

        // Disable Animator component
        Animator animator = hitObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Disable _AI_AgentBrain child object
        Transform aiAgentBrain = hitObject.transform.Find("_AI_AgentBrain");
        if (aiAgentBrain != null)
        {
            aiAgentBrain.gameObject.SetActive(false);
        }

        // Disable RigidbodyFollowPlayer component if present
        RigidbodyFollowPlayer rigidbodyFollowPlayer = hitObject.GetComponent<RigidbodyFollowPlayer>();
        if (rigidbodyFollowPlayer != null)
        {
            rigidbodyFollowPlayer.enabled = false;
        }

        // Disable MaintainYHeight component if present
        MaintainYHeight maintainYHeight = hitObject.GetComponent<MaintainYHeight>();
        if (maintainYHeight != null)
        {
            maintainYHeight.enabled = false;
        }

        // Enable colliders in the armature
        Collider[] colliders = hitObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        // Uncheck kinematic and set drag on rigidbodies in the armature
        Rigidbody[] rigidbodies = hitObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            // Skip the root object Rigidbody
            if (rigidbody.gameObject == hitObject)
            {
                continue;
            }
            
            rigidbody.isKinematic = false;
            rigidbody.drag = ResetDragOnDeath;
        }


        // Disable all LookAt scripts
        LookAtPlayer[] lookAtScripts = hitObject.GetComponentsInChildren<LookAtPlayer>();
        foreach (LookAtPlayer script in lookAtScripts)
        {
            script.enabled = false;
        }

      
    }

 

public void EnableComponents(GameObject hitObject)
{
    // Enable Nav Mesh Agent component
    NavMeshAgent navMeshAgent = hitObject.GetComponent<NavMeshAgent>();
    if (navMeshAgent != null)
    {
        navMeshAgent.enabled = true;
    }

    // Enable AI Animation Controller component
    AIAnimationController aiAnimationController = hitObject.GetComponent<AIAnimationController>();
    if (aiAnimationController != null)
    {
        aiAnimationController.enabled = true;
    }

    // Enable Animator component
    Animator animator = hitObject.GetComponent<Animator>();
    if (animator != null)
    {
        animator.enabled = true;
    }

    // Disable EnemyMovement.cs
    EnemyMovement EnemyMovement = hitObject.GetComponent<EnemyMovement>();
    if (EnemyMovement != null)
    {
        EnemyMovement.enabled = true;
    }

    // Enable _AI_AgentBrain child object
    Transform aiAgentBrain = hitObject.transform.Find("_AI_AgentBrain");
    if (aiAgentBrain != null)
    {
        aiAgentBrain.gameObject.SetActive(true);
    }

    // Enable RigidbodyFollowPlayer component if present
    RigidbodyFollowPlayer rigidbodyFollowPlayer = hitObject.GetComponent<RigidbodyFollowPlayer>();
    if (rigidbodyFollowPlayer != null)
    {
        rigidbodyFollowPlayer.enabled = true;
    }

    // Enable MaintainYHeight component if present
    MaintainYHeight maintainYHeight = hitObject.GetComponent<MaintainYHeight>();
    if (maintainYHeight != null)
    {
        maintainYHeight.enabled = true;
    }

    // Disable colliders in the armature
    Collider[] colliders = hitObject.GetComponentsInChildren<Collider>();
    foreach (Collider collider in colliders)
    {
        // Skip the root object Collider
        if (collider.gameObject == hitObject)
        {
            continue;
        }
        
        collider.enabled = false;
    }


    // Make rigidbodies in the armature kinematic and reset drag
    // Make rigidbodies in the armature non-kinematic
    Rigidbody[] rigidbodies = hitObject.GetComponentsInChildren<Rigidbody>();
    foreach (Rigidbody rigidbody in rigidbodies)
    {
        // Skip the root object Rigidbody
        if (rigidbody.gameObject == hitObject)
        {
            continue;
        }

        rigidbody.isKinematic = false;
        rigidbody.drag = 1f; // Set the original value before death
    }



    // Enable all LookAt scripts
    LookAtPlayer[] lookAtScripts = hitObject.GetComponentsInChildren<LookAtPlayer>();
    foreach (LookAtPlayer script in lookAtScripts)
    {
        script.enabled = true;
    }

    // Re-parent the child objects that were unparented and destroyed on death.
    // Note: This will be a bit tricky, as it seems you're completely destroying the child objects in the UnParentAndRemoveChildren method.
    // You might need to modify the way you handle those objects on death, or have a separate pool of those child objects that you can pull from to replace the destroyed ones.
}



}
