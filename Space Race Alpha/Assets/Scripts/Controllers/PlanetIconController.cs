using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class PlanetIconController : Controller<PlanetModel> {

    public Sprite defaultSprite;
    public Material planetIconMaterial;

    bool mapMode = false;
    internal bool dynamicSize = false;

    internal float width = 1;

    //Model reference
    public PlanetModel Model;

    Camera mainCam;
    Camera mapCam;
    internal bool isReference = false;

    GameObject SOI;

    protected override void OnInitialize()
    {
        //Add listeners
        Message.AddListener<ToggleMapMessage>(ToggleMapMode);

        //Set Model
        Model = model;
        name = model.name + " Icon";

        //Set Collider
        GetComponent<CircleCollider2D>().radius = (float) (model.radius / Units.km);

        //Set Cameras
        mainCam = Camera.main;
        mapCam = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();

        transform.position = (Vector3) (model.position / Units.km);

        gameObject.AddComponent<SpaceTrajectory>().model = model;

        //SOI Initiate
        SOI = Instantiate(Resources.Load("SOI"), transform) as GameObject;
        SOI.transform.localScale = Vector3.one * (float)model.SOI / Units.km;
        SOI.transform.localPosition = Vector3.zero;
        SOI.transform.rotation = Quaternion.identity;

        if (dynamicSize)
        {
            GetComponent<SpriteRenderer>().sprite = defaultSprite;

            if (mapMode)
            {
                transform.localScale = Vector3.one * width * mainCam.orthographicSize;
            }
            else
                transform.localScale = Vector3.one * width * mapCam.orthographicSize;
        }
        else
        {
            //transform.localScale = Vector3.one * (float)(model.radius / Units.km * 2);
            GetComponent<SpriteRenderer>().sprite = null;
            MakeTerrain(1000, (float)model.radius);
        }


    }

    private void ToggleMapMode(ToggleMapMessage m)
    {
        mapMode = m.mapMode;
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!isReference)
        {
            transform.position = (Vector3)(model.LocalPosition / Units.km);

            if (dynamicSize)
            {
                
                if (mapMode)
                {
                    transform.localScale = Vector3.one * width * mainCam.orthographicSize;
                }
                else
                    transform.localScale = Vector3.one * width * mapCam.orthographicSize;
            }
            else
            {
                transform.localScale = Vector3.one * (float)(model.radius / Units.km * 2);
            }
        }
            
        else
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        transform.localRotation = model.rotation;
        
        

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
    public void SetReference(bool val)
    {
        isReference = val;
        
        //initiate orbital trajectory
        if (!isReference)
        {
            
            
        }
        else
        {
            

        }
    }
}
