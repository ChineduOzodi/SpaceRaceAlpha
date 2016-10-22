using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class CameraController : MonoBehaviour {

    internal GameObject target; //target object
    internal BaseModel targetModel; //targetModel

    internal GameObject reference;

    public CameraViewMode viewMode = CameraViewMode.Absolute;
    public ControlMode controlMode = ControlMode.Craft;

    //Cameras
    Camera mainCam;
    Camera mapCam;

    //Modes
    bool mapMode = false;

    //Camera zoom and move modifiers
    public float camMoveSpeed = 1;
    public int zoomSpeed = 1;
    public float maxCamSize = 10000;
    public float minMapSize = 350; //the minmum size of the map view
    public bool setup = false;

    internal float mapViewSize;
    internal float mainViewSize;
    //Background
    public GameObject stars;
    public GameObject starsMap;

    // Use this for initialization
    void Awake () {

        mainCam = GetComponent<Camera>();

        mapCam = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();

        //initialize star background
        stars = Instantiate(Resources.Load("stars")) as GameObject;
        starsMap = Instantiate(Resources.Load("stars")) as GameObject;
        //ToggleMapMode();
        //ToggleMapMode();

    }

    // Update is called once per frame
    void Update()
    {
        //SetViewMode(viewMode);

        float moveModifier = camMoveSpeed * mainCam.orthographicSize;
        float mapMoveMod = camMoveSpeed * mapCam.orthographicSize;
        mainCam.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;

        if (controlMode == ControlMode.Free)
        {
            //camera movement
            if (!setup)
            {
                float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime;
                float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime;

                Camera.main.transform.Translate(new Vector3(transX, transY));
            }
        }
        else
        {
            if (targetModel == null)
            {
                targetModel = target.GetComponent<CraftController>().Model;
            }
        }

        //MapCamera Zoom
        if (Input.GetKey(KeyCode.Equals))
        {
            mapCam.orthographicSize += zoomSpeed * mapMoveMod * .1f;
        }
        else if (Input.GetKey(KeyCode.Minus))
        {
            mapCam.orthographicSize -= zoomSpeed * mapMoveMod * .1f;
        }

        //Toggle Map Mode
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMapMode();
        }


        //if (mapMode)
        //{
        //    //Camera scale limits
        //    if (mainCam.orthographicSize < minMapSize)
        //        mainCam.orthographicSize = minMapSize;

        //    if (mapCam.orthographicSize < 4)
        //        mapCam.orthographicSize = 4;
        //    else if (mapCam.orthographicSize > minMapSize)
        //        mapCam.orthographicSize = minMapSize;
        //}
        //else
        //{
        //    if (mainCam.orthographicSize < 4)
        //        mainCam.orthographicSize = 4;
        //    else if (mainCam.orthographicSize > minMapSize)
        //        mainCam.orthographicSize = minMapSize;

        //    if (mapCam.orthographicSize < minMapSize)
        //        mapCam.orthographicSize = minMapSize;
        //}



        if (Input.GetKeyDown(KeyCode.V))
        {
            SetViewMode((viewMode.GetHashCode() + 1 > 2) ? 0 : viewMode + 1);
        }

        //Update Background
        stars.transform.position = new Vector3(transform.position.x, transform.position.y);
        stars.transform.localScale = new Vector3(mainCam.orthographicSize * .25f, mainCam.orthographicSize * .25f);

        starsMap.transform.position = new Vector3(transform.position.x, transform.position.y);
        starsMap.transform.localScale = new Vector3(mapCam.orthographicSize * .25f, mapCam.orthographicSize * .25f);


        //camera rotation
        if (viewMode == CameraViewMode.Absolute)
        {
            transform.rotation = Quaternion.identity;
        }
        else if (viewMode == CameraViewMode.Reference)
        {
            //rot.eulerAngles = new Vector3(0, 0, new Polar2(targetModel.LocalPosition).angle * Mathf.Rad2Deg);
            transform.eulerAngles = new Vector3(0, 0, (float)((new Polar2(targetModel.LocalPosition).angle + targetModel.reference.Model.Rotation) * Mathd.Rad2Deg - 90)); //set rotation of compass needle
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

    private void ToggleMapMode()
    {

        //change masks
        var mainCM = mainCam.cullingMask;
        mainCam.cullingMask = mapCam.cullingMask;
        mapCam.cullingMask = mainCM;

        if (mapMode)
        {
            mapMode = false;

            //Set view sizes
            mapViewSize = mainCam.orthographicSize;
            mainViewSize = mapCam.orthographicSize;

            mainCam.orthographicSize = mainViewSize;
            mapCam.orthographicSize = mapViewSize;

            transform.parent = target.transform;
            mapCam.transform.parent = GameObject.FindGameObjectWithTag("satellite").transform;

            transform.localPosition = new Vector3(0, 0, -1);
            mapCam.transform.localPosition = new Vector3(0, 0, -1);
        }
        else
        {
            mapMode = true;

            transform.parent = GameObject.FindGameObjectWithTag("satellite").transform;
            mapCam.transform.parent = target.transform;

            //set view sizes
            mapViewSize = mapCam.orthographicSize;
            mainViewSize = mainCam.orthographicSize;

            mainCam.orthographicSize = mapViewSize;
            mapCam.orthographicSize = mainViewSize;

            transform.localPosition = new Vector3(0, 0, -1);
            mapCam.transform.localPosition = new Vector3(0, 0, -1);
        }
        //Send Message
        ToggleMapMessage m = new ToggleMapMessage();
        m.mapMode = mapMode;
        Message.Send(m);

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
