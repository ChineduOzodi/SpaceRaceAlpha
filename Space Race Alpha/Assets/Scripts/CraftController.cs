using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class CraftController : Controller<CraftModel> {

    internal float throttle = 0;
    internal bool control = true;

    //Information needed for control
    FlightInfo fInfo;

    internal bool SAS = false;

    public float translationSpeed = 10f;
    public float rotationSpeed = 10f;
    public float throttleSpeed = 10f;

    internal Rigidbody2D rgb;
    internal CraftModel Model;
    public ParticleSystem prtF;
    public ParticleSystem prtS;

    public Vector3 force;

    // Use this for initialization
    void Start () {

        rgb = GetComponent<Rigidbody2D>();
        Model = model;
	
	}

    protected override void OnInitialize()
    {

        //Add Listeners

        //Message.AddListener("ToggleSASMessage", ToggleSAS);

        //setup initial location and rotation
        transform.position = model.position;
        transform.rotation = model.rotation;

        //set.add to reference object list
        model.reference.Model.crafts.Add(model);

        //create trajectory ring
        SpaceTrajectory orb = gameObject.AddComponent<SpaceTrajectory>();

        orb.model = model;
        orb.width = 1;

        //Initial flight info due to control being true
        fInfo = gameObject.AddComponent<FlightInfo>();
        fInfo.model = model;

        model.flightInfo = fInfo;
    }

    private void ToggleSAS()
    {
        if (SAS)
            SAS = false;
        else
            SAS = true;
            
    }

    // Update is called once per frame
    void Update () {

        //Apply forces
        rgb.AddForce(model.force * Time.deltaTime);
        force = model.force;

        //update basic info
        model.position = transform.position;
        model.rotation = transform.rotation;
        model.mass = rgb.mass;
        model.velocity = rgb.velocity;

        //update Orbital info
        model.orbitalInfo = new OrbitalInfo(model);

        if (control)
        {
            float translationV = Input.GetAxis("Vertical") * translationSpeed * Time.deltaTime;
            float translationH = Input.GetAxis("Horizontal") * translationSpeed * Time.deltaTime;
            float rotation = 0;

            if (Input.GetKey(KeyCode.Q))
            {
                rotation = -1 * rotationSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotation = 1 * rotationSpeed * Time.deltaTime;
            }
            else if (SAS) //run SAS only when not manuelly controlling rotation
            {
                SASProgram();
            }

            throttle += (Input.GetKey(KeyCode.LeftShift)) ? 1 * Time.deltaTime : 0;
            throttle -= (Input.GetKey(KeyCode.LeftControl)) ? 1 * Time.deltaTime : 0;

            if (Input.GetKeyDown(KeyCode.X))
            {
                throttle = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                throttle = 100;
            }
            else if ( throttle > 100)
            {
                throttle = 100;
            }
            else if (throttle < 0)
            {
                throttle = 0;
            }
            prtF.startSpeed = throttle * .1f;
            prtS.startSpeed = throttle * .1f;
            throttle = throttle * throttleSpeed;
            


            rgb.AddRelativeForce(new Vector2(translationH, translationV + throttle));
            rgb.AddTorque(rotation);


            //transform.Translate(new Vector3(translationH, translationV, 0));
            //transform.Rotate(new Vector3(0, 0, rotation));

            //Autopilot buttons
            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleSAS();
            }

        }
        //Figure out LOD for planets
        model.referenceDistance = model.position - model.reference.Model.position;
	
	}

    private void SASProgram()
    {
        float rotation = 0;

        if (fInfo.RotationSpeed > 0)
        {
            rotation = 1 * rotationSpeed * Time.deltaTime;
        }
        else
        {
            rotation = -1 * rotationSpeed * Time.deltaTime;
        }

        rgb.AddTorque(rotation);

    }
}
