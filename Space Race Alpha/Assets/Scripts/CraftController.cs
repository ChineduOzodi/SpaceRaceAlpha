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
    public float rotationSpeed = 1f;
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
        transform.eulerAngles = new Vector3(0, 0, (float)(model.localRotation * Mathd.Rad2Deg));

        //Set physics
        rgb = GetComponent<Rigidbody2D>();
        rgb.mass = (float) model.mass;
        rgb.angularVelocity = (float)(model.LocalRotationRate * Mathd.Rad2Deg);        

        //set.add to reference object list
        //model.reference.Model.crafts.Add(model);
        
    }
    protected override void OnDestroy()
    {
        model.spawned = false;
    }

    protected override void OnModelChanged() //Should only be called when a new local reference point selected
    {
        if (rgb == null)
        {
            rgb = GetComponent<Rigidbody2D>(); //Added this because it was running OnModelChanged vefore initialize
        }
        //update position location parameters
        transform.position = (Vector3)Forces.Rotate((model.LocalPosition - model.sol.Model.localReferencePoint), model.reference.Model.rotation) ; //position in relationship to reference point
        rgb.velocity = (Vector3) Forces.Rotate((model.LocalVelocity - model.sol.Model.localReferenceVel), model.reference.Model.rotation); //sets the reletive velocity
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

            rgb.velocity = (Vector3)Forces.Rotate((model.LocalVelocity - model.sol.Model.localReferenceVel), model.reference.Model.rotation); //sets the reletive velocity
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
            Vector3 force = (Vector3)Forces.Rotate(model.force - model.sol.Model.localReferenceForce, model.reference.Model.rotation);
            rgb.AddForce(force * Time.deltaTime * 50);

            rgb.AddRelativeForce(new Vector2(translationH, translationV + throttle));
            rgb.AddTorque(rotation );
        }
        model.LocalPosition = Forces.Rotate((Vector3d) transform.position,-model.reference.Model.rotation) + model.sol.Model.localReferencePoint;
        model.LocalVelocity =  Forces.Rotate((Vector2d) rgb.velocity,-model.reference.Model.rotation) + model.sol.Model.localReferenceVel; //TODO: Check / update this to be more accurate
        model.localRotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        model.LocalRotationRate += rgb.angularVelocity * Mathd.Deg2Rad;

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

        if (model.RotationRate != 0)
        {
            if (model.RotationRate > 0)
            {
                rotation = rotationSpeed * Time.deltaTime;
            }
            else
            {
                rotation = -rotationSpeed * Time.deltaTime;
            }

            rgb.AddTorque(rotation);
            model.LocalRotationRate = rgb.angularVelocity * Mathd.Deg2Rad;
            if (Mathf.Abs(rgb.angularVelocity) < .1) { //It has reached slow enough speed to stop

                rgb.angularVelocity = 0;
                model.RotationRate = 0;
                model.rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            }
        }

        

        

    }
}
