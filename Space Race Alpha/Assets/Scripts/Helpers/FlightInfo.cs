using UnityEngine;
using System.Collections;

public class FlightInfo : MonoBehaviour {

    private float rotationSpeed=0;
    private float altChangeSpeed=0;

    private float angle;
    private float alt;

    internal float updateInterval = .1f;
    internal float nextUpdate = 0;

    public float RotationSpeed
    {
        get { return rotationSpeed; }
    }

    public float AltChangeSpeed { get { return altChangeSpeed; } }

    internal CraftModel model;


	// Use this for initialization
	void Start () {

        alt = (model.position - model.reference.Model.position).magnitude;
        angle = model.rotation.eulerAngles.z;

    }
	
	// Update is called once per frame
	void Update () {
        if (model != null)
        {
            if (Time.time >= nextUpdate)
            {
                float curAlt = (model.position - model.reference.Model.position).magnitude;
                rotationSpeed = (model.rotation.eulerAngles.z - angle) / updateInterval;
                altChangeSpeed = (curAlt - alt) / updateInterval;

                alt = (model.position - model.reference.Model.position).magnitude;
                angle = model.rotation.eulerAngles.z;

                nextUpdate += updateInterval;
            }
            
        }
	
	}
}
