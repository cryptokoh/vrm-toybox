using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Animator component of the player character.")]
    public Animator anim;                          
    [Tooltip("The Collider component of the player character.")]
    public Collider characterCollider;             
    [Tooltip("The Rigidbody component of the player character.")]
    public Rigidbody characterRigidBody;           
    [Tooltip("The speed at which the character walks.")]
    public float walkSpeed = 0.15f;                
    [Tooltip("Name of the input that makes the player sprint.")]
    public string sprintButton;                 
    [Tooltip("The speed at which the character sprints.")]
    public float sprintSpeed = 2.0f;                
    [Tooltip("Time taken for the character to change speeds.")]
    public float speedDampTime = 0.1f;       
    [Header("Jump Settings")]      
    [Tooltip("Name of the input that makes the player jump.")]
    public string jumpButton;             
    [Tooltip("How high the character jumps.")]
    public float jumpHeight = 1.5f;                
    [Tooltip("The force applied to the jump (used for double jumps).")]
    public float jumpIntertialForce = 10f;
    [Tooltip("Maximum number of times the player can jump without touching the ground.")]
    public int maxJumpCount = 2;      
    [Tooltip("Whether double jump is enabled or not.")]
    public bool enableDoubleJump = true;
    // Counter for the number of jumps performed.
    private int jumpCount = 0;    
    // Variables for determining the current and target speeds.
    private float speed, speedSeeker;              
    // Animator parameter ID for the jump bool. 
    private int jumpBool;                          
    // Animator parameter ID for the grounded bool.
    private int groundedBool;
    // Flag for whether the player is trying to jump.  
    private bool jump;                             
    // Flag for whether the player is sprinting.
    private bool isSprinting = false;
    // Animator parameter ID for the speed float.
    private int speedFloat;
    [Tooltip("Delay to ensure correct jump behaviour on slopes.")]
    public float groundedDelay = 0.2f; 
    // Time when the last jump occurred.
    private float lastJumpTime = 0.0f;
    [Header("Turn Settings")]             
    [Tooltip("Smoothing applied when the character turns.")]
    public float turnSmoothing = 0.06f; 
    // Current horizontal input.
    private float horizontalInput;
    // Current vertical input.
    private float verticalInput;
    [Header("Camera Settings")]
    [Tooltip("The Camera following the player.")]
    public Camera playerCamera;
    [Tooltip("The field of view when the player is sprinting.")]
    public float sprintFOV = 75f;
    [Tooltip("The field of view when the player is not sprinting.")]
    public float normalFOV = 60f;
    [Tooltip("The speed at which the camera transitions between FOVs.")]
    public float FOVLerpSpeed = 3f;
    // The last direction that the player was moving in.
    private Vector3 lastDirection;
    // Variables to hold input values.
    private float h, v; 
    // Another reference to the player's Rigidbody.
    private Rigidbody rBody;
    [Header("Dodge Settings")]
    [Tooltip("Name of the input that makes the player dodge/roll.")]
    public string dodgeButton;
    [Tooltip("The duration of the dodge/roll action.")]
    public float dodgeTime = 0.5f; // How long the dodge lasts
    [Tooltip("The speed multiplier during dodge/roll.")]
    public float dodgeSpeed = 2f;  // How much faster you move during a dodge
    private bool isDodging = false;
    [Header("Slide Settings")]
    [Tooltip("The angle above which a slope is considered slideable.")]
    public float slideSlopeThreshold = 45.0f;
    [Tooltip("The speed at which the character slides.")]
    public bool isPlayerOnSlope;
    public float slideSpeed = 2.5f;
    [Tooltip("Flag for whether the player is sliding.")]
    private bool isSliding = false; // Add this line
    [Tooltip("The additional gravity applied to the player while sliding.")]
    public float slidingGravity = 5f;
    private float lastShiftTime = -1;
    public float doublePressThreshold = 0.3f;  // Adjust this as needed
    private RaycastHit hitInfo;
    private bool didRaycastHit;
    [Tooltip("This might change depending on your character setup")]
    public float distanceToGround = 1.0f; // 
    [Tooltip("This will control how fast the rotation will happen")]
    public float rotationSpeed = 5.0f; // 
    [Tooltip("This is the speed at which the player will rotate back upright. Adjust this value as needed.")]
    public float uprightRotateSpeed = 1.0f; // 
    [Tooltip("This is the amount of slide force when hitting obejcts")]
    public int slideForce;
    [Tooltip("This is the amount of slide damage")]
    public int slideDamage;
    // Variable to track whether player input should be enabled
    private bool inputEnabled = true;

    private MovingWalkable currentMovingWalkable;
    private Vector3 lastPlatformVelocity;
    private Rigidbody platformRigidBody;

    void Start()
    {
        jumpBool = Animator.StringToHash("Jump");
        groundedBool = Animator.StringToHash("Grounded");
        speedFloat = Animator.StringToHash("Speed");
        anim.SetBool(groundedBool, true);

        if (characterCollider == null)
        {
            characterCollider = GetComponent<Collider>();
        }

        if (characterRigidBody == null)
        {
            characterRigidBody = GetComponent<Rigidbody>();
        }
        rBody = characterRigidBody;

        

    }

    void Update()
    {
        PerformRaycastDown();
        if (inputEnabled)
        {
                    horizontalInput = Input.GetAxis("Horizontal");
                    verticalInput = Input.GetAxis("Vertical");
        }

        bool isGrounded = IsGrounded();
        anim.SetBool(groundedBool, isGrounded);

        if (!jump && Input.GetButtonDown(jumpButton) && jumpCount < maxJumpCount)
        {
            jump = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, Time.deltaTime * FOVLerpSpeed);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * FOVLerpSpeed);
        }

        if (Input.GetButtonDown(dodgeButton) && !isDodging)
        {
            StartCoroutine(DodgeRoll());
        }

            if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //Debug.Log("Shift key pressed at time: " + Time.time);
            
            if ((Time.time - lastShiftTime) < doublePressThreshold)
            {
                //Debug.Log("Double press detected. Time between presses: " + (Time.time - lastShiftTime));
                // If shift was pressed twice in a short time, start sliding
                StartCoroutine(StartSlide());
            }

            lastShiftTime = Time.time;
        }


        
            
        

       

        if (isSliding)
        {
            characterRigidBody.AddForce(Physics.gravity * slidingGravity, ForceMode.Acceleration);
        }

        MovementManagement(horizontalInput, verticalInput, anim.GetBool("Slide"));

        if(!IsMoving()) 
        {
            RotateUpright();
        }
    
    }

    void FixedUpdate()
    {
        JumpManagement();

            if (IsOnPlatform())
            {
                // Calculate the platform's change in velocity since the last frame
                Vector3 platformDeltaV = platformRigidBody.velocity - lastPlatformVelocity;

                // Apply an equal and opposite force to the player, effectively cancelling out the platform's acceleration
                characterRigidBody.AddForce(-platformDeltaV * characterRigidBody.mass, ForceMode.Impulse);
            }

        lastPlatformVelocity = platformRigidBody != null ? platformRigidBody.velocity : lastPlatformVelocity;
    }

    void MovementManagement(float horizontal, float vertical, bool isSliding)
    {
        // Transform the movement vector from local space to world space, relative to the camera's rotation
        Vector3 moveDir = playerCamera.transform.right * horizontal + playerCamera.transform.forward * vertical;
        moveDir.y = 0; // Keep the movement in the horizontal plane
        anim.SetFloat("H", horizontal);
        anim.SetFloat("V", vertical);

        if (IsGrounded())
        {
            characterRigidBody.useGravity = true;
            RaycastHit hit;
            if (Physics.Raycast(characterRigidBody.position, Vector3.down, out hit, 1f))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                // Consider slopes with an angle less than 45 degrees
                if (slopeAngle < 45f)
                {
                    // Calculate the move direction on the slope
                    moveDir = Vector3.ProjectOnPlane(moveDir, hit.normal);
                }
            }
        }
        else if (!anim.GetBool(jumpBool) && characterRigidBody.velocity.y > 0)
        {
            RemoveVerticalVelocity();
        }

        // Replace Rotating(horizontal, vertical) with a new rotation method
        Rotating(moveDir);

        // The magnitude of moveDir may not be 1 if the inputs are non-zero. So normalize it.
        speed = moveDir.normalized.magnitude;

        CheckIfDodging();

        anim.SetFloat(speedFloat, speed * speedSeeker, speedDampTime, Time.deltaTime);

        // Use moveDir instead of transform.forward
        characterRigidBody.MovePosition(characterRigidBody.position + moveDir * speed * speedSeeker * Time.deltaTime);
    }


    private void PerformRaycastDown()
    {
        didRaycastHit = Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1f);
    }

    private void CheckIfDodging()
    {
        if (!isDodging) 
        {
            if (isSprinting && !isSliding)
            {
                speedSeeker = sprintSpeed;
            }
            else if (isSliding)
            {
                speedSeeker = slideSpeed;
            }
            else
            {
                speedSeeker = walkSpeed;
            }
        }
    }

    private void Rotating(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSmoothing * Time.deltaTime);
        }
    }

   private void JumpManagement()
    {
        if (jump && jumpCount < (enableDoubleJump ? maxJumpCount : 1) && (IsGrounded() || jumpCount > 0))
        {
            if (jumpCount == 0)
            {
                jumpCount++;
                isSliding = false;
                //Debug.Log("Jump initiated - count: " + jumpCount);
                anim.SetBool(jumpBool, true);
                PerformJump();
            }
            else if (jumpCount == 1 && enableDoubleJump)
            {
                jumpCount++;
                //Debug.Log("Double jump initiated - count: " + jumpCount);
                anim.SetBool(jumpBool, true);
                PerformJump();
            }
            jump = false;
        }
        else if (!IsGrounded()) // Changed this condition
        {
            jump = false;
        }
        else if (anim.GetBool(jumpBool))
        {
            if (IsGrounded())
            {
                jumpCount = 0;
                anim.SetBool(groundedBool, true);
                characterCollider.material.dynamicFriction = 0.6f;
                characterCollider.material.staticFriction = 0.6f;
                jump = false;
                anim.SetBool(jumpBool, false);
            }
        }
    }

    private void PerformJump()
    {
        //Debug.Log("Performing jump - count: " + jumpCount);
        Debug.DrawRay(characterRigidBody.position, Vector3.up * 5, Color.yellow, 1.0f);
        characterCollider.material.dynamicFriction = 0f;
        characterCollider.material.staticFriction = 0f;
        float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
        velocity = Mathf.Sqrt(velocity);
        float jumpFactor = 1.1f;  // Adjust this value as needed
        velocity *= jumpFactor;   // Apply the jump factor
        characterRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
        lastJumpTime = Time.time;  // Remember the time of this jump
    }


    public bool IsGrounded()
    {
        if (Time.time < lastJumpTime + groundedDelay)
        {
            return false;  // Not enough time has passed since the last jump - consider as not grounded
        }

        float extraHeightText = 0.1f;
        Debug.DrawRay(characterCollider.bounds.center, -Vector3.up * (characterCollider.bounds.extents.y + extraHeightText), didRaycastHit ? Color.green : Color.red);
        return didRaycastHit;
    }

   private bool IsOnSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down * 1f, Color.green);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f)) 
        {
            // Check for both angle and movement direction
            if (Vector3.Angle(hit.normal, Vector3.up) > slideSlopeThreshold) 
            {
                isPlayerOnSlope = true;
                return true;
            }
            else 
            {
                isPlayerOnSlope = false;
                return false;
            }
        }
        return false;
    }

    private float SlopeAngle()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
        {
            return Vector3.Angle(hit.normal, Vector3.up);
        }
        return 0f;
    }

    IEnumerator DodgeRoll() 
    {
        float originalSpeedSeeker = speedSeeker; 
        speedSeeker = dodgeSpeed;
        isDodging = true;
        anim.SetBool("DodgeRoll", true);
        //Debug.Log("Player is now dodging.");

        // Wait for the dodge to complete
        yield return new WaitForSeconds(dodgeTime);
        speedSeeker = originalSpeedSeeker;

        isDodging = false;
        anim.SetBool("DodgeRoll", false);
        
    }

    private void RemoveVerticalVelocity()
    {
        Vector3 horizontalVelocity = characterRigidBody.velocity;
        horizontalVelocity.y = 0;
        characterRigidBody.velocity = horizontalVelocity;
    }

	public bool IsMoving()
	{
        return (characterRigidBody.velocity.magnitude > 0.01f && horizontalInput != 0) || (verticalInput != 0);

	}

    // Function to rotate player upright smoothly
    private void RotateUpright()
    {
        Quaternion uprightRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, uprightRotation, Time.deltaTime * uprightRotateSpeed);
    }

    IEnumerator StartSlide()
    {
        isSprinting = true;
        isSliding = true;

         // Instead of simply checking if we're on a slope, now also check the slope angle.
        if (IsOnSlope() && IsGrounded() && IsMoving() && SlopeAngle() > slideSlopeThreshold)
            {
                // Add gravity temporarily for sliding.
                characterRigidBody.AddForce(Physics.gravity);
                //Debug.Log("Player is now sliding.");
                anim.SetBool("Slide", true);
            }


        while (IsOnSlope())
        {
            yield return null;
        }

        isSprinting = false;
        isSliding = false;

          // Remove the temporarily added gravity.
        characterRigidBody.AddForce(-Physics.gravity);
        //Debug.Log("Player has stopped sliding.");
        anim.SetBool("Slide", false);
    }

    private void OnEnable()
    {
        // Subscribe to the event
        WallRun.OnWallRun += HandleWallRun;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when the object is disabled to prevent memory leaks
        WallRun.OnWallRun -= HandleWallRun;
    }

    private void HandleWallRun(bool isRunning)
    {
        // If isRunning is true, disable input. If false, enable input.
        inputEnabled = !isRunning;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(isSliding) // Only react to collisions if the character is sliding
        {
            // Get the direction of the slide relative to the hit object
            Vector3 slideDirection = (collision.transform.position - transform.position).normalized;

            // Check if the collided object has a Rigidbody to apply forces to
            Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();
            if(hitRb != null)
            {
                // Apply a force to the hit object
                hitRb.AddForce(slideDirection * slideForce, ForceMode.Impulse);
            }

           HealthBar healthBar = collision.gameObject.GetComponent<HealthBar>();
            if(healthBar != null)
            {
                // Apply damage to the hit object
                healthBar.ApplyDamage(slideDamage);
            }
        }

 
    }



public void HandleEntityEnter(GameObject entity)
{
    if(entity == this.gameObject)
    {
        transform.parent = currentMovingWalkable.transform;
        Debug.Log("Player is on platform.");
    }
}

public void HandleEntityExit(GameObject entity)
{
    if(entity == this.gameObject)
    {
        transform.parent = null;
        Debug.Log("Player has left platform.");
    }
}

void OnTriggerEnter(Collider other)
{
    MovingWalkable movingWalkable = other.gameObject.GetComponent<MovingWalkable>();
    if(movingWalkable != null)
    {
        currentMovingWalkable = movingWalkable;
        platformRigidBody = movingWalkable.GetComponent<Rigidbody>();  // Get the platform's rigidbody
        movingWalkable.OnEntityEnter += HandleEntityEnter;
        movingWalkable.OnEntityExit += HandleEntityExit;
    }
}

void OnTriggerExit(Collider other)
{
    MovingWalkable movingWalkable = other.gameObject.GetComponent<MovingWalkable>();
    if(movingWalkable != null && movingWalkable == currentMovingWalkable)
    {
        movingWalkable.OnEntityEnter -= HandleEntityEnter;
        movingWalkable.OnEntityExit -= HandleEntityExit;
        platformRigidBody = null;  // Clear the platform's rigidbody
        currentMovingWalkable = null;
    }
}

private bool IsOnPlatform()
{
    return platformRigidBody != null;
}


}


