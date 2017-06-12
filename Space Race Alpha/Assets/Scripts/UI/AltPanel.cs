using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class AltPanel : MonoBehaviour {

    CraftModel model;

    public Text textPref;
    internal Text infoText;

    Image image;

	// Use this for initialization
	void Awake () {
        //Add listener
        Message.AddListener<InfoPanelMessage>(OnInfoPanelMessage);

        image = GetComponent<Image>();
        //instantiate text
        infoText = Instantiate(textPref, transform) as Text;
        image.enabled = false;

    }

    internal void OnInfoPanelMessage(InfoPanelMessage m)
    {
        model = m.model as CraftModel;
        if (model != null)
            image.enabled = true;
        else image.enabled = false;
    }

    // Update is called once per frame
    void Update () {

        if (model != null)
        {
            infoText.text = model.alt.ToString("0") + " m| " + model.SurfaceVel.y.ToString("0.00") + " m/s";
        }

	
	}
}
