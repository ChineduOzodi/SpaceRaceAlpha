using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class SunIconController : Controller<SunModel> {
    bool mapMode = false;

    internal float width = 1;

    Camera mainCam;
    Camera mapCam;

    //Model reference
    public PlanetModel Model;

    protected override void OnInitialize()
    {
        //Add listeners
        Message.AddListener<ToggleMapMessage>(ToggleMapMode);

        //Set Model
        Model = model;
        name = model.name + " Icon";

        width = (float)(model.radius / Units.Mm);

        //Set Cameras
        mainCam = Camera.main;
        mapCam = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();

        width =(float) (model.radius / Units.Mm);
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
            transform.localScale = Vector3.one * (Mathf.Pow(width * mainCam.orthographicSize, .8f));
        }
        else
            transform.localScale = Vector3.one * Mathf.Pow(width * mapCam.orthographicSize, .8f);

    }
}
