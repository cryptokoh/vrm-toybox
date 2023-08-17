using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button newGameButton;
    public Button exitButton;
    public Button optionsButton;
    public GameObject optionsPanel; // Assume this is a panel with various options/settings

    private void Start()
    {
        // Initially, make sure the options panel is hidden
        optionsPanel.SetActive(false);

        // Add listeners to the buttons
        newGameButton.onClick.AddListener(StartNewGame);
        exitButton.onClick.AddListener(ExitGame);
        optionsButton.onClick.AddListener(ToggleOptions);
    }

    void StartNewGame()
    {
        // Assuming the first game scene is indexed as 1 in the build settings.
        // Adjust the scene index if it's different.
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

    void ToggleOptions()
    {
        // This toggles the options panel on and off when you click the options button.
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }
}
