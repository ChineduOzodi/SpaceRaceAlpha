using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class CraftIconController : Controller<CraftModel> {
    bool mapMode = false;

    //width of icon
    internal float iconSize = .05f;

    Camera mainCam;
    Camera mapCam;


    protected override void OnInitialize()
    {
        //Add listeners
        Message.AddListener<ToggleMapMessage>(ToggleMapMode);

        //Set Cameras
        mainCam = Camera.main;
        mapCam = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();

        //Instantiate space trajectory
        gameObject.AddComponent<SpaceTrajectory>().model = model;
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
        transform.position = (Vector3)(model.position / Units.Mm);

        if (mapMode)
        {
            transform.localScale = Vector3.one * (Mathf.Pow(iconSize * mainCam.orthographicSize, .8f));
        }
        else
            transform.localScale = Vector3.one * Mathf.Pow(iconSize * mapCam.orthographicSize, .8f);
        transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
    }
}
