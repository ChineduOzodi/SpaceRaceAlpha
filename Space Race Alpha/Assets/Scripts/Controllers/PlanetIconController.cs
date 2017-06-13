using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

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
    SpriteRenderer sprite;
    CircleCollider2D col;

    internal bool nearReference = false;

    GameObject SOI;
    GameObject terrain;
    SpaceTrajectory spaceT;
    LineRenderer line;
    

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
        col = GetComponent<CircleCollider2D>();
        col.radius = (float)(model.radius / distanceModifier);

        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();

        //Set Icon Size
        iconSize = (float)(model.radius / (zoomMod * Units.Mm * 10));

        spaceT = gameObject.AddComponent<SpaceTrajectory>();
        spaceT.model = model;

        //SOI Initiate
        SOI = Instantiate(Resources.Load("SOI")) as GameObject;
        SOI.transform.localScale = Vector3.one * (float)(model.SOI / zoomMod);
        SOI.transform.position = transform.position;
        SOI.transform.rotation = Quaternion.identity;

        //Set Icon View Mode
        SetIconMode(cam.cameraView, cam.distanceModifier, cam.reference);

    }

    private void CameraViewChanged(SetCameraView m)
    {
        SetIconMode(m.cameraView, m.distanceModifier, m.reference);
    }

    private void SetIconMode(CameraView cameraView, double distanceModifier, SolarBodyModel refer)
    {
        this.distanceModifier = distanceModifier;

        if (cameraView == CameraView.System)
        {
            dynamicSize = true;
            nearReference = false;

            line.enabled = true;
            spaceT.enabled = true;

            sprite.enabled = true;
            sprite.sprite = defaultSprite;
            sprite.color = model.color;

            col.radius = 1;

            SOI.SetActive(true);
            SOI.transform.localScale = Vector3.one * (float)(model.SOI / distanceModifier);

            transform.localScale = Vector3.one * (Mathf.Pow(iconSize * mainCam.orthographicSize, .8f));

            DeleteTerrain();
        }
        else if (cameraView == CameraView.Planet)
        {
            sprite.enabled = false;
            SOI.SetActive(false);

            if (refer.name == model.name || refer.name == model.reference.Model.name)
            {
                sprite.enabled = true;

                if (refer.name == model.name)
                {
                    line.enabled = false;
                    spaceT.enabled = false;
                }

                nearReference = true;
                transform.localScale = Vector2.one;
                MakeTerrain(5000, (float)model.radius);
            }
            else
            {
                line.enabled = false;
                spaceT.enabled = false;
            }
            
        }
        else if (cameraView == CameraView.Surface)
        {
            sprite.enabled = false;
            SOI.SetActive(false);
            nearReference = false;
            line.enabled = false;
            spaceT.enabled = false;

            DeleteTerrain();

        }
    }

    private void DeleteTerrain()
    {
        if (terrain != null)
        {
            Destroy(terrain);
        }
    }

    // Update is called once per frame
    void Update () {
        
        if(cam.cameraView == CameraView.System)
        {
            transform.position = (Vector3)((model.SystemPosition - cam.reference.SystemPosition) / distanceModifier);

            if (dynamicSize)
            {
                transform.localScale = Vector3.one * Mathf.Pow(iconSize * mainCam.orthographicSize * zoomMod, .7f);
                SOI.transform.position = transform.position;                
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
        }
        else if (cam.cameraView == CameraView.Planet)
        {
            if (nearReference)
            {
                transform.localScale = Vector3.one * Mathf.Pow(iconSize * mainCam.orthographicSize * zoomMod * 10, .5f);
                transform.position = (Vector3)((model.SystemPosition - cam.reference.SystemPosition) / distanceModifier);
                transform.eulerAngles = new Vector3(0, 0, (float)(model.Rotation * Mathd.Rad2Deg));
                //terrain resize
                terrain.transform.position = transform.position;
                terrain.transform.rotation = transform.rotation;
            }
        }

        
        
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
        vertexList.Add(polarPosition.cartesian / distanceModifier);  // 2. First vertex on circle outline (radius = 0.5f)
        vertexColor.Add(model.color);

        polarRadius.angle += angleStep;
        polarPosition = new Polar2(polarRadius.radius + FresNoise.GetTerrian(model.name, polarRadius), polarRadius.angle);

        vertexList.Add(polarPosition.cartesian / distanceModifier);     // 3. First vertex on circle outline rotated by angle)
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

            vertexList.Add(polarPosition.cartesian / distanceModifier);
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

        terrain = new GameObject(name + " Terrian"); //Creaate Terrian Object
        MeshRenderer rend = terrain.AddComponent<MeshRenderer>();
        MeshFilter meshF = terrain.AddComponent<MeshFilter>();
        rend.material = planetIconMaterial;
        terrain.layer = 11;
        terrain.transform.position = transform.position;

        meshF.mesh = mesh;
    }

    public void OnMouseEnter()
    {
        LabelCanvas.instance.SetLabel(gameObject, model.name);
    }
    public void OnMouseExit()
    {
        LabelCanvas.instance.CancelLabel();
    }
}
