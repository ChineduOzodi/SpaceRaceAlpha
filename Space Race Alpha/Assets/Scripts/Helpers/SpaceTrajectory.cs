using UnityEngine;
using System.Collections;
using System;
using CodeControl;

public class SpaceTrajectory : MonoBehaviour
{
    public double G = 50;

    public Color c1 = Color.red;//colors for orbit
    public Color c2 = Color.yellow;
    internal float width = .02f;
    public int vertsCount = 200;

    internal BaseModel model;

    internal double SOI;
    internal Vector3[] verts;
    internal Vector3d distance;
    internal Vector3d m2Pos;
    internal Vector3d m2Vel;
    internal double m2;

    //MapMode variables
    bool mapMode = false;

    double distanceModifier;

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

        //Get Relevant information
        distanceModifier = mainCam.GetComponent<CameraController>().distanceModifier;

        verts = new Vector3[vertsCount];
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
            m2Pos = Vector3d.zero; //model.reference.Model.position; //position of reference object
            m2Vel = Vector3d.zero;  //model.reference.Model.velocity; //velocity of reference object
            m2 = model.referenceBody.Model.mass;
            SOI = model.referenceBody.Model.SOI;

            distance = model.LocalPosition / distanceModifier;

            Vector3d PositionFromCenter = model.reference.Model.position - model.sol.Model.mapViewReference.Model.position;

            if (distance.magnitude != 0 && model.state != ObjectState.Landed)
            {
                DrawTraject(PositionFromCenter);
            }
            else gameObject.GetComponent<LineRenderer>().SetVertexCount(0);

        }
    }

    public void DrawTraject(Vector3d positionFromCenter)
    {
        double increment = 2 * Mathd.PI / (vertsCount - 1);

        //print("radius: " + distance.magnitude + " Es: " + eVect.magnitude);
        //print("Radial start: " + CartToAngle(distance));

        for (int i = 0; i < vertsCount; i++)
        {
            double rad = Ellipse(model.SemiMajorAxis / distanceModifier, model.Ecc.magnitude, i * increment + new Polar2(distance).angle); //Figures out the radius of the next angle step in trajectory

            Vector2 disp = (Vector2)(Polar2.PolarToCartesian(new Polar2((float)rad, i * (float)increment + (float)new Polar2(distance).angle + (float)new Polar2(model.Ecc).angle + Mathf.PI)) - (Vector2d) positionFromCenter);

            verts[i] = new Vector3(-disp.x,-disp.y) + (Vector3) m2Pos;
        }

        var line = gameObject.GetComponent<LineRenderer>();
        line.SetVertexCount(vertsCount);
        if (mapMode)
        {
            line.SetWidth(Mathf.Pow(width * mainCam.orthographicSize, .8f), Mathf.Pow(width * mainCam.orthographicSize, .8f));
        }
        else
            line.SetWidth(Mathf.Pow(width * mapCam.orthographicSize, .8f), Mathf.Pow(width * mapCam.orthographicSize, .8f));

        //line.SetColors(c1, c1);

        line.SetPositions(verts);
    }

    

    public Vector2d CartesianToPolar(Vector3d point)
    {
        Vector2d polar;

        //calc longitude
        polar.y = Mathd.Atan2(point.x, point.z);

        //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
        var xzLen = new Vector2d(point.x, point.z).magnitude;
        //atan2 does the magic
        polar.x = Mathd.Atan2(-point.y, xzLen);

        //convert to deg
        polar *= Mathd.Rad2Deg;

        return polar;
    }    

    

    public double Ellipse(double a, double e, double degree)
    {
        return (a * (1 - Mathd.Pow(e, 2))) / (1 + e * Mathd.Cos(degree));
    }
}
