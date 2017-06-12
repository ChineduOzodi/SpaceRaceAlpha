using CodeControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameCanvas : MonoBehaviour {

    public GameObject craftPanels;
	// Use this for initialization
	void Start () {
        //Add listener
        Message.AddListener<InfoPanelMessage>(OnInfoPanelMessage);

        craftPanels.SetActive(false);
    }

    private void OnInfoPanelMessage(InfoPanelMessage m)
    {

        if (m.model.Type == ObjectType.Spacecraft)
            craftPanels.SetActive(true);
        else craftPanels.SetActive(false);
    }
}
