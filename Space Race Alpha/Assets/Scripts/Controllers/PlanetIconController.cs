using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class PlanetIconController : Controller<PlanetModel> {

    public Sprite defaultSprite;
    public Material planetIconMaterial;

    double distanceModifier;
    internal bool dynamicSize = true;
    private float zoomMod = .5f;

    /// <summary>
    /// Used to scale for bigger planet icons if needed
    /// </summary>
    internal float iconSize = 1;

    //Model reference
    internal PlanetModel Model;

    Camera mainCam;
    CameraController cam;

    internal bool isReference = false;

    GameObject SOI;

    protected override void OnInitialize()
    {
        //Add listeners
        Message.AddListener<SetCameraView>(CameraViewChanged);

        //Set Model
        Model = model;
        name = model.name + " Icon";

        //Set Cameras
        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraController>();
        //Get Relevant information
        distanceModifier = mainCam.GetComponent<CameraController>().distanceModifier;

        //Set Collider
        GetComponent<CircleCollider2D>().radius = (float) (model.radius / distanceModifier);

        //Set Icon Size
        iconSize = (float)(model.radius / distanceModifier);

        gameObject.AddComponent<SpaceTrajectory>().model = model;

        //SOI Initiate
        SOI = Instantiate(Resources.Load("SOI")) as GameObject;
        SOI.transform.localScale = Vector3.one * (float)(model.SOI / distanceModifier * zoomMod);
        SOI.transform.position = transform.position;
        SOI.transform.rotation = Quaternion.identity;

        //Set Icon View Mode
        SetIconMode(cam.cameraView, cam.distanceModifier);

    }

    private void CameraViewChanged(SetCameraView m)
    {
        SetIconMode(m.cameraView, m.distanceModifier);
    }

    private void SetIconMode(CameraView cameraView, double distanceModifier)
    {
        this.distanceModifier = distanceModifier;
        if (cameraView == CameraView.System)
        {
            dynamicSize = true;
            GetComponent<SpriteRenderer>().sprite = defaultSprite;
            GetComponent<SpriteRenderer>().color = model.color;
            GetComponent<CircleCollider2D>().radius = 1;
            SOI.transform.localScale = Vector3.one * (float)(model.SOI / distanceModifier);

            transform.localScale = Vector3.one * (Mathf.Pow(iconSize * mainCam.orthographicSize, .8f));
        }
        else if (cameraView == CameraView.Planet)
        {
            if (isReference)
            {
                transform.localScale = Vector2.one;
                GetComponent<SpriteRenderer>().sprite = null;
                MakeTerrain(1000, (float)model.radius);
            }
            
        }
    }
	
	// Update is called once per frame
	void Update () {

        transform.position = (Vector3)((model.SystemPosition - cam.reference.SystemPosition) / distanceModifier);

        if (dynamicSize)
        {
            transform.localScale = Vector3.one * Mathf.Pow(iconSize * mainCam.orthographicSize * zoomMod, .8f);
            SOI.transform.position = transform.position;
        }
        else
        {
            transform.localScale = Vector3.one;
        }

        transform.eulerAngles = new Vector3(0,0,(float)(model.Rotation * Mathd.Rad2Deg));
    }

    /// <summary>
    /// Create test Terrian, could be used to create asteroids in future
    /// </summary>
    /// <param name="numOfPoints"> number of vertices in mesh</param>
    /// <param name="radius"> radius of mesh</param>
    public void MakeTerrain(int numOfPoints, float radius)
    {

        float angleStep = -Mathf.PI * 2 / numOfPoints;
        Polar2 polarRadius = new Polar2(radius, 0);

        Polar2 polarPosition = new Polar2(polarRadius.radius + FresNoise.GetTerrian(model.name, polarRadius), polarRadius.angle);

        List<Vector3d> vertexList = new List<Vector3d>();
        List<int> triangleList = new List<int>();
        List<Color> vertexColor = new List<Color>();

        // Make first triangle.
        vertexList.Add(new Vector3d(0.0f, 0.0f, 0.0f));  // 1. Circle center.
        vertexColor.Add(Color.black);
        vertexList.Add(polarPosition.cartesian / Units.km);  // 2. First vertex on circle outline (radius = 0.5f)
        vertexColor.Add(model.color);

        polarRadius.angle += angleStep;
        polarPosition = new Polar2(polarRadius.radius + FresNoise.GetTerrian(model.name, polarRadius), polarRadius.angle);

        vertexList.Add(polarPosition.cartesian / Units.km);     // 3. First vertex on circle outline rotated by angle)
        vertexColor.Add(model.color);

        // Add triangle indices.
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(2);
        for (int i = 0; i < numOfPoints - 1; i++)
        {
            triangleList.Add(0);                      // Index of circle center.
            triangleList.Add(vertexList.Count - 1);
            triangleList.Add(vertexList.Count);

            polarRadius.angle += angleStep;
            polarPosition = new Polar2(polarRadius.radius + FresNoise.GetTerrian(model.name, polarRadius), polarRadius.angle);

            vertexList.Add(polarPosition.cartesian / Units.km);
            vertexColor.Add(model.color);
        }
        Mesh mesh = new Mesh();
        Vector3[] verts = new Vector3[vertexList.Count]; //Convert list to array
        for (int i = 0; i < vertexList.Count; i++)
        {
            verts[i] = (Vector3)vertexList[i];
        }
        mesh.vertices = verts;
        mesh.triangles = triangleList.ToArray();
        mesh.colors = vertexColor.ToArray();
        mesh.name = "Circle";

        GameObject obj = new GameObject(name + " Terrian"); //Creaate Terrian Object
        MeshRenderer rend = obj.AddComponent<MeshRenderer>();
        MeshFilter meshF = obj.AddComponent<MeshFilter>();
        rend.material = planetIconMaterial;
        obj.layer = 11;
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;

        meshF.mesh = mesh;
    }
}
