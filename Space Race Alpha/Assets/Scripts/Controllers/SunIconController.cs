using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class SunIconController : Controller<SunModel> {
    bool mapMode = false;

    internal float width = 1;

    Camera mainCam;
    Camera mapCam;


    protected override void OnInitialize()
    {
        //Add listeners
        Message.AddListener<ToggleMapMessage>(ToggleMapMode);

        //Set Cameras
        mainCam = Camera.main;
        mapCam = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();

        transform.position = (Vector3) (model.position / Units.Gm);
    }

    private void ToggleMapMode(ToggleMapMessage m)
    {
        mapMode = m.mapMode;
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = (Vector3)(model.position / Units.Gm);

        if (mapMode)
        {
            transform.localScale = Vector3.one * width * mainCam.orthographicSize;
        }
        else
            transform.localScale = Vector3.one * width * mapCam.orthographicSize;

    }
}
