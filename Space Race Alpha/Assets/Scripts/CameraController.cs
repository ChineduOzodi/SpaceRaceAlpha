using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    public LayerMask surfaceViewMask;
    public LayerMask planetViewMask;
    public LayerMask systemViewMask;

    public CameraRotationMode rotationMode = CameraRotationMode.Absolute;
    public ControlMode controlMode = ControlMode.Free;

    /// <summary>
    /// Camera position in the solar system from reference object
    /// </summary>
    internal Vector3d cameraPosition;

    internal GameObject target; //target object
    internal BaseModel targetModel; //targetModel

    internal SolarBodyModel reference;
    internal double distanceModifier = Units.Gm;
    internal bool closeToReference;

    bool initialized = false;
    

    //Cameras
    Camera mainCam;

    //Camera zoom and move modifiers
    public float camMoveSpeed = 1;
    public int zoomSpeed = 1;
    public float maxCamSize = 10000;
    public float minMapSize = 350; //the minmum size of the map view

    public float GravityMod = 1;

    internal float planetViewSize = 1000;
    internal float surfaceViewSize = 20000;
    internal float systemViewSize = 500;
    //Background
    internal GameObject stars;

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
        Vector3d pos = (cameraPosition - reference.sol.Model.localReferencePoint) / distanceModifier;
        pos.z = 0;
        Polar2 newPol = Polar2.CartesianToPolar(pos);
        transform.position = (Vector3) (Vector3d) (new Polar2(newPol.radius, newPol.angle -reference.Rotation).cartesian); //position in relationship to reference point
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    public void SetCameraView(CameraView view)
    {
        if (view == CameraView.System)
        {
            
            distanceModifier = Units.Gm; //Possible 10 * Units.Mm

            if (reference != null)
            {
                cameraPosition = reference.SystemPosition + cameraPosition;
                transform.position = (Vector3)(cameraPosition / distanceModifier);
                transform.position = new Vector3(transform.position.x, transform.position.y, -1);
            }

            //Set Preset Camera Sizes
            if (cameraView == CameraView.Planet)
                planetViewSize = Camera.main.orthographicSize - 1;
            else if (cameraView == CameraView.Surface)
                surfaceViewSize = Camera.main.orthographicSize;

            cameraView = CameraView.System;

            //Camera Settings
            Camera.main.cullingMask = systemViewMask;
            Camera.main.orthographicSize = systemViewSize;

            reference = GameController.instance.system.centerObject.Model;
        }
        else if (view == CameraView.Planet)
        {
            Vector3d position = reference.SystemPosition + cameraPosition;
            position.z = 0;
            Vector3d distance = position;
            distance.z = 0;

            distanceModifier = Units.Mm;

            //Set Preset Camera Sizes
            if (cameraView == CameraView.System)
                systemViewSize = Camera.main.orthographicSize + .1f;
            else if (cameraView == CameraView.Surface)
                surfaceViewSize = Camera.main.orthographicSize - 5;

            cameraView = view;

            //Camera Settings
            Camera.main.cullingMask = planetViewMask;
            Camera.main.orthographicSize = planetViewSize;

            foreach (SolarBodyModel body in reference.sol.Model.centerObject.Model.solarBodies)
            {
                if ((body.SystemPosition - position).sqrMagnitude < distance.sqrMagnitude)
                {
                    distance = position - body.SystemPosition;
                    reference = body;
                }
            }

            //Camera Position

            cameraPosition = distance;
            transform.position = (Vector3)(cameraPosition / distanceModifier);
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        }
        else if (view == CameraView.Surface)
        {
            Vector3d position = reference.SystemPosition + (Vector3d)transform.position * distanceModifier;
            position.z = 0;
            Vector3d distance = (Vector3d)transform.position * distanceModifier;
            distance.z = 0;
            
            distanceModifier = 1;

            //Set Preset Camera Sizes
            if (cameraView == CameraView.Planet)
                planetViewSize = Camera.main.orthographicSize + .001f;
            else if (cameraView == CameraView.System)
                systemViewSize = Camera.main.orthographicSize;

            cameraView = view;


            //Camera Settings
            Camera.main.cullingMask = surfaceViewMask;
            Camera.main.orthographicSize = surfaceViewSize;

            foreach (SolarBodyModel body in reference.solarBodies)
            {
                if ((body.SystemPosition - position).sqrMagnitude < distance.sqrMagnitude)
                {
                    distance = position - body.SystemPosition;
                    reference = body;
                    cameraPosition = distance;
                }
            }

            foreach (CraftModel craft in reference.crafts)
            {
                if ((craft.SystemPosition - position).sqrMagnitude < distance.sqrMagnitude && craft.State != ObjectState.Landed)
                {
                    distance = position - craft.SystemPosition;
                    targetModel = craft;
                    reference = craft.reference.Model;
                }
            }

            

            
            if(targetModel != null)
            {
                Controller.Instantiate<CraftController>(targetModel);
                transform.position = new Vector3(0, 0, -1);
                Camera.main.orthographicSize = 5;
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
        if (cameraPosition.magnitude < reference.radius + Units.km * 10)
        {
            closeToReference = true;
        }
        else closeToReference = false;

        if (!GameController.instance.setup)
        {
            float moveModifier = camMoveSpeed * mainCam.orthographicSize;
            mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;

            //Setting Camera localPosition TODO: Check to make sure it is accurate
            if (cameraView == CameraView.Surface)
            {
                if (closeToReference)
                {
                    cameraPosition = (Vector3d)(Camera.main.transform.position) * distanceModifier + reference.sol.Model.localReferencePoint;
                    cameraPosition.z = 0;
                }
                else
                {
                    if (targetModel != null)
                    {
                        cameraPosition = (Vector3d)(Camera.main.transform.position) * distanceModifier + targetModel.LocalPosition;
                        cameraPosition.z = 0;
                    }
                    else
                    {
                        cameraPosition = (Vector3d) transform.position.normalized * (reference.radius + Units.km * 5);
                        closeToReference = true;
                    }
                    
                }

            }

            else
            {
                cameraPosition = (Vector3d)(Camera.main.transform.position) * distanceModifier;
                cameraPosition.z = 0;
            }

            //Switch between the different views
            if ( cameraView == CameraView.System)
            {
                if ( mainCam.orthographicSize < 1)
                {
                    SetCameraView(CameraView.Planet);
                }
            }
            else if (cameraView == CameraView.Planet)
            {
                if (mainCam.orthographicSize > 1000)
                {
                    SetCameraView(CameraView.System);
                }
                else if (mainCam.orthographicSize < .01)
                {
                    SetCameraView(CameraView.Surface);
                }
            }
            else if (cameraView == CameraView.Surface)
            {
                if (mainCam.orthographicSize > 20000)
                {
                    SetCameraView(CameraView.Planet);
                }
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

            //Settings for Camera Rotation

            if (rotationMode == CameraRotationMode.Absolute)
            {
                if (cameraView == CameraView.Surface)
                {
                    if (closeToReference)
                    transform.eulerAngles = new Vector3(0, 0, (float)(-reference.Rotation * Mathd.Rad2Deg));
                    else
                        transform.localRotation = Quaternion.identity;
                }
                else
                {
                    transform.localRotation = Quaternion.identity;
                }
            }
            else if (rotationMode == CameraRotationMode.Reference)
            {
                if (cameraView == CameraView.Surface)
                {
                    transform.eulerAngles = new Vector3(0, 0, (float)((new Polar2(cameraPosition).angle - reference.Rotation) * Mathd.Rad2Deg - 90));
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, (float)((new Polar2(cameraPosition).angle) * Mathd.Rad2Deg - 90));
                }
            }
            else if (rotationMode == CameraRotationMode.Relative)
            {
                if (target)
                {
                    transform.rotation = target.transform.rotation;
                }
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
                SetRotationMode((rotationMode.GetHashCode() + 1 > 2) ? 0 : rotationMode + 1);
            }

            //Set Follow
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleFollow();
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

        SetRotationMode(rotationMode);
    }
    public void SetControlMode(ControlMode mode)
    {
        if (mode == ControlMode.Craft)
        {
            
            SetRotationMode(rotationMode);
        }

        controlMode = mode;
    }
    public void SetRotationMode(CameraRotationMode mode)
    {
        rotationMode = mode;
        MessagePanel.SendMessage("Rotation Mode: " + rotationMode.ToString(), 3, Color.blue);
    }
}
