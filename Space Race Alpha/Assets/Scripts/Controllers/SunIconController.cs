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
        model.color = Color.yellow;

        //Get Relevant information
        distanceModifier = mainCam.GetComponent<CameraController>().distanceModifier;

        width =(float) (model.radius / (Units.Mm * 10));
    }

    private void CameraViewChanged(SetCameraView m)
    {
        SetIconMode(m.cameraView, m.distanceModifier, m.reference);
    }

    private void SetIconMode(CameraView cameraView, double distanceModifier, SolarBodyModel refer)
    {
        this.distanceModifier = distanceModifier;

        if (cameraView == CameraView.System)
        {
            transform.localScale = Vector3.one * (Mathf.Pow(width * mainCam.orthographicSize * zoomMod, .8f));
            transform.localScale = Vector3.one * width;
        }
        else if (cameraView == CameraView.Planet)
        {
            //Don't really need to change anything because the layer mask should be set to only visible in solar system view
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (cam.cameraView == CameraView.System)
        {
            transform.position = (Vector3)((model.SystemPosition - cam.reference.SystemPosition) / distanceModifier);
            transform.localScale = Vector3.one * (Mathf.Pow(width * mainCam.orthographicSize * zoomMod, .8f));
        }   
    }

    public void OnMouseEnter()
    {
        LabelCanvas.instance.SetLabel(gameObject, model.name);
    }
    public void OnMouseExit()
    {
        LabelCanvas.instance.CancelLabel();
    }
}
