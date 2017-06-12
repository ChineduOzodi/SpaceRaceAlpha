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

    internal SolarBodyModel reference;
    internal double distanceModifier = Units.Gm;

    bool initialized = false;
    

    //Cameras
    Camera mainCam;

    //Camera zoom and move modifiers
    public float camMoveSpeed = 1;
    public int zoomSpeed = 1;
    public float maxCamSize = 10000;
    public float minMapSize = 350; //the minmum size of the map view

    internal float planetViewSize = 5000;
    internal float surfaceViewSize = 10000;
    internal float systemViewSize = 500;
    //Background
    public GameObject stars;

    internal CameraView cameraView;

    // Use this for initialization
    void Start () {

        mainCam = GetComponent<Camera>();
        //initialize star background
        stars = Instantiate(Resources.Load("stars")) as GameObject;

        //Add listeners
        Message.AddListener("SurfaceReferencesUpdated", OnSurfaceReferenceUpdated);
    }

    private void OnSurfaceReferenceUpdated()
    {
        transform.position = (Vector3)Forces.Rotate((cameraPosition - reference.sol.Model.localReferencePoint) / distanceModifier, -reference.Rotation); //position in relationship to reference point
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    public void SetCameraView(CameraView view)
    {
        if (view == CameraView.System)
        {
            cameraView = CameraView.System;
            distanceModifier = Units.Gm; //Possible 10 * Units.Mm

            if (reference != null)
            {
                cameraPosition = reference.SystemPosition + cameraPosition;
                transform.position = (Vector3)(cameraPosition / distanceModifier);
                transform.position = new Vector3(transform.position.x, transform.position.y, -1);
            }

            //Camera Settings
            Camera.main.cullingMask = systemViewMask;
            Camera.main.orthographicSize = systemViewSize;

            reference = GameController.instance.system.centerObject.Model;
        }
        else if (view == CameraView.Planet)
        {
            Vector3d position = reference.SystemPosition + (Vector3d)transform.position * distanceModifier;
            Vector3d distance = position;
            cameraView = view;
            distanceModifier = Units.Mm;

            systemViewSize = Camera.main.orthographicSize + 5;

            //Camera Settings
            Camera.main.cullingMask = planetViewMask;
            Camera.main.orthographicSize = planetViewSize;

            foreach (SolarBodyModel body in reference.sol.Model.centerObject.Model.solarBodies)
            {
                if ((body.SystemPosition - position).sqrMagnitude < distance.sqrMagnitude)
                {
                    distance = body.SystemPosition - position;
                    reference = body;
                }
            }
        }
        else if (view == CameraView.Surface)
        {
            Vector3d position = reference.SystemPosition + (Vector3d)transform.position * distanceModifier;
            Vector3d distance = (Vector3d)transform.position * distanceModifier;
            cameraView = view;
            distanceModifier = 1;

            planetViewSize = Camera.main.orthographicSize + 5;

            //Camera Settings
            Camera.main.cullingMask = surfaceViewMask;
            Camera.main.orthographicSize = surfaceViewSize;

            foreach (SolarBodyModel body in reference.solarBodies)
            {
                if ((body.SystemPosition - position).sqrMagnitude < distance.sqrMagnitude)
                {
                    distance = body.SystemPosition - position;
                    reference = body;
                }
            }

            //Instantiate planet controller for the reference, which will take care of terrain generation
            Controller.Instantiate<PlanetController>(reference.Type.ToString(), reference);

        }
        else
        {
            print(view + " does not exist");
        }

        //Send Message
        SetCameraView m = new SetCameraView();
        m.cameraView = cameraView;
        m.distanceModifier = distanceModifier;
        m.reference = reference;
        Message.Send(m);
    }

    // Update is called once per frame
    void Update()
    {

        if (!GameController.instance.setup)
        {
            float moveModifier = camMoveSpeed * mainCam.orthographicSize;
            mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;

            //Switch between the different views
            if ( cameraView == CameraView.System)
            {
                if ( mainCam.orthographicSize < 5)
                {
                    SetCameraView(CameraView.Planet);
                }
            }
            else if (cameraView == CameraView.Planet)
            {
                if (mainCam.orthographicSize > 10000)
                {
                    SetCameraView(CameraView.System);
                }
                else if (mainCam.orthographicSize < .001)
                {
                    SetCameraView(CameraView.Surface);
                }
            }
            else if (cameraView == CameraView.Surface)
            {

            }

            if (controlMode == ControlMode.Free)
            {
                //camera movement
                float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime;
                float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime;

                Camera.main.transform.Translate(new Vector3(transX, transY));
            }
            else if(controlMode == ControlMode.Follow && target != null)
            {
                Camera.main.transform.position = new Vector3(target.transform.position.x,
                    target.transform.position.y, -1);
                
            }

            //Setting Camera localPosition
            if (cameraView != CameraView.Surface)
                cameraPosition = (Vector3d)(Camera.main.transform.position) * distanceModifier;
            else cameraPosition = (Vector3d)(Camera.main.transform.position) * distanceModifier + reference.sol.Model.localReferencePoint;

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

            //Set Follow
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleFollow();
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

    private void ToggleFollow()
    {
        if (controlMode == ControlMode.Follow)
        {
            controlMode = ControlMode.Free;
        }
        else { controlMode = ControlMode.Follow; }
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

    public void SetTarget(GameObject obj)
    {
        target = obj;
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
