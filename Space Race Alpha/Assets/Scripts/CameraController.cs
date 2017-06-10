using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    public LayerMask surfaceViewMask;
    public LayerMask planetViewMask;
    public LayerMask systemViewMask;

    public CameraViewMode viewMode = CameraViewMode.Absolute;
    public ControlMode controlMode = ControlMode.Free;

    /// <summary>
    /// Camera position in the solar system from reference object
    /// </summary>
    internal Vector3d cameraPosition;

    internal GameObject target; //target object
    internal BaseModel targetModel; //targetModel

    internal BaseModel reference;
    internal double distanceModifier = Units.Mm * 10;

    bool initialized = false;
    

    //Cameras
    Camera mainCam;

    //Camera zoom and move modifiers
    public float camMoveSpeed = 1;
    public int zoomSpeed = 1;
    public float maxCamSize = 10000;
    public float minMapSize = 350; //the minmum size of the map view

    internal float planetViewSize = 500;
    internal float surfaceViewSize = 500;
    internal float systemViewSize = 500;
    //Background
    public GameObject stars;

    internal CameraView cameraView;

    // Use this for initialization
    void Start () {

        mainCam = GetComponent<Camera>();
        //initialize star background
        stars = Instantiate(Resources.Load("stars")) as GameObject;
    }

    public void SetCameraView(CameraView view)
    {
        if (view == CameraView.System)
        {
            cameraView = CameraView.System;
            distanceModifier = Units.Mm * 10;

            //Camera Settings
            Camera.main.cullingMask = systemViewMask;
            Camera.main.orthographicSize = systemViewSize;

            reference = GameController.instance.system.centerObject.Model;
        }

        //Send Message
        SetCameraView m = new SetCameraView();
        m.cameraView = cameraView;
        m.distanceModifier = distanceModifier;
        Message.Send(m);
    }

    // Update is called once per frame
    void Update()
    {

        if (!GameController.instance.setup)
        {
            float moveModifier = camMoveSpeed * mainCam.orthographicSize;
            mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;

            if (controlMode == ControlMode.Free)
            {
                //camera movement
                float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime;
                float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime;

                Camera.main.transform.Translate(new Vector3(transX, transY));
                cameraPosition = (Vector3d) (Camera.main.transform.position) * distanceModifier;
            }

            //Camera Zoom
            if (Input.GetKey(KeyCode.Equals))
            {
                mainCam.orthographicSize += zoomSpeed * moveModifier * .1f;
            }
            else if (Input.GetKey(KeyCode.Minus))
            {
                mainCam.orthographicSize -= zoomSpeed * moveModifier * .1f;
            }

            //Toggle Map Mode
            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleMapMode();
            }
            //Set View Mode
            if (Input.GetKeyDown(KeyCode.V))
            {
                SetViewMode((viewMode.GetHashCode() + 1 > 2) ? 0 : viewMode + 1);
            }

            //camera rotation
            if (viewMode == CameraViewMode.Absolute)
            {
                transform.rotation = Quaternion.identity;
            }
            else if (viewMode == CameraViewMode.Reference && targetModel != null)
            {
                //rot.eulerAngles = new Vector3(0, 0, new Polar2(targetModel.LocalPosition).angle * Mathf.Rad2Deg);
                transform.eulerAngles = new Vector3(0, 0, (float)((new Polar2(targetModel.LocalPosition).angle + targetModel.reference.Model.Rotation) * Mathd.Rad2Deg - 90)); //set rotation of compass needle
            }

            //Update Background
            stars.transform.position = new Vector3(transform.position.x, transform.position.y);
            stars.transform.localScale = new Vector3(mainCam.orthographicSize * .25f, mainCam.orthographicSize * .25f);

        } 
    }

    public void ToggleMapMode()
    {
        if (cameraView == CameraView.Surface)
        {
            SetCameraView(CameraView.Planet);
        }
        else
        {
            SetCameraView(CameraView.Planet);
        }
    }

    void FixedUpdate()
    {
        

        

        //if (mapMode)
        //{
        //    transform.position = new Vector3(0, 0, -1);
        //    mapCam.transform.position = new Vector3(0, 0, -1);
        //}
        //else
        //{
        //    transform.position = new Vector3(0, 0, -1);
        //    mapCam.transform.position = new Vector3(0, 0, -1);
        //}
    }
        
    public void SetTarget(CraftController targetController)
    {
        target = targetController.gameObject;
        targetModel = targetController.Model;

        SetViewMode(viewMode);
    }
    public void SetControlMode(ControlMode mode)
    {
        if (mode == ControlMode.Craft)
        {
            
            SetViewMode(viewMode);
        }

        controlMode = mode;
    }
    public void SetViewMode(CameraViewMode mode)
    {
        viewMode = mode;

        transform.parent = target.transform;
        transform.localPosition = new Vector3(0, 0, -1);
        transform.localRotation = Quaternion.identity;
    }
}
