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
            craft.rootCraft.Model.AddCraftPartModel(CraftPartModel.LiquidFuelContainer, Vector3d.down, 0);
            craft.rootCraft.Model.craftParts[0].AddCraftPartModel(CraftPartModel.SpaceEngine, Vector3d.down, 0);

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
        force = Forces.Force(this, sol.Model.allSolarBodies);

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

        //Check for SOI change
        double closestBody = (SystemPosition - reference.Model.SystemPosition).magnitude;

        for (int i = 0; i < sol.Model.allSolarBodies.Count; i++)
        {
            SolarBodyModel solarMod = sol.Model.allSolarBodies[i];
            double distance = (SystemPosition - solarMod.SystemPosition).magnitude;
            if (distance < closestBody && solarMod.SOI > distance)
            {
                reference = new ModelRef<BaseModel>(solarMod);
                SystemPosition = SystemPosition; //update the new local positions and rotations
                velocity = velocity;
                Rotation = Rotation;
            }
        }
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
