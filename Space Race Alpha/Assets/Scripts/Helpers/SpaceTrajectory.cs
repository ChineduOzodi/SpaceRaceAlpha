using UnityEngine;
using System.Collections;
using System;
using CodeControl;

public class SpaceTrajectory : MonoBehaviour
{
    public double G = 50;
    public float alphaMod = 1;
    internal float width = .02f;
    public int vertsCount = 200;

    internal BaseModel model;

    internal double SOI;
    internal Vector3[] verts;
    internal Vector3d distance;
    internal Vector3d m2Pos;
    internal Vector3d m2Vel;
    internal double m2;

    double distanceModifier;

    Camera mainCam;
    CameraController cam;

    // Use this for initialization
    void Start()
    {
        //Add listeners

        //Set Cameras
        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraController>();

        

        verts = new Vector3[vertsCount];
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

            //Get Relevant information
            distanceModifier = cam.distanceModifier;

            distance = model.LocalPosition / distanceModifier;

            Vector3d PositionFromCenter = (model.reference.Model.SystemPosition - cam.reference.SystemPosition) / (distanceModifier); 

            if (distance.magnitude != 0 && model.State != ObjectState.Landed)
            {
                DrawTraject(PositionFromCenter);
            }
            else gameObject.GetComponent<LineRenderer>().positionCount = 0;

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

            Vector2 disp = (Vector2)(Polar2.PolarToCartesian(new Polar2((float)rad, i * (float)increment + (float)new Polar2(distance).angle + (float)new Polar2(model.Ecc).angle + Mathf.PI)) - (Vector2d)positionFromCenter);

            verts[i] = new Vector3(-disp.x, -disp.y) + (Vector3)m2Pos;
        }
        var line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = vertsCount;
        line.widthMultiplier = Mathf.Pow(width * mainCam.orthographicSize, .8f);
        Color color = new Color(1, 1, 1, line.widthMultiplier / (float) (line.widthMultiplier + Math.Pow(model.radius / Units.Mm, alphaMod)));
        line.startColor = color;
        line.endColor = color;

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
