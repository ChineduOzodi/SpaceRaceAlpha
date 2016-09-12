using UnityEngine;
using System.Collections;
using System;

public class SpaceTrajectory : MonoBehaviour
{

    public Color c1 = Color.red;
    public Color c2 = Color.yellow;
    public float width = 1;
    public int vertsCount = 200;
    internal float SOI;
    internal Vector3[] verts;
    internal Vector3 distance;
    internal Vector3 m2Pos;
    internal Vector3 m2Vel;
    internal float m2;
    internal float G; //Same as G in Space Gravity

    // Use this for initialization
    void Start()
    {

        verts = new Vector3[vertsCount];
        G = Camera.main.GetComponent<SpaceGravityReal>().G / 100 * 2;

    }

    // Update is called once per frame
    void Update()
    {
        verts = new Vector3[vertsCount];
        if (distance.magnitude != 0)
        {
            DrawTraject(m2Pos);
        }


    }

    public void DrawTraject(Vector3 m2Pos)
    {
        float increment = 2 * Mathf.PI / (vertsCount - 1);

        Vector2 relVel = GetComponent<Rigidbody2D>().velocity - new Vector2(m2Vel.x, m2Vel.y);

        float angleY = Mathf.Deg2Rad * Vector2.Angle(relVel, distance);

        Vector2 perApo = PerApo(distance.magnitude, relVel.magnitude, m2 * G, angleY);
        //print("radius: " + distance.magnitude + " " + perApo);
        float a = SemiMajorAxis(relVel, distance, m2 * G);
        float e = Eccentricity(relVel.magnitude, distance.magnitude, m2 * G, angleY);
        Vector3 eVect = Eccentricity(relVel, distance, m2 * G);

        //print("radius: " + distance.magnitude + " Es: " + eVect.magnitude);
        //print("Radial start: " + CartToAngle(distance));

        for (int i = 0; i < vertsCount; i++)
        {
            float rad = Ellipse(a, e, i * increment + CartToAngle(distance));

            verts[i] = PolarToCartesian(new Vector2(rad, i * increment + CartToAngle(distance) + CartToAngle(eVect))) + m2Pos;
        }

        var line = gameObject.GetComponent<LineRenderer>();
        line.SetVertexCount(vertsCount);
        line.SetWidth(width * m2/500, width * m2/ 500);
        line.SetColors(c1, c2);

        line.SetPositions(verts);
    }

    private float Eccentricity(float v, float r, float GM, float angleY)
    {
        return Mathf.Sqrt(Mathf.Pow(((r * v * v / GM) - 1), 2) * Mathf.Pow(Mathf.Sin(angleY), 2) + Mathf.Pow(Mathf.Cos(angleY), 2));
    }
    private Vector3 Eccentricity(Vector3 v, Vector3 r, float GM)
    {
        return (Vector3.Cross(v, Vector3.Cross(r, v)) / GM - (r / r.magnitude));
    }

    public Vector2 CartesianToPolar(Vector3 point)
    {
        Vector2 polar;

        //calc longitude
        polar.y = Mathf.Atan2(point.x, point.z);

        //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
        var xzLen = new Vector2(point.x, point.z).magnitude;
        //atan2 does the magic
        polar.x = Mathf.Atan2(-point.y, xzLen);

        //convert to deg
        polar *= Mathf.Rad2Deg;

        return polar;
    }
    public float CartToAngle(Vector3 point)
    {
        float angle = Mathf.Atan(point.y / point.x);

        if (point.x < 0)
        {
            angle += Mathf.PI * 2;
        }
        else if (point.y < 0)
        {
            angle += Mathf.PI;
        }
        else
        {

            angle += Mathf.PI;
        }


        return angle;
    }

    public Vector3 PolarToCartesian(Vector2 polar)
    {
        //an origin vector, representing lat,lon of 0,0. 

        //var origin = new Vector3(0, 0, 0);
        //build a quaternion using euler angles for lat,lon
        // var rotation = Quaternion.Euler(polar.x, polar.y, 0);
        //transform our reference vector by the rotation. Easy-peasy!
        //Vector3 point = rotation * origin;

        //return point;

        return new Vector3(polar.x * Mathf.Cos(polar.y), polar.x * Mathf.Sin(polar.y));
    }

    public Vector2 PerApo(float r, float v, float GM, float angleY)
    {
        Vector2 perApo = new Vector2();

        float C = (2 * GM) / (r * v * v);

        perApo.x = (-C + Mathf.Sqrt((C * C) - (4 * (1 - C) * (-Mathf.Pow(Mathf.Sin(angleY), 2))))) / (2 * (1 - C));
        perApo.y = (-C - Mathf.Sqrt(C * C - 4 * (1 - C) * (-Mathf.Pow(Mathf.Sin(angleY), 2)))) / (2 * (1 - C));

        perApo = (perApo.x < perApo.y) ? perApo * r : new Vector2(perApo.y, perApo.x) * r;

        return perApo;
    }

    public float SemiMajorAxis(Vector3 vel, Vector3 r, float GM)
    {
        float sEnergy = ((Mathf.Pow(vel.magnitude, 2) * .5f) - (GM / r.magnitude));

        return -(GM / (2 * sEnergy));
    }

    public float Ellipse(float a, float e, float degree)
    {
        return (a * (1 - Mathf.Pow(e, 2))) / (1 + e * Mathf.Cos(degree));
    }
}
