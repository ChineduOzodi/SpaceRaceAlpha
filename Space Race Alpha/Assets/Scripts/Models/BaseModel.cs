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
            surfPol = new Polar2(pol.radius, pol.angle - reference.Model.rotation.eulerAngles.z * Mathd.Deg2Rad);
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
            surfPol = new Polar2(pol.radius, pol.angle - reference.Model.rotation.eulerAngles.z * Mathd.Deg2Rad);
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
            surfPol = new Polar2(pol.radius, pol.angle - reference.Model.rotation.eulerAngles.z * Mathd.Deg2Rad);

            localPosition = Polar2.PolarToCartesian(pol);
            worldPosition = reference.Model.position + localPosition;
        }
    }
    public Polar2 surfacePolar {
        get { return surfPol; }
        set
        {
            surfPol = value;

            pol = new Polar2(surfPol.radius, reference.Model.rotation.eulerAngles.z * Mathd.Deg2Rad  + surfPol.angle);

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

            surfaceVel = new Polar2(relPolVel.radius, relPolVel.angle - reference.Model.rotation.eulerAngles.z * Mathd.Deg2Rad).cartesian;
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
    /// velocity relative to the reference point set by either a planer mesh or a craft
    /// </summary>
    public Vector3d referencePointSurfaceVelocity
    {
        get
        {
            return Forces.Rotate(relVel, new Polar2(sol.Model.localReferencePoint).angle + sol.Model.localReferencePointRotation.eulerAngles.z * Mathd.Deg2Rad - .5d * Mathd.PI);
        }

        set
        {
            relVel = Forces.Rotate(value, -new Polar2(sol.Model.localReferencePoint).angle - sol.Model.localReferencePointRotation.eulerAngles.z * Mathd.Deg2Rad - .5d * Mathd.PI);
        }
    }
    /// <summary>
    /// The force vector in reference to the reference point
    /// </summary>
    public Vector3d referencePointSurfaceForce
    {
        get
        {
            return Forces.Rotate(force, new Polar2(sol.Model.localReferencePoint).angle + sol.Model.localReferencePointRotation.eulerAngles.z * Mathd.Deg2Rad - .5d * Mathd.PI);
        }

        set
        {
            force = Forces.Rotate(value, - new Polar2(sol.Model.localReferencePoint).angle - sol.Model.localReferencePointRotation.eulerAngles.z * Mathd.Deg2Rad - .5d * Mathd.PI);
        }
    }
    public Vector3d relativeForce
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

    //global position in world settings
    public Quaternion rotation = Quaternion.identity; //rotation speed
    /// <summary>
    /// rotation in radians per second
    /// </summary>
    public double rotationRate = 0;


    //basic physics info
    public double mass;
    public double density;
    /// <summary>
    /// force considered relative to reference object;
    /// </summary>
    public Vector3d force = Vector3d.zero;

    //orbital info
    public Vector3d altitude;
    public OrbitalInfo orbitalInfo;

    //parent object
    public ModelRef<SolarBodyModel> reference;

    //--------------------Private fields------------------------------//

    private Vector3d localPosition;
    private Vector3d worldPosition;
    private Polar2 pol;
    private Polar2 surfPol;

    private Vector3d vel = Vector3d.zero;
    private Vector3d relVel = Vector3d.zero;
    private Vector3d surfaceVel;
    
}
