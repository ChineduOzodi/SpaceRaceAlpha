using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class CraftController : Controller<CraftModel> {

    public float G = 1;

    internal float throttle = 0;
    internal float rotation = 0;
    float translationV = 0;
    float translationH = 0;

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

    //public Vector3 force;

    // Use this for initialization
    void Start () {

        
        Model = model;
	
	}

    protected override void OnInitialize()
    {

        //Add Listeners

        //Message.AddListener("ToggleSASMessage", ToggleSAS);

        //setup initial location and rotation
        transform.position = model.position;
        transform.rotation = model.rotation;

        //Set physics
        rgb = GetComponent<Rigidbody2D>();
        rgb.velocity = model.velocity;
        rgb.mass = model.mass;

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

    protected override void OnModelChanged()
    {
        //update orgital parameters
        //transform.Translate(model.velocity * Time.deltaTime);
        //rect.position = model.position;
        //transform.rotation = model.rotation;
        //transform.localScale = model.localScale;


        //rb2D.mass = model.mass;
        //rb2D.velocity = model.velocity;
    }

    internal void OnStateChanged(Transform reference)
    {
        if (model.state == ObjectState.Landed)
        {
            transform.parent = reference;
        }
        else
        {
            transform.parent = reference;

            //rgb.velocity = (model.reference.Model.velocity * .1f) + model.velocity;

            model.velocity = Forces.AngularVelocity(model.reference.Model) 
                * (model.position - model.reference.Model.reference.Model.position).magnitude 
                * Forces.Tangent((model.reference.Model.position - model.reference.Model.reference.Model.position).normalized) + model.velocity;

            model.NotifyChange();
        }
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
        
        //update Orbital info
        model.orbitalInfo = new OrbitalInfo(model, Forces.G);

        //force = model.force;

        //update basic info
        //model.position = transform.position;
        model.rotation = transform.rotation;
        rotation = 0;
        //model.mass = rgb.mass;
        //model.velocity = rgb.velocity;

        if (control)
        {
            translationV = Input.GetAxis("Vertical") * translationSpeed * Time.deltaTime;
            translationH = Input.GetAxis("Horizontal") * translationSpeed * Time.deltaTime;
            

            if (Input.GetKey(KeyCode.Q))
            {
                rotation = -1 * rotationSpeed * Time.deltaTime;
                if (model.state == ObjectState.Landed)
                {
                    model.state = ObjectState.SubOrbit;
                    OnStateChanged(null);
                }
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotation = 1 * rotationSpeed * Time.deltaTime;
                if (model.state == ObjectState.Landed)
                {
                    model.state = ObjectState.SubOrbit;
                    OnStateChanged(null);
                }
                
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

            if (throttle > 0)
            {
                if (model.state == ObjectState.Landed)
                {
                    model.state = ObjectState.SubOrbit;
                    OnStateChanged(null);
                }
            }
            prtF.startSpeed = throttle * .1f;
            prtS.startSpeed = throttle * .1f;
            model.throttle = throttle * throttleSpeed;

            //Vector3 relForce = Forces.PolarToCartesian(new Vector2(throttle, transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
            //model.force += relForce;
            //model.velocity = Forces.ForceToVelocity(model);
            //model.position = Forces.VelocityToPosition(model);
            //model.NotifyChange();

            


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

    void FixedUpdate()
    {
        if (model.state != ObjectState.Landed)
        {
            //Update Physics
            rgb.AddForce(model.force);

            rgb.AddRelativeForce(new Vector2(translationH, translationV + throttle));
            rgb.AddTorque(rotation);

            model.position = transform.position;
            model.rotation = transform.rotation;
            model.velocity = rgb.velocity;
        }
        

    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        model.state = ObjectState.Landed;
        transform.parent = coll.transform;
        rgb.velocity = Vector2.zero;
        rgb.angularVelocity = 0;
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
