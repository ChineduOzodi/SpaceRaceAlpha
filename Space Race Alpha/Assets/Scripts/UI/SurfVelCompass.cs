using UnityEngine;
using System.Collections;

public class SurfVelCompass : MonoBehaviour {

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
            double pSV = (new Polar2(targetModel.SurfaceVel).angle);
            if (pSV > 2 * Mathd.PI)
            {
                pSV -= 2 * Mathd.PI;
            }
            else if (pSV < 0)
            {
                pSV += 2 * Mathd.PI;
            }

            transform.localEulerAngles = new Vector3(0, 0, (float) (pSV * Mathd.Rad2Deg)); //set rotation of compass needle
        }
	
	}
}
