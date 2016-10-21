using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class CraftController : Controller<CraftModel> {

    public float G = 1;

    internal bool closeToReference = false;
    PlanetController referenceController;

    internal float throttle = 0;
    internal float rotation = 0;
    float translationV = 0;
    float translationH = 0;

    internal bool control = true;

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

        //-------Set Message Spawned ---------//
        model.spawned = true;

        //setup initial location and rotation
        CheckAltitude();

        //Set physics
        rgb = GetComponent<Rigidbody2D>();
        rgb.mass = (float) model.mass;
        rgb.angularVelocity = (float)(model.rotationRate * Mathd.Rad2Deg);
        //set.add to reference object list
        //model.reference.Model.crafts.Add(model);

        //Check if near solar body
        
    }
    protected override void OnDestroy()
    {
        model.spawned = false;
    }

    protected override void OnModelChanged()
    {
        //update position location parameters

        transform.position = (Vector3) Forces.ReferencePosition(model.LocalPosition, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation); //position in relationship to reference point
        transform.eulerAngles = new Vector3( 0, 0, model.rotation.eulerAngles.z - model.sol.Model.localReferencePointRotation.eulerAngles.z); //rotation in relationship to reference point
        //transform.localScale = model.localScale;
        transform.rotation = model.rotation;

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

            //model.LocalVelocity = Forces.AngularVelocity(model.reference.Model) 
            //    * model.LocalPosition.magnitude 
            //    * Forces.Tangent(model.LocalPosition.normalized) + model.LocalVelocity;

            //model.NotifyChange();
        }
    }

    internal void ToggleSAS()
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

            throttle += (Input.GetKey(KeyCode.LeftShift)) ? 10 * Time.deltaTime : 0;
            throttle -= (Input.GetKey(KeyCode.LeftControl)) ? 10 * Time.deltaTime : 0;

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

            

        }

        if (model.state != ObjectState.Landed)
        {
            //Update Physics
            Vector3 force = (Vector3) Forces.Rotate(model.relativeForce, new Polar2(model.sol.Model.localReferencePoint).angle + model.sol.Model.localReferencePointRotation.eulerAngles.z * Mathd.Deg2Rad - .5d * Mathd.PI);
            //rgb.AddForce( force * Time.deltaTime * 50);

            rgb.AddRelativeForce(new Vector2(translationH, translationV + throttle));
            rgb.AddTorque(rotation);

            
        }
        Vector3d surfVel = model.referencePointSurfaceVelocity;
        model.LocalPosition = Forces.ReferencePositionReverse((Vector3d) transform.position, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation);
        model.rotation.eulerAngles = new Vector3(0, 0, model.sol.Model.localReferencePointRotation.eulerAngles.z - model.rotation.eulerAngles.z);
        model.referencePointSurfaceVelocity = (Vector2d)rgb.velocity + new Vector2d(model.polar.radius * model.reference.Model.rotationRate, 0 ); //TODO: Check / update this to be more accurate
        model.rotationRate = rgb.angularVelocity * Mathd.Deg2Rad;

    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        //Check Landed
        if (model.state == ObjectState.Landed)
        {
            transform.parent = coll.transform;
            rgb.velocity = Vector2.zero;
            rgb.angularVelocity = 0;
        }
        //model.state = ObjectState.Landed;


    }

    void CheckAltitude()
    {
        if (closeToReference && model.polar.radius - model.reference.Model.radius > 10000) {
            OnExitBodyProximity();
            closeToReference = false;
        }
        else if (!closeToReference && model.polar.radius - model.reference.Model.radius < 10000)
        {
            OnEnterBodyProximity();
            closeToReference = true;
        }
    }
    void OnEnterBodyProximity()
    {
        referenceController = Controller.Instantiate<PlanetController>(model.reference.Model.type.ToString(), model.reference.Model);
    }
    void OnExitBodyProximity()
    {
        Destroy(referenceController.gameObject);
    }
    private void SASProgram()
    {
        float rotation = 0;

        if (model.rotationRate > 0)
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
