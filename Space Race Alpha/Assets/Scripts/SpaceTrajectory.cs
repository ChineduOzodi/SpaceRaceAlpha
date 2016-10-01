using UnityEngine;
using System.Collections;
using System;
using CodeControl;

public class SpaceTrajectory : MonoBehaviour
{

    public Color c1 = Color.red;//colors for orbit
    public Color c2 = Color.yellow;
    internal float width = 1;
    public int vertsCount = 200;

    internal BaseModel model;

    internal float SOI;
    internal Vector3[] verts;
    internal Vector3 distance;
    internal Vector3 m2Pos;
    internal Vector3 m2Vel;
    internal float m2;
    internal float G; //Same as G in Space Gravity

    //MapMode variables
    bool mapMode = false;
    Camera mainCam;
    Camera mapCam;

    // Use this for initialization
    void Start()
    {
        //Add listeners
        Message.AddListener<ToggleMapMessage>(ToggleMapMode);

        //Set Cameras
        mainCam = Camera.main;
        mapCam = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();

        verts = new Vector3[vertsCount];
        G = Forces.G / 100 * 2; //Camera.main.GetComponent<SpaceGravityReal>().G / 100 * 2;

        
    }

    private void ToggleMapMode(ToggleMapMessage m)
    {
        mapMode = m.mapMode;
    }

    // Update is called once per frame
    void Update()
    {
        verts = new Vector3[vertsCount];//update number of vertexes

        if (model != null)
        {
            m2Pos = model.reference.Model.position; //position of reference object
            m2Vel = model.reference.Model.velocity; //velocity of reference object
            m2 = model.reference.Model.mass;
            SOI = model.reference.Model.SOI;

            distance = model.position - m2Pos;

            if (distance.magnitude != 0)
            {
                DrawTraject(m2Pos);
            }
        }
    }

    public void DrawTraject(Vector3 m2Pos)
    {
        float increment = 2 * Mathf.PI / (vertsCount - 1);

        OrbitalInfo orbit = new OrbitalInfo(model);

        //print("radius: " + distance.magnitude + " Es: " + eVect.magnitude);
        //print("Radial start: " + CartToAngle(distance));

        for (int i = 0; i < vertsCount; i++)
        {
            float rad = Ellipse(orbit.SemiMajorAxis, orbit.EccMag, i * increment + CartToAngle(distance));

            verts[i] = PolarToCartesian(new Vector2(rad, i * increment + CartToAngle(distance) + CartToAngle(orbit.Ecc) + Mathf.PI)) + m2Pos;
        }

        var line = gameObject.GetComponent<LineRenderer>();
        line.SetVertexCount(vertsCount);
        if (mapMode)
        {
            line.SetWidth(width * mainCam.orthographicSize * .02f, width * mainCam.orthographicSize * .02f);
        }
        else
            line.SetWidth(width * mapCam.orthographicSize * .02f, width * mapCam.orthographicSize * .02f);

        line.SetColors(c1, c1);

        line.SetPositions(verts);
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

    

    

    public float Ellipse(float a, float e, float degree)
    {
        return (a * (1 - Mathf.Pow(e, 2))) / (1 + e * Mathf.Cos(degree));
    }
}
