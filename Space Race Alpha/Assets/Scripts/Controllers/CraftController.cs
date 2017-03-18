using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class CraftController : Controller<CraftModel> {

    internal bool closeToReference = false;
    PlanetController referenceController;

    internal float throttle = 0;
    internal float rotation = 0;
    float translationV = 0;
    float translationH = 0;

    internal bool control = true;

    internal bool SAS = false;
    internal bool Prograde = false;
    internal bool Retrograde = false;

    public float translationSpeed = 10f;
    public float rotationSpeed = 1f;
    public float throttleSpeed = 1f;

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

        //Set physics
        rgb = GetComponent<Rigidbody2D>();
        rgb.mass = (float)model.mass;

        if (model.isRoot)
        {
            //setup initial location and rotation
            CheckAltitude();

            transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
            rgb.angularVelocity = (float)(model.LocalRotationRate * Mathd.Rad2Deg);
        }
        else
        {
            transform.localPosition = (Vector3) model.LocalPosition;
            transform.localEulerAngles = new Vector3(0, 0, (float)model.LocalRotation);
        }

        foreach (CraftModel craft in model.craftParts)
        {
            Controller.Instantiate<CraftController>(craft.spriteName, craft, this.transform);
        }
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
            rgb = GetComponent<Rigidbody2D>(); //Added this because it was running OnModelChanged before initialize
        }

        if (model.isRoot)
        {
            //update position location parameters
            transform.position = (Vector3)Forces.Rotate((model.LocalPosition - model.sol.Model.localReferencePoint), -model.reference.Model.Rotation); //position in relationship to reference point
            transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
            rgb.velocity = (Vector3)Forces.Rotate((model.LocalVelocity - model.sol.Model.localReferenceVel), -model.reference.Model.Rotation - model.polar.angle + .5 * Mathd.PI); //sets the reletive velocity
        }

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


        if (model.isRoot)
        {
            //Check Altitude

            CheckAltitude();



            rotation = 0; //rotation torque to add

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
                else if (Prograde)
                {
                    ProgradeProgram();
                }
                else if (Retrograde)
                {
                    RetrogradeProgram();
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
                else if (throttle > 100)
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
                model.throttle = throttle;

                //Vector3 relForce = Forces.PolarToCartesian(new Vector2(throttle, transform.rotation.eulerAngles.z * Mathf.Deg2Rad));
                //model.force += relForce;
                //model.velocity = Forces.ForceToVelocity(model);
                //model.position = Forces.VelocityToPosition(model);
                //model.NotifyChange();




                //transform.Translate(new Vector3(translationH, translationV, 0));
                //transform.Rotate(new Vector3(0, 0, rotation));



            }

            //If not close to reference settings
            if (!closeToReference)
            {
                //if (model.alt < 100 * Units.km)
                //{
                //    model.throttle = 100;
                //    throttle = model.throttle;
                //    if (model.alt < 4.5 * Units.km)
                //    {
                //        SASProgram(DesiredRotationRate(0 * Mathd.PI));
                //    }
                //    else if (model.alt < 60 * Units.km)
                //    {
                //        SASProgram(DesiredRotationRate(.15 * Mathd.PI));
                //    }
                //    else
                //    {
                //        SASProgram(DesiredRotationRate(.20 * Mathd.PI));
                //    }
                //}
                //else if (model.Ecc.sqrMagnitude > .0001)
                //{

                //    if ( model.SurfaceVel.y > 100)
                //    {
                //        model.throttle--;
                //        throttle = model.throttle;
                //        ProgradeProgram();
                //    }
                //    else if (model.SurfaceVel.y < 100)
                //    {
                //        model.throttle++;
                //        throttle = model.throttle;

                //        if (model.SurfaceVel.y < 50)
                //        {
                //            SASProgram(DesiredRotationRate(.20 * Mathd.PI));
                //        }
                //        else
                //        {
                //            SASProgram(DesiredRotationRate(.30 * Mathd.PI));
                //        }
                //    }
                //}
                //else
                //{
                //    model.throttle = 0;
                //    throttle = model.throttle;
                //}

                //Manually update model forces

                Vector3d addedForce = Forces.Rotate(new Vector3d(translationH, translationV + throttle), (model.Rotation)); //forces located to local orientation

                model.LocalVelocity += Forces.ForceToVelocity(model, addedForce);
                model.position = Forces.VelocityToPosition(model);
                Vector3d surfVel = model.SurfaceVel;

                //Update reference point
                model.sol.Model.localReferencePoint = model.LocalPosition;
                model.sol.Model.localReferenceVel = model.LocalVelocity;
                model.sol.Model.localReferenceForce = model.force;


            }
            else
            {
                if (model.state != ObjectState.Landed)
                {

                    Vector3 force = (Vector3)Forces.Rotate(model.force - model.sol.Model.localReferenceForce, model.reference.Model.Rotation);
                    rgb.AddForce(force * Time.deltaTime * 50);

                    rgb.AddRelativeForce(new Vector2(translationH, translationV + model.throttle));



                }
                else
                {
                    rgb.velocity = Vector2.zero;
                }
                Vector3d newlocPosDiff = Forces.Rotate((Vector3d)transform.position, model.reference.Model.Rotation);
                model.LocalPosition = newlocPosDiff + model.sol.Model.localReferencePoint;
                model.LocalVelocity = Forces.Rotate((Vector3d)(Vector2d)rgb.velocity, model.polar.angle - .5 * Mathd.PI + model.reference.Model.Rotation) + model.sol.Model.localReferenceVel; //TODO: Check / update this to be more accurate

            }

            rgb.AddTorque(rotation);
            model.Rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            model.LocalRotationRate += rgb.angularVelocity * Mathd.Deg2Rad;
        }
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
        if (closeToReference && model.alt> 20000) {
            closeToReference = false;
            OnExitBodyProximity();
            
        }
        else if (!closeToReference && model.alt < 20000)
        {
            closeToReference = true;
            OnEnterBodyProximity();
            
        }
    }
    void OnEnterBodyProximity()
    {
        referenceController = Controller.Instantiate<PlanetController>(model.referenceBody.Model.type.ToString(), model.referenceBody.Model);
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
        float rotation = 0;

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

            rgb.AddTorque(rotation);
            model.LocalRotationRate = rgb.angularVelocity * Mathd.Deg2Rad;
            if (Mathf.Abs(rgb.angularVelocity) < .1)
            { //It has reached slow enough speed to stop

                rgb.angularVelocity = 0;
                model.RotationRate = 0;
                model.Rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
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
