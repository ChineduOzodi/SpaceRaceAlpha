using UnityEngine;
using System.Collections;
using System;

public class OrbitalInfo {
    /// <summary>
    /// Determines and stores orbital info for a model
    /// </summary>
    ///

    private  float alt;
    private  Vector3 ecc;
    private  float eccMag;
    private  float per;
    private  float apo;
    private  float semiMajorAxis;
    public Vector3 Ecc //eccentricity
    {
        get { return ecc; }
    }
    public float EccMag //magnitude of ecc vector
    {
        get { return eccMag; }
    }

    public float Per
    {
        get { return per; }
    }

    public float Apo
    {
        get { return apo; }
    }

    public float Alt
    {
        get { return alt; }
    }
    public float SemiMajorAxis
    {
        get { return semiMajorAxis; }
    }


    public OrbitalInfo(BaseModel model)
    {
        Vector3 altitude = model.position - model.reference.Model.position;     //distance b/w object one and reference
        float GM = Forces.G / 100 * 2 * model.reference.Model.mass;                                           //modified gravitational constant
        Vector3 relVel = model.velocity - model.reference.Model.velocity;       //velocity relative to parent object

        float angleY = Mathf.Deg2Rad * Vector2.Angle(relVel, altitude);         //angle of trajectory with reference to the reference object
        Vector2 perApo = PerApo(altitude.magnitude, relVel.magnitude, GM , angleY);
        Vector3 eccentricity = Eccentricity(relVel, altitude, GM);

        //set properties
        alt = altitude.magnitude - model.reference.Model.radius;
        per = perApo.x - model.reference.Model.radius;
        apo = perApo.y - model.reference.Model.radius;
        ecc = eccentricity;
        eccMag = eccentricity.magnitude;
        semiMajorAxis = GetSemiMajorAxis(relVel, altitude, GM);


    }
    //public static float Eccentricity(float v, float r, float GM, float angleY)
    //{
    //    return Mathf.Sqrt(Mathf.Pow(((r * v * v / GM) - 1), 2) * Mathf.Pow(Mathf.Sin(angleY), 2) + Mathf.Pow(Mathf.Cos(angleY), 2));
    //}
    private static Vector3 Eccentricity(Vector3 v, Vector3 r, float GM)
    {
        return (Vector3.Cross(v, Vector3.Cross(r, v)) / GM - (r / r.magnitude));
    }

    public static float GetSemiMajorAxis(Vector3 vel, Vector3 r, float GM)
    {
        float sEnergy = ((Mathf.Pow(vel.magnitude, 2) * .5f) - (GM / r.magnitude));

        return -(GM / (2 * sEnergy));
    }
    private static Vector2 PerApo(float r, float v, float GM, float angleY)
    {
        Vector2 perApo = new Vector2();

        float C = (2 * GM) / (r * v * v);

        perApo.x = (-C + Mathf.Sqrt((C * C) - (4 * (1 - C) * (-Mathf.Pow(Mathf.Sin(angleY), 2))))) / (2 * (1 - C));
        perApo.y = (-C - Mathf.Sqrt(C * C - 4 * (1 - C) * (-Mathf.Pow(Mathf.Sin(angleY), 2)))) / (2 * (1 - C));

        perApo = (perApo.x < perApo.y) ? perApo * r : new Vector2(perApo.y, perApo.x) * r;

        return perApo;
    }

    public static string GetInfo(BaseModel model)
    {
        OrbitalInfo orbit = new OrbitalInfo(model);

        return String.Format("Mass: {0} kg\nGravity: {1} m/s^2\nAlt: {2} m\nApo: {3} m\nPer: {4} m\nEcc: {5}",
            model.mass, (model.force.magnitude / model.mass).ToString("0.00"), orbit.Alt.ToString("0"), orbit.Apo.ToString("0"), orbit.Per.ToString("0"), orbit.EccMag.ToString("0.00"));


    }
}
