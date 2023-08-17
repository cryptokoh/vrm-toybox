using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance; // Singleton instance
    
    public ScrollRect scrollRect;


    
    public string username;
    
    public int maxMessages = 25;

    public GameObject chatPanel, textObject;
    public TMP_InputField chatBox;

    public Color playerMessage, info, totalDamage;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject); // Ensures only one instance exists

        // Add any other initialization code here
    }

    // Update is called once per frame
    void Update()
    {
        if(chatBox.text != "")
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
            SendMessageToChat(username + ": " + chatBox.text, Message.MessageType.playerMessage);
            chatBox.text = "";
            }

        }
        else
        {
            if(!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return)){
                    chatBox.ActivateInputField();
            }
        }
        
        if(!chatBox.isFocused)
        {
                    if(Input.GetKeyDown(KeyCode.Space))
                    {
                    SendMessageToChat("Debug: " + "you pressed the space key", Message.MessageType.info);
                    
                    }

        }



    }

    public void SendMessageToChat(string text, Message.MessageType messageType){
        if(messageList.Count >= maxMessages){
            Destroy(messageList[0].textObject.gameObject);
        messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI >();

        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        messageList.Add(newMessage);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;  // 0 for bottom, 1 for top
    }

    Color MessageTypeColor(Message.MessageType messageType){
        Color color = info;

        switch(messageType)
    {
        case Message.MessageType.playerMessage:
            color = playerMessage; 
            break; // Add break here

        case Message.MessageType.totalDamage:
            color = totalDamage;
            break;
    }

    return color;
    }

}

[System.Serializable]
public class Message
{
    public string text;
    public TextMeshProUGUI  textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage,
        info,
        totalDamage
    }
}