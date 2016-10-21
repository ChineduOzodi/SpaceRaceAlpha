using UnityEngine;
using System.Collections;
using CodeControl;

public class BaseModel : Model {

    //name, state, and type info
    public string name;
    public ObjectType type;
    public ObjectState state;

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

            localPosition = reference.Model.position - worldPosition;
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

            relVel = reference.Model.velocity - vel;

            var relPolVel = new Polar2(relVel);

            surfaceVel = new Polar2(relPolVel.radius, relPolVel.angle - reference.Model.rotation).cartesian;
        }
    }
    /// <summary>
    /// The Velocity of model relative to reference object
    /// </summary>
    public Vector3d LocalVelocity
    {
        get { return relVel; }
        set
        {
            relVel = value;

            vel = reference.Model.velocity + relVel;

            surfaceVel = Forces.Rotate(relVel, -pol.angle - .5 * Mathd.PI); //Rotate it by the local position angle of - 90 degrees so that the planes work out
        }
    }
    /// <summary>
    /// velocity relative to the surface of body
    /// </summary>
    public Vector3d SurfaceVel {
        get { return surfaceVel; }

        set
        {
            surfaceVel = value;

            relVel = Forces.Rotate(surfaceVel, pol.angle - .5 * Mathd.PI); //Rotate it by the local position angle of - 90 degrees so that the planes work out

            vel = reference.Model.velocity + relVel;
        }

    }
    /// <summary>
    /// Force relative to reference position
    /// </summary>
    public Vector3d relativeForce //TODO: correct this, need to calculated theoretical force for the reference point of meshes
    {
        get
        {
            Vector3d velAhead = relVel + Forces.ForceToVelocity(force, mass);
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
    public double rotation = 0;
    /// <summary>
    /// z axis rotation of object in relation to reference rotation (in radians)
    /// </summary>
    public double localRotation
    {
        get
        {
            return rotation - reference.Model.rotation;
        }
        set
        {
            rotation = value + reference.Model.rotation;
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

            localRotationRate = rotationRate - surfaceVel.x / pol.radius;
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

            rotationRate = localRotationRate + surfaceVel.x / pol.radius;
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
        get { return (Vector3d.Cross(relVel, Vector3d.Cross(localPosition, relVel)) / (reference.Model.mass * Forces.G) - (localPosition / pol.radius)); }
    }

    public Vector2d PerApo
    {
        get {
            Vector2d perApo = new Vector2d();
            double angleY = Mathd.Deg2Rad * Vector2d.Angle(relVel, localPosition);         //angle of trajectory with reference to the reference object

            double C = (2 * (reference.Model.mass * Forces.G)) / (pol.radius * relVel.magnitude * relVel.magnitude);

            perApo.x = (-C + Mathd.Sqrt((C * C) - (4 * (1 - C) * (-Mathd.Pow(Mathd.Sin(angleY), 2))))) / (2 * (1 - C));
            perApo.y = (-C - Mathd.Sqrt(C * C - 4 * (1 - C) * (-Mathd.Pow(Mathd.Sin(angleY), 2)))) / (2 * (1 - C));

            perApo = (perApo.x < perApo.y) ? perApo * pol.radius : new Vector2d(perApo.y, perApo.x) * pol.radius;

            return perApo;
        }
    }
    public double SemiMajorAxis
    {
        get {

            double sEnergy = ((Mathd.Pow(relVel.magnitude, 2) * .5f) - ((reference.Model.mass * Forces.G) / pol.radius));

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

    //parent object
    public ModelRef<SolarBodyModel> reference;

    //--------------------Private fields------------------------------//

    private Vector3d localPosition;
    private Vector3d worldPosition;
    private Polar2 pol;
    private Polar2 surfPol;

    private double rotationRate = 0;
    private double localRotationRate = 0;

    private Vector3d vel = Vector3d.zero;
    private Vector3d relVel = Vector3d.zero;
    private Vector3d surfaceVel;
    
}
