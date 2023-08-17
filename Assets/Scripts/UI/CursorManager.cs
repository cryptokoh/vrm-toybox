using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private bool isCursorVisible = false;

    void Start()
    {
        // Hide the cursor when the game starts
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check if the "Escape" key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the cursor visibility and update the isCursorVisible flag
            isCursorVisible = !isCursorVisible;
            Cursor.visible = isCursorVisible;
            Cursor.lockState = CursorLockMode.None; 

        }
    }
}
