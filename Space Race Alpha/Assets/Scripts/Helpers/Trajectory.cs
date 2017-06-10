using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class Trajectory : MonoBehaviour
{
    internal Color c1 = Color.red;
    internal Color c2 = Color.red;
    internal int verts = 100; //Can't be greater than simcount in gravity code
    internal float width = 1;
    internal BaseModel model;
    internal Rigidbody2D rgb;


    Camera mainCam;


    // Use this for initialization
    void Start()
    {
        //Add listeners


        rgb = GetComponent<Rigidbody2D>();
        model = GetComponent<CraftController>().Model;

        //Set Cameras
        mainCam = Camera.main;

    }


    // Update is called once per frame
    void Update()
    {

        DrawTraject();

    }

    public void DrawTraject()
    {
        Vector3[] vectPos = new Vector3[verts];
        vectPos[0] = transform.position;
        Vector3 vel = rgb.velocity;

        for (int i = 1; i < verts; i++)
        {
            vel += (Vector3) Forces.ForceToVelocity(Forces.Rotate(model.force - model.sol.Model.localReferenceForce, model.reference.Model.Rotation), model.mass, Time.deltaTime);
            vectPos[i] = Forces.VelocityToPosition(vectPos[i - 1],vel, Time.deltaTime);
            //vectPos[i] = (Vector3) Porabola(model.force / model.mass, (Vector3d) vel, (Vector3d) transform.position, i * 1 * Time.deltaTime);
        }

        var line = gameObject.GetComponent<LineRenderer>();
        line.SetVertexCount(verts);
        line.SetWidth(width * mainCam.orthographicSize * .02f, width * mainCam.orthographicSize * .02f);
        //line.SetColors(c1, c2);

        line.SetPositions(vectPos);
    }

    private Vector3d Porabola(Vector3d gravity, Vector3d vel, Vector3d  position, double time)
    {
        //Vector3d positionAdjx = Forces.Tangent(position.normalized) * vel.x * time; ;
        //Vector3d positionAdjy = position.normalized * (vel.y * time + .5d * gravity.magnitude * time * time);
        return new Vector3d(position.x + vel.x * time, position.y + (vel.y * time + .5d * -gravity.magnitude * time * time)); //position + positionAdjx + positionAdjy;
    }
}
