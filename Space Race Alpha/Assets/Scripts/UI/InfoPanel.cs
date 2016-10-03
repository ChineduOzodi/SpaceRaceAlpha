using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CodeControl;
using System;

public class InfoPanel : MonoBehaviour {

    internal string targetName;
    internal string info;

    public Text nameText;
    public Text infoText;

    internal BaseModel model;

	void Awake()
    {
        //Add listener
        Message.AddListener<InfoPanelMessage>(OnInfoPanelMessage);

        //set text variables and instantiate text objects

        nameText = Instantiate(nameText, transform) as Text;

        infoText = Instantiate(infoText, transform) as Text;

        
    }

    private void OnInfoPanelMessage(InfoPanelMessage m)
    {
        model = m.model;
    }

    void Update()
    {
        if (model != null)
        {
            nameText.text = model.name;
            infoText.text = OrbitalInfo.GetInfo(model, Forces.G);
        }
    }
}
