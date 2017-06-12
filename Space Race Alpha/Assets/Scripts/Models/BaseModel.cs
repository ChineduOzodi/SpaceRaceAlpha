using UnityEngine;
using System.Collections;
using CodeControl;

public class BaseModel : Model
{
    //-----------Public Properties---------------//
    public string name;
    /// <summary>
    /// reference to the current solar system
    /// </summary>
    public ModelRef<SolarSystemModel> sol;

    //parent object
    public ModelRef<BaseModel> reference;
    public ModelRef<SolarBodyModel> referenceBody;

    public double radius;
    public double deltaTime;
    //basic physics info
    public double mass;
    public double density;
    /// <summary>
    /// force acting on object relative to reference object. In m/s
    /// </summary>
    public Vector3d force = Vector3d.zero;

    //-----------Private properties-------------//
    private ObjectType type;
    private ObjectState state;

    //------------Public fields-----------------//

    public ObjectType Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public ObjectState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    //----------Position

    /// <summary>
    /// world position of model
    /// </summary>
    public Vector3d SystemPosition

    {
        get { return solarPosition; }
        set {
            solarPosition = value;

            localPosition = solarPosition  - reference.Model.SystemPosition;
            pol = new Polar2(localPosition);
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

            solarPosition = reference.Model.SystemPosition + localPosition;
            pol = new Polar2(localPosition);
        }
    }
    /// <summary>
    /// Local Polar position in relation to the reference object with the angle in radians
    /// </summary>
    public Polar2 polar
    {
        get { return pol; }
        set
        {
            pol = value;

            localPosition = Polar2.PolarToCartesian(pol);
            solarPosition = reference.Model.SystemPosition + localPosition;
        }
    }
    /// <summary>
    /// Polar position in relationship to the surface of the body. Can be used to get position or above body
    /// </summary>
    public Polar2 surfacePolar {
        get { return new Polar2(pol.radius, pol.angle - reference.Model.rotation); }
        set
        {
            pol = new Polar2(value.radius, reference.Model.rotation + value.angle);

            localPosition = Polar2.PolarToCartesian(pol);
            solarPosition = reference.Model.SystemPosition + localPosition;
        }
    }

    //-------------Velocity

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
    /// velocity relative to the surface of reference body
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

    //-----------Forces

    /// <summary>
    /// Force relative to reference position. Make sure you are updating the deltaTime variable. TODO: Need to make
    /// sure it is working
    /// </summary>
    public Vector3d relativeForce //TODO: correct this, need to calculated theoretical force for the reference point of meshes
    {
        get
        {
            Vector3d velAhead = LocalVelocity + Forces.ForceToVelocity(force, mass, deltaTime);
            Vector3d posAhead = Forces.VelocityToPosition(localPosition, velAhead, deltaTime);
            Vector3d forceAhead = Forces.Rotate(Forces.Force(mass, reference.Model.mass, posAhead), new Polar2(posAhead).angle - pol.angle);

            Vector3d changeInForce = -forceAhead - force;
            return changeInForce / deltaTime;
        }
    }

    //----------------Rotation

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
        }
    }
    /// <summary>
    /// z axis rotation of object in relation to reference rotation (in radians) (can also be called surface rotation)
    /// </summary>
    public double LocalRotation
    {
        get
        {
            return (rotation - reference.Model.rotation) % (2 * Mathd.PI);
        }
        set
        {
            rotation = (value + reference.Model.rotation) % (2 * Mathd.PI);
        }
    }
    /// <summary>
    /// global rotation rate in degrees per second. NOT rotation rate of orbit
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
    /// local rotation rate of object in reference to reference object in radians per second.
    /// NOT rotation rate of orbit
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

    //---------------------Orbital Info--------------------------------//

    /// <summary>
    /// get altitude above body radius. in m
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
            Vector2d pA = PerApo;
            double semiMA = (pA.x + pA.y) / 2f;

            return semiMA;

        }
    }
    public double SemiMinorAxis
    {
        get
        {
            Vector2d pA = PerApo;
            return Mathd.Sqrt(pA.x * pA.y);
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
            //double sEnergy = ((Mathd.Pow(LocalVelocity.magnitude, 2) * .5f) - ((reference.Model.mass * Forces.G) / pol.radius));
            //double energy = -((reference.Model.mass * Forces.G) / (2 * sEnergy));
            //return Mathd.Sqrt((Forces.G * reference.Model.mass) / Mathd.Pow(energy, 3)); //energy could be semiMajorAxis

            return (2 * Mathd.PI) / OrbitalPeriod;
        }
    }
    //--------------------Private fields------------------------------//

    private Vector3d localPosition;
    private Vector3d solarPosition;
    private Polar2 pol;
    
    private double rotation = 0;
    private double rotationRate = 0;
    private double localRotationRate = 0;

    private Vector3d vel = Vector3d.zero;

    //------------Constructors-----------------------//

    //-----------Public Functions----------------------//

    public Vector3d LocalPositionKeplar(double deltaTime) //TODO: Fix this so it solar bodies can move to a solar orbit when time is accelerated
    {
        Vector3d localPosition = Vector3d.zero;
        double ecc = Ecc.magnitude;
        if (LocalPosition.sqrMagnitude < .01)
            return LocalPosition;

        if (ecc < 1) //Cuurently catches all orbits and makes them circular
        {
            polar = new Polar2(polar.radius, polar.angle + MeanMotion * deltaTime);
            velocity = SolarSystemModel.VelocityFromOrbit(this);
            return LocalPosition;
        }
        double trueAnom = TrueAnomaly(LocalPosition.magnitude, ecc);

        double E0 = EccentricAnomaly(trueAnom, ecc);

        //localPosition.x = (Mathd.Cos(E0) - ecc) * SemiMajorAxis;
        //localPosition.y = Mathd.Sin(E0) * SemiMinorAxis; //* Mathd.Sqrt(1.0 - ecc * ecc);

        //localPosition = new Polar2(localPosition.magnitude, Polar2.CartesianToPolar(localPosition).angle + Polar2.CartesianToPolar(Ecc).angle + Mathd.PI).cartesian;

        double E = EccentricAnomaly(E0, ecc, deltaTime, 10);
        localPosition.x = (Mathd.Cos(E) - ecc) * SemiMajorAxis;
        localPosition.y = Mathd.Sin(E) * SemiMinorAxis; //* Mathd.Sqrt(1.0 - ecc * ecc);
        localPosition = new Polar2(localPosition.magnitude, Polar2.CartesianToPolar(localPosition).angle + Polar2.CartesianToPolar(Ecc).angle + Mathd.PI).cartesian;

        LocalPosition = localPosition;
        double vel = Mathd.Sqrt(Forces.G * reference.Model.mass * (2 / localPosition.magnitude - 1 / SemiMajorAxis));
        double flightAngle = FlightAngle();
        Polar2 velAngle = Polar2.CartesianToPolar(localPosition.normalized);
        velAngle.angle += flightAngle - (.5 * Mathd.PI); //To get it facing the same way as the velocity should
        LocalVelocity = velAngle.cartesian * vel;

        return localPosition;
    }

    //------------Private Functions-----------------//

    private double TrueAnomaly(double r, double ecc) //Checked and should be accurate
    {
        //double trueAnom = Mathd.Acos((SemiMajorAxis * (1 - Mathd.Pow(ecc, 2)) - r) / (ecc * r));
        //if (FlightAngle() < 0)
        //    trueAnom *= -1;
        //return trueAnom;
        double dot = Vector3d.Dot(Ecc, LocalPosition);
        double trueA = Mathd.Acos(dot / (Ecc.magnitude * LocalPosition.magnitude));
        if (dot < 0)
            return 2 * Mathd.PI - trueA;
        else return trueA;



    }

    private double EccentricAnomaly(double trueAnomaly, double ecc) //Checked, should be accurate
    {
        
        double eccentr = Mathd.Acos((ecc + Mathd.Cos(trueAnomaly)) / (1 + ecc * Mathd.Cos(trueAnomaly)));
        if (trueAnomaly > Mathd.PI)
            return 2 * Mathd.PI - eccentr;
        else return eccentr;
    }
    /// <summary>
    /// Eccentric anomaly at a future time given the current eccentric anomaly
    /// </summary>
    /// <param name="E0">current eccentric anomaly</param>
    /// <param name="ecc">eccentricity</param>
    /// <param name="deltaTime">change in time</param>
    /// <param name="n">number of iterations</param>
    /// <returns></returns>
    private double EccentricAnomaly(double E0, double ecc, double deltaTime, double n)
    {
        double M0 = E0 - ecc * Mathd.Sin(E0);
        double M = M0 + MeanMotion * deltaTime;
        double E = M;
        for (int i = 0; i < n; i++)
        {
            E = M + ecc * Mathd.Sin(E);
        }
        return E;
    }
    private double FlightAngle()
    {
        double Y = polar.angle - Polar2.CartesianToPolar(LocalVelocity).angle;

        return (.5 * Mathd.PI - Y);
    }


}
