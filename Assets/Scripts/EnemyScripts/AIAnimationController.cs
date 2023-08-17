using UnityEngine;
using UnityEngine.AI;

public class AIAnimationController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Check if the agent is moving
        bool isMoving = agent.velocity.magnitude > 0.1f;

        // Update the animator parameters
        animator.SetBool("isMoving", isMoving);

        // Get the normalized speed of the agent
        float normalizedSpeed = agent.speed / agent.speed;

        // Update the "speed" parameter of the blend tree
        animator.SetFloat("speed", normalizedSpeed);
    }

    
}
