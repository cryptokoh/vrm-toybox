using UnityEngine;
using System;
using System.Collections;

public class WallRun : MonoBehaviour
{
    public Rigidbody playerRigidbody;
    public float wallRunSpeed = 5f;
    public float wallRunDuration = 2f;
    public float wallJumpForce = 15f;
    public string wallTag = "Wall";

    private bool isWallRunning = false;
    private float wallRunTime = 0f;
    private Vector3 wallNormal;
    private Vector3 wallRunDirection;

    public Camera playerCamera;
    public float wallRunFOV = 90f;
    public float fovTransitionTime = 0.5f;
    private float defaultFOV;

    public static event Action<bool> OnWallRun;

    private void Start()
    {
        defaultFOV = playerCamera.fieldOfView;
    }

    void FixedUpdate()
    {
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (isWallRunning)
        {
            wallRunTime += Time.fixedDeltaTime;

            // Move the player along the wall
            playerRigidbody.velocity = wallRunDirection * wallRunSpeed;

            if (wallRunTime >= wallRunDuration || !isHoldingShift)
            {
                StopWallRun();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallJump();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isWallRunning && CheckWallRunConditions(collision))
        {
            StartWallRun(collision.contacts[0].normal);
        }
    }

    private bool CheckWallRunConditions(Collision collision)
    {
        if (!collision.collider.CompareTag(wallTag))
            return false;

        // Check if the collision is not with the floor
        if (Mathf.Abs(collision.contacts[0].normal.y) > 0.9f)
            return false;

        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!isHoldingShift)
            return false;

        return true;
    }

    private void StartWallRun(Vector3 normal)
    {
        isWallRunning = true;
        wallRunTime = 0f;
        wallNormal = normal;

        // Determine the direction to run along the wall
        wallRunDirection = Vector3.Cross(normal, Vector3.up);
        if (Vector3.Dot(wallRunDirection, playerRigidbody.velocity) < 0)
        {
            // If the current velocity is in the opposite direction, reverse the run direction
            wallRunDirection *= -1;
        }

        // Set the rotation of the player to face the direction of wall running
        transform.rotation = Quaternion.LookRotation(wallRunDirection, Vector3.up);

        // Increase camera field of view
        StartCoroutine(ChangeFOV(wallRunFOV, fovTransitionTime));

        OnWallRun?.Invoke(true);
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        wallRunTime = 0f;

        // Reset horizontal velocity
        playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);

        // Reset camera field of view
        StartCoroutine(ChangeFOV(defaultFOV, fovTransitionTime));

        OnWallRun?.Invoke(false);
    }

private IEnumerator ChangeFOV(float targetFOV, float duration)
{
    float startFOV = playerCamera.fieldOfView;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        playerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / duration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    playerCamera.fieldOfView = targetFOV;
}

    private void WallJump()
    {
        isWallRunning = false;
        wallRunTime = 0f;

        // Add a force in the direction opposite to the wall and upwards
        playerRigidbody.AddForce(-wallNormal * wallJumpForce + Vector3.up * wallJumpForce);
    }
}
