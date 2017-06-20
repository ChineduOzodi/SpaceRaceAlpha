using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class CraftIconController : Controller<CraftModel> {

    //width of icon
    internal float iconSize = .05f;
    public float zoomMod = 1;

    Camera mainCam;
    CameraController cam;
    SpriteRenderer sprite;
    LineRenderer line;
    SpaceTrajectory spaceT;

    double distanceModifier;
    

    protected override void OnInitialize()
    {
        name = model.name + " Icon";

        //Add listeners
        Message.AddListener<SetCameraView>(CameraViewChanged);

        //Set Cameras
        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraController>();

        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();

        //Instantiate space trajectory
        spaceT = gameObject.AddComponent<SpaceTrajectory>();
        spaceT.model = model;

        //Get Relevant information
        distanceModifier = cam.distanceModifier;

        //Set Icon View Mode
        SetIconMode(cam.cameraView, cam.distanceModifier, cam.reference);
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
            line.enabled = true;
            spaceT.enabled = true;

            sprite.enabled = true;

            transform.localScale = Vector3.one * (Mathf.Pow(iconSize * mainCam.orthographicSize, .8f));
        }
        else if (cameraView == CameraView.Planet)
        {
            sprite.enabled = false;

            if (refer.name == model.reference.Model.name || refer.name == model.reference.Model.reference.Model.name)
            {
                spaceT.enabled = true;
                line.enabled = true;
                sprite.enabled = true;

                if (model.State == ObjectState.Landed)
                {
                    line.enabled = false;
                    spaceT.enabled = false;
                }
            }
            else
            {
                line.enabled = false;
                spaceT.enabled = false;
            }

        }
        else if (cameraView == CameraView.Surface)
        {
            sprite.enabled = false;
            line.enabled = false;
            spaceT.enabled = false;

        }
    }
	
	// Update is called once per frame
	void Update () {
        if (cam.cameraView == CameraView.System)
        {
            transform.position = (Vector3)((model.SystemPosition - cam.reference.SystemPosition) / distanceModifier);

            transform.localScale = Vector3.one * Mathf.Pow(iconSize * mainCam.orthographicSize * zoomMod, .7f);

            transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
        }
        else if (cam.cameraView == CameraView.Planet)
        {
            if (sprite.enabled)
            {
                transform.position = (Vector3)((model.SystemPosition - cam.reference.SystemPosition) / distanceModifier);
                transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
                transform.localScale = Vector3.one * Mathf.Pow(iconSize * mainCam.orthographicSize * zoomMod, .7f);

            }
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

    public CraftModel GetModel()
    {
        return model;
    }
}
