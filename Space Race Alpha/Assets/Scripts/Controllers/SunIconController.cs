using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class SunIconController : Controller<SunModel> {

    internal float width = 1;

    public float zoomMod = .01f;
    Camera mainCam;
    CameraController cam;
    double distanceModifier;

    //Model reference
    public PlanetModel Model;

    protected override void OnInitialize()
    {
        //Add listeners
        Message.AddListener<SetCameraView>(CameraViewChanged);

        //Set Model
        Model = model;
        name = model.name + " Icon";

        //Set Cameras
        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraController>();

        //Get Relevant information
        distanceModifier = mainCam.GetComponent<CameraController>().distanceModifier;

        width =(float) (model.radius / distanceModifier);
    }

    private void CameraViewChanged(SetCameraView m)
    {
        SetIconMode(m.cameraView, m.distanceModifier);
    }

    private void SetIconMode(CameraView cameraView, double distanceModifier)
    {
        this.distanceModifier = distanceModifier;
        if (cameraView == CameraView.System)
        {
            transform.localScale = Vector3.one * (Mathf.Pow(width * mainCam.orthographicSize * zoomMod, .8f));
            transform.localScale = Vector3.one * width;
        }
        else if (cameraView == CameraView.Planet)
        {

        }
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = (Vector3)((model.SystemPosition - cam.reference.SystemPosition) / distanceModifier);
        transform.localScale = Vector3.one * (Mathf.Pow(width * mainCam.orthographicSize * zoomMod, .8f));
    }
}
