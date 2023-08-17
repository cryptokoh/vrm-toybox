using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private PlayerHealthBar playerHealthBar;
    public GameObject gameOverUI;
    public Button respawnButton;
    public Button exitGameButton;
    public Button mainMenuButton;


    // Start is called before the first frame update
    void Start()
    {
        playerHealthBar = FindFirstObjectByType<PlayerHealthBar>();
        gameOverUI.SetActive(false);

        respawnButton.onClick.AddListener(Respawn);
        exitGameButton.onClick.AddListener(ExitGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealthBar != null)
        {

            if (playerHealthBar.currentHealth <= 0 ) {

                gameOverUI.SetActive(true);
                ShowAndCenterMouse();
                //SceneManager.LoadScene(1);
            }
        }
    }

    void ShowAndCenterMouse()
    {
        // Make the cursor visible
        Cursor.visible = true;

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;

        // Center the cursor (optional)
        //Vector3 centeredMousePosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //Input.mousePosition = centeredMousePosition;
    }

    void Respawn()
    {
        // This will reload the current scene.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(1);
    }

    void ExitGame()
    {
        // If you're in the Unity editor, this will stop play mode. If you're in a built game, this will close the game.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    void GoToMainMenu()
    {
        // Assuming your main menu is the first scene in the build settings.
        // Adjust the scene index if it's different.
        SceneManager.LoadScene(0);
    }
}
