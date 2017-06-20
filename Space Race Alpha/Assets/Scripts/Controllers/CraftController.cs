using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class CraftController : Controller<CraftModel> {

    PlanetController referenceController;

    internal bool SAS = false;
    internal bool Prograde = false;
    internal bool Retrograde = false;

    public double translationSpeed = 10f;
    public double rotationSpeed = 1f;
    public double throttleSpeed = 1f;

    internal Rigidbody2D rgb;

    internal BaseModel Model { get; private set; }

    private CameraController cam;

    public float throttle {
        get
        {
            return (float) model.throttle;
        }
    }

    //public Vector3 force;

    // Use this for initialization
    void Start () {      
	
	}

    protected override void OnInitialize()
    {

        //Add Listeners

        //Message.AddListener("ToggleSASMessage", ToggleSAS);
        Model = model;
        //-------Set Message Spawned ---------//
        model.spawned = true;

        cam = Camera.main.GetComponent<CameraController>();

        //Set physics
        rgb = gameObject.AddComponent<Rigidbody2D>();
        rgb.mass = (float)model.mass;

        //setup initial location and rotation

        transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
        rgb.angularVelocity = (float)(model.RotationRate * Mathd.Rad2Deg);

        Controller.Instantiate<CraftPartController>(model.rootCraft.Model.spriteName, model.rootCraft.Model, this.transform);

        //Set initial position if not targetModel
        if (cam.closeToReference)
        {
            transform.position = (Vector3)Forces.Rotate(model.LocalPosition - model.sol.Model.localReferencePoint, -model.reference.Model.Rotation);
            transform.eulerAngles = new Vector3(0, 0, (float)model.LocalRotation * Mathf.Rad2Deg);
            rgb.velocity = (Vector3)Forces.Rotate(model.LocalVelocity - model.sol.Model.localReferenceVel, -model.reference.Model.Rotation);
        }
        else if (cam.targetModel != null && model.name != cam.targetModel.name)
        {
            transform.position = (Vector3)(model.SystemPosition - cam.targetModel.SystemPosition);
            transform.eulerAngles = new Vector3(0, 0, (float)model.Rotation * Mathf.Rad2Deg);
            rgb.velocity = (Vector3)(model.velocity - cam.targetModel.velocity);
        }
        //set.add to reference object list
        //model.reference.Model.crafts.Add(model);

    }

    protected override void OnDestroy()
    {
        model.spawned = false;
        base.OnDestroy();
    }

    protected override void OnModelChanged() //Should only be called when a new local reference point selected
    {
        if (rgb == null)
        {
            rgb = GetComponent<Rigidbody2D>(); //Added this because it was running OnModelChanged before initialize
        }

        //update position location parameters
        //    transform.position = (Vector3)Forces.Rotate((model.LocalPosition - model.sol.Model.localReferencePoint), -model.reference.Model.Rotation); //position in relationship to reference point
        //    transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
        //    rgb.velocity = (Vector3)Forces.Rotate((model.LocalVelocity - model.sol.Model.localReferenceVel), -model.reference.Model.Rotation - model.polar.angle + .5 * Mathd.PI); //sets the reletive velocity
    }
   
    /// <summary>
    /// When the landed/orbit state is changed
    /// </summary>
    /// <param name="reference"></param>
    internal void OnStateChanged(Transform reference)
    {
        if (model.State == ObjectState.Landed)
        {
            transform.parent = reference;
        }
        else
        {
            transform.parent = reference;

            rgb.velocity = (Vector3)Forces.Rotate((model.LocalVelocity - model.sol.Model.localReferenceVel), model.reference.Model.Rotation + model.polar.angle - .5 * Mathd.PI); //sets the reletive velocity
            //model.LocalVelocity = Forces.AngularVelocity(model.reference.Model) 
            //    * model.LocalPosition.magnitude 
            //    * Forces.Tangent(model.LocalPosition.normalized) + model.LocalVelocity;

            //model.NotifyChange();
        }
    }

    internal void ToggleSAS()
    {
        if (SAS)
        {
            SAS = false;
            Prograde = false;
            Retrograde = false;
        }


        else
        {
            SAS = true;
            Prograde = false;
            Retrograde = false;
        }

    }
    internal void TogglePrograde()
    {
        if (Prograde)
        {
            SAS = false;
            Prograde = false;
        }


        else
        {
            SAS = false;
            Prograde = true;
        }
    }

    internal void ToggleRetrograde()
    {
        if (Retrograde)
        {
            SAS = false;
            Prograde = false;
            Retrograde = false;
        }


        else
        {
            SAS = false;
            Prograde = false;
            Retrograde = true;
        }
    }

    // Update is called once per frame
    public void Update () {

        if (model.playerControlled)
        {
            double translationV = Input.GetAxis("Vertical") * translationSpeed * model.sol.Model.date.deltaTime;
            double translationH = Input.GetAxis("Horizontal") * translationSpeed * model.sol.Model.date.deltaTime;
            double rotation = 0; //rotation torque to add

            //if (Input.GetKey(KeyCode.Q))
            //{
            //    rotation = -1 * rotationSpeed * model.sol.Model.date.deltaTime;
            //    if (model.State == ObjectState.Landed)
            //    {
            //        model.State = ObjectState.SubOrbit;
            //        OnStateChanged(null);
            //    }
            //}
            //else if (Input.GetKey(KeyCode.E))
            //{
            //    rotation = 1 * rotationSpeed * model.sol.Model.date.deltaTime;
            //    if (model.State == ObjectState.Landed)
            //    {
            //        model.State = ObjectState.SubOrbit;
            //        OnStateChanged(null);
            //    }

            //}
            //else if (SAS) //run SAS only when not manuelly controlling rotation
            //{
            //    SASProgram();
            //}
            //else if (Prograde)
            //{
            //    ProgradeProgram();
            //}
            //else if (Retrograde)
            //{
            //    RetrogradeProgram();
            //}

            model.throttle += (Input.GetKey(KeyCode.LeftShift)) ? 10 * model.sol.Model.date.deltaTime : 0;
            model.throttle -= (Input.GetKey(KeyCode.LeftControl)) ? 10 * model.sol.Model.date.deltaTime : 0;

            if (Input.GetKeyDown(KeyCode.X))
            {
                model.throttle = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                model.throttle = 100;
            }
            else if (model.throttle > 100)
            {
                model.throttle = 100;
            }
            else if (model.throttle < 0)
            {
                model.throttle = 0;
            }

            if (model.throttle > 0)
            {
                if (model.State == ObjectState.Landed)
                {
                    model.State = ObjectState.SubOrbit;
                    OnStateChanged(null);
                }
            }
            model.CraftControl(model.throttle, new Vector2d(0, translationV), rotation, model.sol.Model.date.deltaTime);

        }

        //Settings to control the system position of a craft once it is spawned
        if (model.State != ObjectState.Landed)
        {
            if (model.closeToReference)
            {
                Vector3 force = (Vector3)Forces.Rotate(model.force - model.sol.Model.localReferenceForce, model.reference.Model.Rotation);
                rgb.AddForce(force * model.sol.Model.date.deltaTime * 50);

                Vector3d newlocPosDiff = Forces.Rotate((Vector3d)transform.position, model.reference.Model.Rotation);
                model.LocalPosition = newlocPosDiff + model.sol.Model.localReferencePoint;
                model.LocalVelocity = Forces.Rotate((Vector3d)(Vector2d)rgb.velocity, model.polar.angle - .5 * Mathd.PI + model.reference.Model.Rotation)
                    + model.sol.Model.localReferenceVel; //TODO: Check / update this to be more accurate
            }
            else
            {
                if (model.name == cam.targetModel.name)
                {
                    model.velocity += Forces.ForceToVelocity(model.force, model.mass, model.sol.Model.date.deltaTime);
                    model.SystemPosition += model.velocity * model.sol.Model.date.deltaTime;
                }
                else
                {
                    model.velocity = (Vector3d)(Vector2d)rgb.velocity * cam.distanceModifier + cam.targetModel.velocity;
                    model.SystemPosition = cam.targetModel.SystemPosition + (Vector3d)transform.position * cam.distanceModifier;
                }
            }

        }
        else
        {
            rgb.velocity = Vector2.zero;
        }

        rgb.AddTorque((float) model.torque);
        model.Rotation = transform.rotation.eulerAngles.z * Mathd.Deg2Rad;
        model.RotationRate += rgb.angularVelocity * Mathd.Deg2Rad;

        //Check if craft controller should be deleted
        if (transform.position.magnitude > Units.km * 10 || cam.cameraView != CameraView.Surface)
        {
            model.spawned = false;
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        //Check Landed
        if (model.State == ObjectState.Landed)
        {
            transform.parent = coll.transform;
            rgb.velocity = Vector2.zero;
            rgb.angularVelocity = 0;
        }
        //model.state = ObjectState.Landed;


    }

    /// <summary>
    /// Checks the altitude of Craft and spawns the appropriate bodies;
    /// </summary>
    void CheckAltitude()
    {
        if (model.closeToReference && model.alt > 10000) {
            model.closeToReference = false;
            OnExitBodyProximity();
            
        }
        else if (!model.closeToReference && model.alt < 10000)
        {
            model.closeToReference = true;
            OnEnterBodyProximity();
            
        }
    }
    void OnEnterBodyProximity()
    {
        referenceController = Controller.Instantiate<PlanetController>(model.reference.Model.Type.ToString(), model.reference.Model);
    }
    void OnExitBodyProximity()
    {
        Destroy(referenceController.gameObject); //Destroy pbody game object

        model.sol.Model.localReferencePoint = model.LocalPosition; //set new reference point
        model.sol.Model.localReferenceVel = model.LocalVelocity;
        model.sol.Model.localReferenceForce = model.force;


        model.NotifyChange(); //update local controller
    }
    
    
}
