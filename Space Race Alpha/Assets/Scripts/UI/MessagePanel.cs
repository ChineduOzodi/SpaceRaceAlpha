using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour {

    public GameObject messageBox;
	// Use this for initialization
	void Start () {

        Message.AddListener<MessagePanelMessage>(OnMessagePanelMessage);
	
	}

    private void OnMessagePanelMessage(MessagePanelMessage m)
    {
        GameObject message = Instantiate(messageBox, transform) as GameObject;
        Text messageText = message.GetComponentInChildren<Text>();
        Image messageBackground = message.GetComponent<Image>();
        messageText.text = m.message;

        if (m.color != null)
        {
            m.color.a = .33f;
            messageBackground.color = m.color;
        }

        Destroy(message, m.duration * Time.timeScale);
    }

    public static void SendMessage(string message, float duration, Color color)
    {
        MessagePanelMessage m = new MessagePanelMessage();
        m.color = color;
        m.message = message;
        m.duration = duration;

        Message.Send(m);
    }
}
