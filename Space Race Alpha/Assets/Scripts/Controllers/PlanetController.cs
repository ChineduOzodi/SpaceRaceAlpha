using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class PlanetController : Controller<PlanetModel> {

    Transform rect;
    internal Rigidbody2D rb2D;

    private void Awake()
    {
        //Message.AddListener<AddPlanetMessage>(OnAddPlanetMessage);
        Message.AddListener<ShowForceMessage>(OnShowForceMessage);

        rb2D = GetComponent<Rigidbody2D>();
        rect = transform; //GetComponent<RectTransform>();
        


    }

    private void OnShowForceMessage(ShowForceMessage m)
    {
        Message.RemoveListener<ShowForceMessage>(OnShowForceMessage);

        ForceArrowModel arrow = new ForceArrowModel();
        arrow.color = m.color;
        arrow.parent = new ModelRef<PlanetModel>(model); ;     
        arrow.force = model.force;

        model.showForce = true;

        Controller.Instantiate<ForceController>("forceArrow", arrow);

        

    }

    protected override void OnInitialize()
    {
        //setup initial location and rotation
        rect.position = model.position;
        rect.rotation = model.rotation;
        rect.localScale = model.localScale;

        MakeCircle(8);

        //rb2D.mass = model.mass;
        //rb2D.velocity = model.velocity;
    }

    protected override void OnModelChanged()
    {
        //update orgital parameters
        rect.position = model.position;
        //rect.rotation = model.rotation;
        rect.localScale = model.localScale;


        //rb2D.mass = model.mass;
        //rb2D.velocity = model.velocity;
    }

    void Update()
    {
        //model.position = rect.position;
        //model.rotation = rect.rotation;
        //model.localScale = rect.localScale;
        //model.mass = rb2D.mass;

        //model.NotifyChange();
    }

    //private void OnPlanetAddedMessage(PlanetAddedMessage m)
    //{
    //    AddSolarBody(m.planet);
    //    model.listsUpdated = true;
    //}

    //private void OnAddPlanetMessage(AddPlanetMessage m)
    //{
    //    if (m.planet != model)
    //    {
    //        AddSolarBody(m.planet);
    //        PlanetAddedMessage returnM = new PlanetAddedMessage();

    //        returnM.planet = model;
    //        Message.Send(returnM);
    //    }
        

    //}

    private void ListUpdate() {
        model.listsUpdated = false;

        //AddPlanetMessage m = new AddPlanetMessage();
        //    m.planet = model;
        //    Message.Send(m);

        //    Message.AddListener<PlanetAddedMessage>(OnPlanetAddedMessage);
    }

    private void AddSolarBody(SolarBodyModel p)
    {
        if (p.bodyType == BodyType.SolarBody)
            model.solarBodies.Add(p);
    }

    public void MakeCircle(int numOfPoints)
    {
        float angleStep = 360.0f / (float)numOfPoints;
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, -angleStep);
        // Make first triangle.
        vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));  // 1. Circle center.
        vertexList.Add(new Vector3(0.0f, 0.5f, 0.0f));  // 2. First vertex on circle outline (radius = 0.5f)
        vertexList.Add(quaternion * vertexList[1]);     // 3. First vertex on circle outline rotated by angle)
                                                        // Add triangle indices.
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(2);
        for (int i = 0; i < numOfPoints - 1; i++)
        {
            triangleList.Add(0);                      // Index of circle center.
            triangleList.Add(vertexList.Count - 1);
            triangleList.Add(vertexList.Count);
            vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.name = "Circle";

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
