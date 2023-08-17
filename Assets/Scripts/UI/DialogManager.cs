using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public Transform buttonContainer;
    public TMP_Text questionText;
    public Button[] answerButtons;
    public Button exitButton;
    public GameObject dialogWindow;
    public Animator anim;  // Animator reference

    [TextArea(3, 10)]
    public string dialogJson;

    private DialogNode dialogData;
    private DialogNode currentQuestion;
    private DialogNode dialogRoot;  // To remember the root of your dialog
    private MonoBehaviour[] playerScripts;
        private ThirdPersonOrbitCamBasic thirdPersonCamera;  // ThirdPersonOrbitCamBasic reference


    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerScripts = player.GetComponents<MonoBehaviour>();

            // Find the Avatar child and get its animator
            GameObject avatarChild = GameObject.FindGameObjectWithTag("Avatar");
            if (avatarChild != null)
            {
                anim = avatarChild.GetComponent<Animator>();
                
            }
        }

        exitButton.onClick.AddListener(HideDialogWindow);

        // Get reference to ThirdPersonOrbitCamBasic
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            thirdPersonCamera = mainCamera.GetComponent<ThirdPersonOrbitCamBasic>();
        }

        if (!string.IsNullOrEmpty(dialogJson))
        {
            DialogRoot dialogObj = JsonUtility.FromJson<DialogRoot>(dialogJson);

            if (dialogObj != null && dialogObj.dialogue.Count > 0)
            {
                dialogRoot = dialogObj.dialogue[0]; // remember the root node
                DisplayNode(dialogRoot);
            }
            else
            {
                Debug.LogError("Dialog data is empty or invalid.");
            }
        }
        else
        {
            Debug.LogError("Dialog JSON string is empty.");
        }
    }

    void DisplayNode(DialogNode node)
    {
        currentQuestion = node;
        questionText.text = currentQuestion.question;

        int i = 0;
        foreach (var answer in currentQuestion.answers)
        {
            if (i >= answerButtons.Length)
                break;

            answerButtons[i].gameObject.SetActive(true);
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = answer.answer;
            int answerIndex = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => SelectAnswer(answerIndex));
            i++;
        }

        // Disable remaining buttons if there are fewer answers than buttons
        for (int j = i; j < answerButtons.Length; j++)
        {
            answerButtons[j].gameObject.SetActive(false);
        }

        // Check if we are at the end of the dialogue
        if (currentQuestion.answers == null || currentQuestion.answers.Count == 0)
        {
            EndOfDialog();
        }
    }

    public void SelectAnswer(int answerIndex)
    {
        var selectedAnswer = currentQuestion.answers[answerIndex];
        if (selectedAnswer != null && selectedAnswer.response != null && selectedAnswer.response.Count > 0)
        {
            DisplayNode(selectedAnswer.response[0]); // consider the first response as the next dialog node
        }
        else
        {
            Debug.LogError("Selected answer is null or invalid.");
        }
    }

    public void HideDialogWindow()
    {
        dialogWindow.SetActive(false);
        ResetDialog();
        // Enable all scripts on player when dialog is exited
        if (playerScripts != null)
        {
            foreach (var script in playerScripts)
            {
                script.enabled = true;
            }
        }

        // Enable ThirdPersonOrbitCamBasic when dialog is exited
        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.enabled = true;
        }
    }


    public void ResetDialog()
    {
        DisplayNode(dialogRoot);  // Reset to the root when the dialog is exited
    }

    void EndOfDialog()
    {
        Debug.Log("You've reached the end of the dialog!");
    }
}

[System.Serializable]
public class DialogRoot
{
    public string npcName;
    public List<DialogNode> dialogue;
}

[System.Serializable]
public class DialogNode
{
    public string question;
    public List<AnswerNode> answers;
}

[System.Serializable]
public class AnswerNode
{
    public string answer;
    public List<DialogNode> response;
}
