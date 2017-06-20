using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;

public class CraftModel : BaseModel
{

    //----------Public Variables---------------//
    public ModelRef<CraftPartModel> rootCraft;
    public Vector3d centerOfMassPosition;
    public bool playerControlled = false;
    public bool spawned = false;
    public bool closeToReference = false;
    /// <summary>
    /// throttle power from 0 to 100
    /// </summary>
    public double throttle = 0;
    public double torque;

    //----------Private Variables-------------//


    //---------Public Fields------------------//

    //--------------------Constructors-----------//
    public CraftModel() { }

    public CraftModel(string _name, CraftPartModel craftPart)
    {
        Type = ObjectType.Spacecraft;

        rootCraft = new ModelRef<CraftPartModel>(craftPart);
        name = _name;

        CalculateMassPosition();
    }

    private void CalculateMassPosition()
    {
        mass = rootCraft.Model.totalMass();
        centerOfMassPosition = rootCraft.Model.CalculateMassPosition();
    }
    //------------Functions------------------//

    /// <summary>
    /// Thrust/(m * force)
    /// </summary>
    public double TWR()
    {
        double totalThrust = rootCraft.Model.TotalThrust();
        return totalThrust / (mass * force.magnitude);
    }   

    public Vector3d LowestPosition()
    {
        return rootCraft.Model.LowestPosition() - centerOfMassPosition; 
    }
    //---------------------Defualt Crafts----------------//
    public static CraftModel BasicCraft
    {
        get
        {
            CraftModel craft = new CraftModel("Basic Craft", CraftPartModel.BasicCapsule);
            craft.rootCraft.Model.craft = new ModelRef<CraftModel>(craft);
            craft.rootCraft.Model.AddCraftPartModel(CraftPartModel.LiquidFuelContainer, Vector3d.down * 1.08f, 0);
            craft.rootCraft.Model.craftParts[0].AddCraftPartModel(CraftPartModel.SpaceEngine, Vector3d.down * .805f, 0);

            return craft;
        }
    }
    
    //--------------------Craft Control-----------//
    public void CraftControl(double thrust, Vector2d translation, double rotatiton, double deltaTime)
    {
        CraftControl(deltaTime);
        Vector3d addedForce = Forces.Rotate(new Vector3d(translation.x, translation.y + throttle), (Rotation)); //forces located to local orientation
        torque = rotatiton;

        if (!playerControlled)
        {
            velocity += Forces.ForceToVelocity(this, addedForce, deltaTime);
            SystemPosition += velocity * deltaTime;
            //Rotation += LocalRotationRate;

            if (!closeToReference)
            {
                //Update reference point
                sol.Model.localReferencePoint = LocalPosition;
                sol.Model.localReferenceVel = LocalVelocity;
                sol.Model.localReferenceForce = force;
            }
        }

        
    }

    public void CraftControl(double deltaTime)
    {
        force = Forces.Force(this, true);

        if (!spawned) //Sets craft that are not spawned
        {
            if (State != ObjectState.Landed)
            {
                velocity += Forces.ForceToVelocity(force, mass, deltaTime);
                SystemPosition += velocity * deltaTime;
            }
            else
            {
                surfacePolar = surfacePolar; //Used to keep world position and velocity updated using, while not moving them on the surface
                SurfaceVel = SurfaceVel;
                LocalRotation = LocalRotation;
            }
        }

        //Check for SOI change. TODO: Change soi change checking to force based checking?
        double closestBody = (SystemPosition - reference.Model.SystemPosition).sqrMagnitude;

        for (int i = 0; i < sol.Model.allSolarBodies.Count; i++)
        {
            SolarBodyModel solarMod = sol.Model.allSolarBodies[i];
            double distance = (SystemPosition - solarMod.SystemPosition).sqrMagnitude;
            if (distance < closestBody && solarMod.SOI > distance)
            {
                reference = new ModelRef<SolarBodyModel>(solarMod);
                SystemPosition = SystemPosition; //update the new local positions and rotations
                velocity = velocity;
                Rotation = Rotation;
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
        double rotationDifference = RotationDifference(desiredRotation, Rotation);
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
    private void SASProgram(double desiredRotationRate = 0)
    {
        double rotation = 0;

        if (RotationRate != desiredRotationRate)
        {
            //if (RotationRate > desiredRotationRate)
            //{
            //    rotation = rotationSpeed * sol.Model.date.deltaTime;
            //}
            //else
            //{
            //    rotation = -rotationSpeed * sol.Model.date.deltaTime;
            //}

            //rgb.AddTorque((float)rotation);
            //model.LocalRotationRate = rgb.angularVelocity * Mathd.Deg2Rad;
            //if (Mathd.Abs(rgb.angularVelocity) < .1)
            //{ //It has reached slow enough speed to stop

            //    rgb.angularVelocity = 0;
            //    model.RotationRate = 0;
            //    model.Rotation = transform.rotation.eulerAngles.z * Mathd.Deg2Rad;
            //}
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

        if (rotationDifference > Mathd.PI)
        {
            rotationDifference -= 2 * Mathd.PI;
        }
        if (rotationDifference < -Mathd.PI)
        {
            rotationDifference += 2 * Mathd.PI;
        }
        return rotationDifference;
    }


    /// <summary>
    /// rotates craft to prograde
    /// </summary>
    private void ProgradeProgram()
    {
        double desiredRotationRate = DesiredRotationRate(new Polar2(LocalVelocity).angle - .5 * Mathd.PI); //Figure out rotation rate wanted

        SASProgram(desiredRotationRate); //Add torque


    }
    private void RetrogradeProgram()
    {
        double desiredRotationRate = DesiredRotationRate(new Polar2(LocalVelocity).angle - 1.5 * Mathd.PI); //Figure out rotation rate wanted

        SASProgram(desiredRotationRate); //Add torque


    }
    //--------------------------Autopilot--------------------//
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
}
