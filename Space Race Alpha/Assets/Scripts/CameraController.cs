using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject target; //target object
    public BaseModel targetModel; //targetModel

    internal CameraViewMode viewMode = CameraViewMode.Absolute;
    public ControlMode controlMode = ControlMode.Craft;


	// Use this for initialization
	void Start () {

        //SetViewMode(viewMode);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetTarget(GameObject _target, BaseModel _targetModel)
    {
        target = _target;
        targetModel = _targetModel;

        SetViewMode(viewMode);
    }
    public void SetViewMode(CameraViewMode mode)
    {
        viewMode = mode;

        if (mode == CameraViewMode.Absolute)
        {
            transform.parent = target.transform;
            transform.localPosition = new Vector3(0, 0, -1);
            transform.localRotation = Quaternion.identity;
        }
    }
}
