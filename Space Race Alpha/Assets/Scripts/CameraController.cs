using UnityEngine;
using System.Collections;
using CodeControl;

public class CameraController : MonoBehaviour {

    public GameObject target; //target object
    public BaseModel targetModel; //targetModel

    private CameraViewMode viewMode = CameraViewMode.Absolute;
    public ControlMode controlMode = ControlMode.Craft;

    //Cameras
    Camera mainCam;
    Camera mapCam;

    //Camera zoom and move modifiers
    public float camMoveSpeed = 1;
    public int zoomSpeed = 1;
    public float maxCamSize = 10000;
    public bool setup = false;


    // Use this for initialization
    void Start () {

        mainCam = GetComponent<Camera>();

        mapCam = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        if (targetModel == null)
        {
            targetModel = target.GetComponent<CraftController>().Model;
        }

        //SetViewMode(viewMode);

        float moveModifier = camMoveSpeed * Camera.main.orthographicSize;
        Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed * moveModifier;
        mapCam.orthographicSize = Camera.main.orthographicSize * 100;

        //Camera scale limits
        if (Camera.main.orthographicSize < 4)
            Camera.main.orthographicSize = 4;
        else if (Camera.main.orthographicSize > maxCamSize)
            Camera.main.orthographicSize = maxCamSize;

        //camera movement
        if (!setup)
        {
            //float transX = Input.GetAxis("Horizontal") * moveModifier * Time.deltaTime;
            //float transY = Input.GetAxis("Vertical") * moveModifier * Time.deltaTime;

            //Camera.main.transform.Translate(new Vector3(transX, transY));
        }

        //camera rotation
        if (viewMode == CameraViewMode.Absolute)
        {
            transform.rotation = Quaternion.identity;
        }
        else if (viewMode == CameraViewMode.Reference)
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0, 0, Forces.CartesianToPolar(targetModel.position - targetModel.reference.Model.position).y * Mathf.Rad2Deg + 90);
            transform.rotation = rot;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            SetViewMode((viewMode.GetHashCode() + 1 > 2) ? 0 : viewMode + 1);
        }
    }

    public void SetTarget(CraftController targetController)
    {
        target = targetController.gameObject;
        targetModel = targetController.Model;

        SetViewMode(viewMode);
    }
    public void SetViewMode(CameraViewMode mode)
    {
        viewMode = mode;

        transform.parent = target.transform;
        transform.localPosition = new Vector3(0, 0, -1);
        transform.localRotation = Quaternion.identity;
    }
}
