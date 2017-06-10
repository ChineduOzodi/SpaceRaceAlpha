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

    public BaseModel Model { get; internal set; }
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

        //Set physics
        rgb = gameObject.AddComponent<Rigidbody2D>();
        rgb.mass = (float)model.mass;

        //setup initial location and rotation
        CheckAltitude();

        transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
        rgb.angularVelocity = (float)(model.LocalRotationRate * Mathd.Rad2Deg);

        Controller.Instantiate<CraftPartController>(model.rootCraft.Model.spriteName, model.rootCraft.Model, this.transform);

        
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
        transform.position = (Vector3)Forces.Rotate((model.LocalPosition - model.sol.Model.localReferencePoint), -model.reference.Model.Rotation); //position in relationship to reference point
        transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
        rgb.velocity = (Vector3)Forces.Rotate((model.LocalVelocity - model.sol.Model.localReferenceVel), -model.reference.Model.Rotation - model.polar.angle + .5 * Mathd.PI); //sets the reletive velocity
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

        //Check Altitude

        CheckAltitude();

        if (model.playerControlled)
        {
            double translationV = Input.GetAxis("Vertical") * translationSpeed * Time.deltaTime;
            double translationH = Input.GetAxis("Horizontal") * translationSpeed * Time.deltaTime;
            double rotation = 0; //rotation torque to add

            if (Input.GetKey(KeyCode.Q))
            {
                rotation = -1 * rotationSpeed * Time.deltaTime;
                if (model.State == ObjectState.Landed)
                {
                    model.State = ObjectState.SubOrbit;
                    OnStateChanged(null);
                }
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotation = 1 * rotationSpeed * Time.deltaTime;
                if (model.State == ObjectState.Landed)
                {
                    model.State = ObjectState.SubOrbit;
                    OnStateChanged(null);
                }

            }
            else if (SAS) //run SAS only when not manuelly controlling rotation
            {
                SASProgram();
            }
            else if (Prograde)
            {
                ProgradeProgram();
            }
            else if (Retrograde)
            {
                RetrogradeProgram();
            }

            model.throttle += (Input.GetKey(KeyCode.LeftShift)) ? 10 * Time.deltaTime : 0;
            model.throttle -= (Input.GetKey(KeyCode.LeftControl)) ? 10 * Time.deltaTime : 0;

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
            model.CraftControl(model.throttle, new Vector2d(0, translationV), rotation, Time.deltaTime);

        }

        //If not close to reference settings
        if (model.State != ObjectState.Landed && model.closeToReference)
        {

            Vector3 force = (Vector3)Forces.Rotate(model.force - model.sol.Model.localReferenceForce, model.reference.Model.Rotation);
            rgb.AddForce(force * Time.deltaTime * 50);

            Vector3d newlocPosDiff = Forces.Rotate((Vector3d)transform.position, model.reference.Model.Rotation);
            model.LocalPosition = newlocPosDiff + model.sol.Model.localReferencePoint;
            model.LocalVelocity = Forces.Rotate((Vector3d)(Vector2d)rgb.velocity, model.polar.angle - .5 * Mathd.PI + model.reference.Model.Rotation) + model.sol.Model.localReferenceVel; //TODO: Check / update this to be more accurate



        }
        else
        {
            rgb.velocity = Vector2.zero;
        }

        rgb.AddTorque((float) model.torque);
        model.Rotation = transform.rotation.eulerAngles.z * Mathd.Deg2Rad;
        model.LocalRotationRate += rgb.angularVelocity * Mathd.Deg2Rad;
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
        referenceController = Controller.Instantiate<PlanetController>(model.referenceBody.Model.Type.ToString(), model.referenceBody.Model);
    }
    void OnExitBodyProximity()
    {
        Destroy(referenceController.gameObject); //Destroy pbody game object

        model.sol.Model.localReferencePoint = model.LocalPosition; //set new reference point
        model.sol.Model.localReferenceVel = model.LocalVelocity;
        model.sol.Model.localReferenceForce = model.force;


        model.NotifyChange(); //update local controller
    }
    private void SASProgram(double desiredRotationRate = 0)
    {
        double rotation = 0;

        if (model.RotationRate != desiredRotationRate)
        {
            if (model.RotationRate > desiredRotationRate)
            {
                rotation = rotationSpeed * Time.deltaTime;
            }
            else
            {
                rotation = -rotationSpeed * Time.deltaTime;
            }

            rgb.AddTorque((float) rotation);
            model.LocalRotationRate = rgb.angularVelocity * Mathd.Deg2Rad;
            if (Mathd.Abs(rgb.angularVelocity) < .1)
            { //It has reached slow enough speed to stop

                rgb.angularVelocity = 0;
                model.RotationRate = 0;
                model.Rotation = transform.rotation.eulerAngles.z * Mathd.Deg2Rad;
            }
        }       

    }
    /// <summary>
    /// returns the appropriate rotation rate for autopilot to turn to an angle (world)
    /// </summary>
    /// <param name="desiredRotation">dsired world angle in radians</param>
    /// <returns></returns>
    private double DesiredRotationRate(double desiredRotation)
    {
        double multiplier = 1.25;
        double pow = .4;
        double rotationDifference = RotationDifference(desiredRotation, model.Rotation);
        if (rotationDifference > 0)
        {
            double desiredRotationRate = Mathd.Pow(rotationDifference * multiplier, pow);
            return desiredRotationRate;
        }
        else
        {
            double desiredRotationRate = -Mathd.Pow(-rotationDifference * multiplier, pow);
            return desiredRotationRate;
        }
        
        
    }

    /// <summary>
    /// Difference of two angles, in radians
    /// </summary>
    /// <param name="desiredRotation"></param>
    /// <param name="localRotation"></param>
    /// <returns></returns>
    private double RotationDifference(double desiredRotation, double localRotation)
    {
        double rotationDifference = desiredRotation - localRotation;

        if (rotationDifference > Math.PI)
        {
            rotationDifference -= 2 * Math.PI;
        }
        if (rotationDifference < -Math.PI)
        {
            rotationDifference += 2 * Math.PI;
        }
        return rotationDifference;
    }


    /// <summary>
    /// rotates craft to prograde
    /// </summary>
    private void ProgradeProgram()
    {
        double desiredRotationRate = DesiredRotationRate(new Polar2(model.LocalVelocity).angle - .5 * Mathd.PI); //Figure out rotation rate wanted

        SASProgram(desiredRotationRate); //Add torque


    }
    private void RetrogradeProgram()
    {
        double desiredRotationRate = DesiredRotationRate(new Polar2(model.LocalVelocity).angle - 1.5 * Mathd.PI); //Figure out rotation rate wanted

        SASProgram(desiredRotationRate); //Add torque


    }
}
