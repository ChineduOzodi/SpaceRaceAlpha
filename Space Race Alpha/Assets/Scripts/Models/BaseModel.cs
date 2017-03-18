﻿using UnityEngine;
using System.Collections;
using CodeControl;

public class BaseModel : Model {

    //name, state, and type info
    public string name;
    public ObjectType type;
    public ObjectState state;
    public string spriteName;

    public ModelRef<SolarSystemModel> sol;

    //Public Properties

    /// <summary>
    /// world position of model
    /// </summary>
    public Vector3d position
    {
        get { return worldPosition; }
        set {
            worldPosition = value;

            localPosition = - reference.Model.position + worldPosition;
            pol = new Polar2(localPosition);
            surfPol = new Polar2(pol.radius, pol.angle - reference.Model.rotation);
        }
    }
    /// <summary>
    /// Position of model relative to reference object
    /// </summary>
    public Vector3d LocalPosition
    {
        get { return localPosition; }
        set
        {
            localPosition = value;

            worldPosition = reference.Model.position + localPosition;
            pol = new Polar2(localPosition);
            surfPol = new Polar2(pol.radius, pol.angle - reference.Model.rotation);
        }
    }
    /// <summary>
    /// Polar position in relation to the reference object with the angle in radians
    /// </summary>
    public Polar2 polar
    {
        get { return pol; }
        set
        {
            pol = value;
            surfPol = new Polar2(pol.radius, pol.angle - reference.Model.rotation);

            localPosition = Polar2.PolarToCartesian(pol);
            worldPosition = reference.Model.position + localPosition;
        }
    }
    /// <summary>
    /// Polar position in relationship to the surface of the body. Can be used to get position or above body
    /// </summary>
    public Polar2 surfacePolar {
        get { return surfPol; }
        set
        {
            surfPol = value;

            pol = new Polar2(surfPol.radius, reference.Model.rotation);

            localPosition = Polar2.PolarToCartesian(pol);
            worldPosition = reference.Model.position + localPosition;
        }
    }
    /// <summary>
    /// world space velocity of model
    /// </summary>
    public Vector3d velocity
    {
        get { return vel; }
        set
        {
            vel = value;

            //localVel = 

            //var relPolVel = new Polar2(localVel);

            //surfaceVel = Forces.Rotate(localVel, -pol.angle + .5 * Mathd.PI) - Forces.Rotate(new Vector3d(polar.radius * -reference.Model.RotationRate, 0, 0), -pol.angle + .5 * Mathd.PI); //Rotate it
        }
    }
    /// <summary>
    /// The Velocity of model relative to reference object
    /// </summary>
    public Vector3d LocalVelocity
    {
        get {

            return  vel - reference.Model.velocity;
        }
        set
        {

            vel = reference.Model.velocity + value;

            //surfaceVel = Forces.Rotate(localVel, -pol.angle + .5 * Mathd.PI) - Forces.Rotate(new Vector3d(polar.radius * -reference.Model.RotationRate,0,0), - pol.angle + .5 * Mathd.PI); //Rotate it
        }
    }
    /// <summary>
    /// velocity relative to the surface of body
    /// </summary>
    public Vector3d SurfaceVel
    {
        get {

            return Forces.Rotate(LocalVelocity, -pol.angle + .5 * Mathd.PI) - new Vector3d(polar.radius * -reference.Model.RotationRate, 0, 0);
        }

        set
        {
            LocalVelocity = Forces.Rotate(value + new Vector3d(polar.radius * -reference.Model.RotationRate, 0, 0), pol.angle - .5 * Mathd.PI); //Rotate it by the local position angle of - 90 degrees so that the planes work out
        }

    }
    /// <summary>
    /// Force relative to reference position
    /// </summary>
    public Vector3d relativeForce //TODO: correct this, need to calculated theoretical force for the reference point of meshes
    {
        get
        {
            Vector3d velAhead = LocalVelocity + Forces.ForceToVelocity(force, mass);
            Vector3d posAhead = Forces.VelocityToPosition(localPosition, velAhead);
            Vector3d forceAhead = Forces.Rotate(Forces.Force(mass, reference.Model.mass, posAhead), new Polar2(posAhead).angle - pol.angle);

            Vector3d changeInForce = -forceAhead - force;
            return changeInForce / Time.deltaTime;
        }
    }
    //----------------Public fields------------------------------//

    /// <summary>
    /// z axis rotation of object in global parameters (in radians)
    /// </summary>
    public double Rotation
    {
        get
        {
            return rotation;
        }
        set
        {
            rotation = value % (2 * Mathd.PI);
            localRotation = (rotation - reference.Model.rotation) % (2 * Mathd.PI);           
        }
    }
    /// <summary>
    /// z axis rotation of object in relation to reference rotation (in radians) (can also be called surface rotation)
    /// </summary>
    public double LocalRotation
    {
        get
        {
            return localRotation;
        }
        set
        {
            localRotation = value % (2 * Mathd.PI);
            rotation = (localRotation + reference.Model.rotation) % (2 * Mathd.PI);
        }
    }
    /// <summary>
    /// global rotation rate in radians per second
    /// </summary>
    public double RotationRate
    {
        get
        {
            return rotationRate;
        }
        set
        {
            rotationRate = value;
            var orbitalRotationRate = SurfaceVel.x / pol.radius;
            localRotationRate = rotationRate + orbitalRotationRate;
        }
    }
    /// <summary>
    /// local rotation rate of object in reference to reference object. NOT rotation rate of orbit
    /// </summary>
    public double LocalRotationRate
    {
        get
        {
            return localRotationRate;
        }
        set
        {
            localRotationRate = value;

            if (pol.radius == 0)
            {
                rotationRate = value; //deals with references to self, where you would be deviding by 0
            }
            else
                rotationRate = localRotationRate - reference.Model.SurfaceVel.x / pol.radius;
        }
    }


    //basic physics info
    public double mass;
    public double density;
    /// <summary>
    /// force considered relative to reference object;
    /// </summary>
    public Vector3d force = Vector3d.zero;

    //---------------------Orbital Info--------------------------------//

    /// <summary>
    /// get altitude above body radius
    /// </summary>
    public double alt
    {
        get
        {
            return polar.radius - reference.Model.radius;
        }

    }
    public Vector3d Ecc //eccentricity
    {
        get { return (Vector3d.Cross(LocalVelocity, Vector3d.Cross(localPosition, LocalVelocity)) / (reference.Model.mass * Forces.G) - (localPosition / pol.radius)); }
    }

    public Vector2d PerApo
    {
        get {
            Vector2d perApo = new Vector2d();
            double angleY = Mathd.Deg2Rad * Vector2d.Angle(LocalVelocity, localPosition);         //angle of trajectory with reference to the reference object

            double C = (2 * (reference.Model.mass * Forces.G)) / (pol.radius * LocalVelocity.magnitude * LocalVelocity.magnitude);

            perApo.x = (-C + Mathd.Sqrt((C * C) - (4 * (1 - C) * (-Mathd.Pow(Mathd.Sin(angleY), 2))))) / (2 * (1 - C));
            perApo.y = (-C - Mathd.Sqrt(C * C - 4 * (1 - C) * (-Mathd.Pow(Mathd.Sin(angleY), 2)))) / (2 * (1 - C));

            perApo = (perApo.x < perApo.y) ? perApo * pol.radius : new Vector2d(perApo.y, perApo.x) * pol.radius;

            return perApo;
        }
    }
    public double SemiMajorAxis
    {
        get {

            double sEnergy = ((Mathd.Pow(LocalVelocity.magnitude, 2) * .5f) - ((reference.Model.mass * Forces.G) / pol.radius));

            return -((reference.Model.mass * Forces.G) / (2 * sEnergy));

        }
    }
    /// <summary>
    /// Orbital period in seconds
    /// </summary>
    public double OrbitalPeriod
    {
        get
        {
            return Mathd.PI * 2 * Mathd.Sqrt(Mathd.Pow(SemiMajorAxis, 3) / (reference.Model.mass * Forces.G));
        }
    }
    /// <summary>
    /// angle of prograde vector in relation with the surface
    /// </summary>
    public double ProgradeSurfaceAngle
    {
        get
        {
            double pSA = (new Polar2(Forces.Rotate(LocalVelocity, pol.angle - .5 * Mathd.PI)).angle);
            if (pSA > 2 * Mathd.PI)
            {
                pSA -= 2 * Mathd.PI;
            }
            else if (pSA < 0)
            {
                pSA += 2 * Mathd.PI;
            }
            return pSA;
        }
    }
    public double RetrogradeSurfaceAngle
    {
        get
        {
            double rSA = (new Polar2(Forces.Rotate(LocalVelocity, pol.angle - .5 * Mathd.PI)).angle + 1 * Mathd.PI);
            if (rSA > 2 * Mathd.PI)
            {
                rSA -= 2 * Mathd.PI;
            }
            else if (rSA < 0)
            {
                rSA += 2 * Mathd.PI;
            }
            return rSA;
        }
    }
    /// <summary>
    /// average speed of satellite
    /// </summary>
    public double MeanMotion
    {
        get
        {
            return Mathd.Sqrt((Forces.G * reference.Model.mass) / Mathd.Pow(SemiMajorAxis, 3));
        }
    }

    public double TrueAnomly
    {
        get
        {
            //time past perigee
            double t = OrbitalPeriod * .5;
            double MeanAnomaly = MeanMotion * t;
            double E0 = MeanAnomaly;

            for (int i = 0; i < 10; i++)
            {
                E0 = E0 - ((E0 - Mathd.Epsilon * Mathd.Sin(E0) - MeanAnomaly) / (1 - Mathd.Epsilon * Mathd.Cos(E0)));
            }
            return E0;
        }
    }

    //parent object
    public ModelRef<BaseModel> reference;
    public ModelRef<SolarBodyModel> referenceBody;

    public double radius;

    //--------------------Private fields------------------------------//

    private Vector3d localPosition;
    private Vector3d worldPosition;
    private Polar2 pol;
    private Polar2 surfPol;
    
    private double rotation = 0;
    private double localRotation = 0;
    private double rotationRate = 0;
    private double localRotationRate = 0;

    protected Vector3d vel = Vector3d.zero;
    
}
