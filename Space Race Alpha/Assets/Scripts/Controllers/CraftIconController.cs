using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class CraftIconController : Controller<CraftModel> {

    //width of icon
    internal float iconSize = .05f;
    public float zoomMod = 1;
    Camera mainCam;
    CameraController cam;

    double distanceModifier;

    protected override void OnInitialize()
    {
        name = model.name + " Icon";

        //Add listeners
        Message.AddListener<SetCameraView>(CameraViewChanged);

        //Set Cameras
        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraController>();

        //Instantiate space trajectory
        gameObject.AddComponent<SpaceTrajectory>().model = model;
        
        //Get Relevant information
        distanceModifier = cam.distanceModifier;
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
            //Currently Does nothing because the icon does nothing
        }
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = (Vector3)((model.SystemPosition - cam.reference.SystemPosition) / distanceModifier);
        transform.localScale = Vector3.one * Mathf.Pow(iconSize * mainCam.orthographicSize * zoomMod, .8f);
        transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
    }
}
