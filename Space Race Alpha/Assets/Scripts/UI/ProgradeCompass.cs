using UnityEngine;
using System.Collections;

public class ProgradeCompass : MonoBehaviour {

    public GameObject target; //target object
    public BaseModel targetModel; //targetModel

    public GameObject reference;

    CameraController camContr;

    // Use this for initialization
    void Start () {

        camContr = Camera.main.GetComponent<CameraController>();

    }
	
	// Update is called once per frame
	void Update () {

        if (targetModel == null)
        {
            targetModel = camContr.targetModel;
        }
        else {

        
            transform.localEulerAngles = new Vector3(0, 0, (float) (targetModel.ProgradeSurfaceAngle * Mathd.Rad2Deg)); //set rotation of compass needle
        }
	
	}
}
