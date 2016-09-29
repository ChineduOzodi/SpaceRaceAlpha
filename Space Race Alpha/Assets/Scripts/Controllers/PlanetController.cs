using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class PlanetController : Controller<PlanetModel> {

    Transform rect;
    internal Rigidbody2D rb2D;
    internal float[] LOD1 = { 1, 45, 10 };
    internal List<int[]> createdMeshes = new List<int[]>();
    internal List<GameObject> planetMeshObjs;

    private void Awake()
    {
        //Message.AddListener<AddPlanetMessage>(OnAddPlanetMessage);
        Message.AddListener<ShowForceMessage>(OnShowForceMessage);
        planetMeshObjs = new List<GameObject>();

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

        MakeCircle(1000,model.radius);

        CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = model.radius;

        //rb2D.mass = model.mass;
        //rb2D.velocity = model.velocity;
    }

    protected override void OnModelChanged()
    {
        //update orgital parameters
        rect.position = model.position;
        rect.rotation = model.rotation;
        rect.localScale = model.localScale;


        //rb2D.mass = model.mass;
        //rb2D.velocity = model.velocity;
    }

    void Update()
    {
        //SetMeshes(); //Needs a lot more work

        model.position = rect.position;
        model.rotation = rect.rotation;
        //model.localScale = rect.localScale;
        //model.mass = rb2D.mass;
    }

    private void SetMeshes()
    {
        List<int[]> idealMeshes = CalculateIdealMesh();
        //List<int[]> meshesToAdd = new List<int[]>();
        //List<int[]> meshesToDelete = new List<int[]>();
        List<int[]> meshesThatExist = new List<int[]>();

        for (int i = 0; i < idealMeshes.Count; i++)
        {
            for (int b = 0; b < createdMeshes.Count; b++)
            {
                if (createdMeshes[b][1] == idealMeshes[i][1])
                {
                    meshesThatExist.Add(idealMeshes[i]);
                }
            }
        }
        foreach (int[] meshID in meshesThatExist)
        {
            for (int b = 0; b < createdMeshes.Count; b++)
            {
                if (createdMeshes[b][1] == meshID[1])
                {
                   createdMeshes.RemoveAt(b);
                }
            }
            for (int b = 0; b < idealMeshes.Count; b++)
            {
                if (idealMeshes[b][1] == meshID[1])
                {
                    idealMeshes.RemoveAt(b);
                }
            }
        }

        foreach (int[] meshID in idealMeshes)
        {
            CreateMeshObj(meshID);
        }
        foreach (int[] meshID in createdMeshes)
        {
            DeleteMeshObj(meshID);
            
        }
        foreach (int[] meshID in meshesThatExist)
        {
            idealMeshes.Add(meshID);
        }

        createdMeshes = idealMeshes;

    }

    private void DeleteMeshObj(int[] meshID)
    {
        foreach (GameObject obj in planetMeshObjs)
        {
            if (obj.name == meshID[1].ToString())
            {
                planetMeshObjs.Remove(obj);
                Destroy(obj);
                break;
            }
        }
    }

    private void CreateMeshObj(int[] meshID)
    {
        
        GameObject obj = new GameObject(meshID[1].ToString());
        obj.AddComponent<MeshRenderer>();
        MeshFilter meshF  = obj.AddComponent<MeshFilter>();
        obj.transform.parent = transform;

        meshF.mesh = MakeMesh(meshID, obj);
        Instantiate(obj,transform);
        planetMeshObjs.Add(obj);


    }

    private Mesh MakeMesh(int[] meshID, GameObject obj)
    {
        float angleStep = LOD1[1] / LOD1[2] ;
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, -LOD1[1] * meshID[1]);
        // Make first triangle.
        vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));  // 1. Circle center.

        var radius = new Vector3(model.radius, 0.0f, 0.0f);
        vertexList.Add(quaternion * radius);            // 2. First vertex on circle outline (radius = 0.5f)

        quaternion = Quaternion.Euler(0.0f, 0.0f, -angleStep * (meshID[1]+1));
        vertexList.Add(quaternion * vertexList[1]);     // 3. First vertex on circle outline rotated by angle)
                                                        // Add triangle indices.
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(2);
        for (int i = 0; i < LOD1[2] - 1; i++)
        {
            triangleList.Add(0);                       // Index of circle center.
            triangleList.Add(vertexList.Count - 1);
            triangleList.Add(vertexList.Count);
            quaternion.eulerAngles = new Vector3(0f, 0f, -angleStep * (2 + i));
            vertexList.Add(quaternion.eulerAngles = quaternion * vertexList[1]);
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.name = "Circle";

        return mesh;
    }

    private List<int[]> CalculateIdealMesh()
    {

        List<int[]> list = new List<int[]>();

        foreach ( CraftModel craft in model.crafts)
        {
            if (craft.referenceDistance.sqrMagnitude > .01f){ //avoid unspawned crafts

                float angle = Forces.CartesianToPolar(craft.referenceDistance).y - model.rotation.eulerAngles.z * Mathf.Deg2Rad;
                int count = Mathf.FloorToInt(angle / (LOD1[1] * Mathf.Deg2Rad));

                int[] meshID = { 1, count };

                if (!list.Contains(meshID))
                {
                    list.Add(meshID);
                }

                count = (count + 1 < Mathf.FloorToInt(360 / LOD1[1])) ? count + 1 : 0;

                meshID = new int[] { 1, count };

                if (!list.Contains(meshID))
                {
                    list.Add(meshID);
                }

                count = (count - 2 < 0) ? Mathf.FloorToInt(360 / LOD1[1]) - 1 : count - 2;

                meshID = new int[] { 1, count };

                if (!list.Contains(meshID))
                {
                    list.Add(meshID);
                }
            }
            
        }

        return list;
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

    public void MakeCircle(int numOfPoints, float radius)
    {
        float angleStep = 360.0f / (float)numOfPoints;
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, -angleStep);
        // Make first triangle.
        vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));  // 1. Circle center.
        vertexList.Add(new Vector3(radius, 0.0f, 0.0f));  // 2. First vertex on circle outline (radius = 0.5f)
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
            quaternion.eulerAngles = new Vector3(0f,0f,-angleStep * (2 + i));
            vertexList.Add(quaternion.eulerAngles = quaternion * vertexList[1]);
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.name = "Circle";

        GetComponent<MeshFilter>().mesh = mesh;
        //GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
