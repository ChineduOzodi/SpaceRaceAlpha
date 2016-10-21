using UnityEngine;
using System.Collections;
using System;

public class OrbitalInfo {
    /// <summary>
    /// Determines and stores orbital info for a model
    /// </summary>
    ///

    private  double alt;
    private  Vector3d ecc;
    private  double eccMag;
    private  double per;
    private  double apo;
    private  double semiMajorAxis;
    public Vector3d Ecc //eccentricity
    {
        get { return ecc; }
    }
    public double EccMag //magnitude of ecc vector
    {
        get { return eccMag; }
    }

    public double Per
    {
        get { return per; }
    }

    public double Apo
    {
        get { return apo; }
    }

    public double Alt
    {
        get { return alt; }
    }
    public double SemiMajorAxis
    {
        get { return semiMajorAxis; }
    }


    public OrbitalInfo(BaseModel model, double G)
    {
        Vector3d altitude = model.LocalPosition;     //distance b/w object one and reference in km

        double GM = G * model.reference.Model.mass * .02f;

        if (model.type == ObjectType.Spacecraft)
        {
            GM = G * model.reference.Model.mass * .02f;                                           //modified gravitational constant
        }

        Vector3d relVel = model.LocalVelocity ;       //velocity relative to parent object

        double angleY = Mathd.Deg2Rad * Vector2d.Angle(relVel, altitude);         //angle of trajectory with reference to the reference object
        Vector2d perApo = PerApo(altitude.magnitude, relVel.magnitude, GM , angleY);
        Vector3d eccentricity = Eccentricity(relVel, altitude, GM);

        //set properties
        alt = altitude.magnitude - model.reference.Model.radius;
        per = perApo.x - model.reference.Model.radius;
        apo = perApo.y - model.reference.Model.radius;
        ecc = eccentricity;
        eccMag = eccentricity.magnitude;
        semiMajorAxis = GetSemiMajorAxis(relVel, altitude, GM);


    }
    //public static double Eccentricity(double v, double r, double GM, double angleY)
    //{
    //    return Mathd.Sqrt(Mathd.Pow(((r * v * v / GM) - 1), 2) * Mathd.Pow(Mathd.Sin(angleY), 2) + Mathd.Pow(Mathd.Cos(angleY), 2));
    //}
    private static Vector3d Eccentricity(Vector3d v, Vector3d r, double GM)
    {
        return (Vector3d.Cross(v, Vector3d.Cross(r, v)) / GM - (r / r.magnitude));
    }

    public static double GetSemiMajorAxis(Vector3d vel, Vector3d r, double GM)
    {
        double sEnergy = ((Mathd.Pow(vel.magnitude, 2) * .5f) - (GM / r.magnitude));

        return -(GM / (2 * sEnergy));
    }
    private static Vector2d PerApo(double r, double v, double GM, double angleY)
    {
        Vector2d perApo = new Vector2d();

        double C = (2 * GM) / (r * v * v);

        perApo.x = (-C + Mathd.Sqrt((C * C) - (4 * (1 - C) * (-Mathd.Pow(Mathd.Sin(angleY), 2))))) / (2 * (1 - C));
        perApo.y = (-C - Mathd.Sqrt(C * C - 4 * (1 - C) * (-Mathd.Pow(Mathd.Sin(angleY), 2)))) / (2 * (1 - C));

        perApo = (perApo.x < perApo.y) ? perApo * r : new Vector2d(perApo.y, perApo.x) * r;

        return perApo;
    }

    public static string GetInfo(BaseModel model, double G)
    {
        OrbitalInfo orbit = new OrbitalInfo(model, G * 50);
        if (model.type == ObjectType.Spacecraft)
            return String.Format("Mass: {0} kg\nGravity: {1} m/s^2\n Velocity: {6} m/s\nAlt: {2} km\nApo: {3} km\nPer: {4} km\nEcc: {5}",
            model.mass, (model.force.magnitude / model.mass).ToString("0.00"), (orbit.Alt * .001f).ToString("0.000"), (orbit.Apo * .001f).ToString("0.00"), (orbit.Per * .001f).ToString("0.00"), orbit.EccMag.ToString("0.00"), model.velocity.magnitude.ToString("0.00"));
        else
        {
            
            return String.Format("Radius: {0} km\nGravity: {1} m/s^2\n Velocity: {6} m/s\nAlt: {2} km\nApo: {3} km\nPer: {4} km\nRotaion Period: {5} hrs",
            model.mass, (((PlanetModel) model).radius * .001f).ToString("0.00"), (orbit.Alt * .001f).ToString("0.000"), (orbit.Apo * .001f).ToString("0.00"), (orbit.Per * .001f).ToString("0.00"), (360 / ((PlanetModel)model).rotationRate / Date.Hour).ToString("0.00"), model.velocity.magnitude.ToString("0.00"));

        }


    }
}
