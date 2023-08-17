using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitManagerUI : MonoBehaviour
{
    public static HitManagerUI instance; // Singleton instance
    
    public ScrollRect scrollRect;

    private string ObjectHit;
    
    public int maxMessages = 25;

    public GameObject chatPanel, textObject;
    //public TMP_InputField chatBox;

    public Color hitMessageColor, comboColor, expColor;

    [SerializeField]
    List<HitMessage> messageList = new List<HitMessage>();

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
   



    }

    public void SendMessageToChat(string text, HitMessage.MessageType messageType){
        if(messageList.Count >= maxMessages){
            Destroy(messageList[0].textObject.gameObject);
        messageList.Remove(messageList[0]);
        }

        HitMessage newMessage = new HitMessage();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI >();

        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        messageList.Add(newMessage);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;  // 0 for bottom, 1 for top
    }

    Color MessageTypeColor(HitMessage.MessageType messageType){
        Color color = hitMessageColor;

        switch(messageType)
    {
        case HitMessage.MessageType.combo:
            color = comboColor; 
            break; // Add break here

        case HitMessage.MessageType.exp:
            color = expColor;
            break;
    }

    return color;
    }

}

[System.Serializable]
public class HitMessage
{
    public string text;
    public TextMeshProUGUI  textObject;
    public MessageType messageType;

    public enum MessageType
    {
        hitMessage,
        combo,
        exp
    }
}