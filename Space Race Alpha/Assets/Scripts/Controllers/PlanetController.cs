using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class PlanetController : Controller<PlanetModel> {

    public Material planetMaterial;
    Transform rect;
    internal Rigidbody2D rb2D;
    //internal float[] LOD1 = { 1, 45, 10 }; //level of detail for making the planet mesh
    internal List<int> createdMeshes = new List<int>();
    internal List<GameObject> planetMeshObjs;

    internal bool initial = true;
    internal int referenceID;

    double circumference;
    double meshAngleStep;

    //Model reference
    public PlanetModel Model;

    private void Awake()
    {

        planetMeshObjs = new List<GameObject>();

        rect = transform; //GetComponent<RectTransform>();

    }

    protected override void OnInitialize()
    {
        //setup initial location and rotation
        rect.position = Vector3.zero;
        rect.rotation = Quaternion.identity;

        circumference = model.radius * Mathf.PI * .002f; //circumference in km
        meshAngleStep = 2 * Mathf.PI / circumference; //angle distance for each km in radians

        //MakeTerrain(65000,model.radius);

        //CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        //col.radius = model.radius;

        //Set Model
        Model = model;
        name = model.name;

        SetMeshes();

        model.sol.Model.controlModel.Model.NotifyChange();


    }

    protected override void OnModelChanged()
    {
        //rect.rotation = model.rotation; //set rotation;
    }

    void Update()
    {

        //SetMeshes(); //Needs a lot more work

        UpdateReferencePointData();

        
    }

    private void UpdateReferencePointData()
    {
        double worldAngleDisp = model.rotation.eulerAngles.z * Mathd.Deg2Rad;

        Polar2 localReferencePoint = new Polar2(model.radius, (referenceID + .5) * meshAngleStep + worldAngleDisp); //the radius and angle of mesh Id in relationship with planet Object

        model.sol.Model.localReferencePoint = localReferencePoint.cartesian;
        model.sol.Model.localReferencePointRotation.eulerAngles = new Vector3(0, 0, (float)(localReferencePoint.angle * Mathd.Rad2Deg - 90));
    }

    private void SetMeshes()
    {
        List<int> idealMeshes = CalculateIdealMesh();
        //List<int[]> meshesToAdd = new List<int[]>();
        //List<int[]> meshesToDelete = new List<int[]>();
        List<int> meshesThatExist = new List<int>();

        for (int i = 0; i < idealMeshes.Count; i++)
        {
            for (int b = 0; b < createdMeshes.Count; b++)
            {
                if (createdMeshes[b] == idealMeshes[i])
                {
                    meshesThatExist.Add(idealMeshes[i]);
                }
            }
        }
        foreach (int meshID in meshesThatExist)
        {
            for (int b = 0; b < createdMeshes.Count; b++)
            {
                if (createdMeshes[b] == meshID)
                {
                   createdMeshes.RemoveAt(b);
                }
            }
            for (int b = 0; b < idealMeshes.Count; b++)
            {
                if (idealMeshes[b] == meshID)
                {
                    idealMeshes.RemoveAt(b);
                }
            }
        }

        foreach (int meshID in idealMeshes)
        {
            CreateMeshObj(meshID);
        }
        foreach (int meshID in createdMeshes)
        {
            DeleteMeshObj(meshID);
            
        }
        foreach (int meshID in meshesThatExist)
        {
            idealMeshes.Add(meshID);
        }

        createdMeshes = idealMeshes;

    }

    private void DeleteMeshObj(int meshID)
    {
        foreach (GameObject obj in planetMeshObjs)
        {
            if (obj.name == meshID.ToString())
            {
                planetMeshObjs.Remove(obj);
                Destroy(obj);
                break;
            }
        }
    }

    private void CreateMeshObj(int meshID)
    {
        
        GameObject obj = new GameObject(meshID.ToString());
        MeshRenderer rend = obj.AddComponent<MeshRenderer>();
        MeshFilter meshF  = obj.AddComponent<MeshFilter>();
        obj.layer = 10;
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;

        rend.material = planetMaterial;

        meshF.mesh = MakeMesh(meshID, obj);
        planetMeshObjs.Add(obj);


    }

    private Mesh MakeMesh(int meshID, GameObject obj)
    {
        //Create 2d polygon collider
        EdgeCollider2D poly = obj.AddComponent<EdgeCollider2D>();
        List<Vector2> polyPoints = new List<Vector2>();

        int numVert = 1000;
        
        
        double angleStep = -meshAngleStep / numVert;

        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        List<Color> vertexColor = new List<Color>();

        

        Polar2 polarRadius = new Polar2(model.radius, (meshID + 1) * meshAngleStep); //the radius and angle of mesh Id in relationship with planet Object
        double worldAngleDisp = model.rotation.eulerAngles.z * Mathd.Deg2Rad;

        

        if (initial)
        {
            referenceID = meshID;

            UpdateReferencePointData();

            initial = false;

            UpdateControlModel();
        }

        float displacement = -5000 + FresNoise.GetTerrian(model.name, polarRadius);
        Polar2 polarPosition = new Polar2(polarRadius.radius + displacement, polarRadius.angle - worldAngleDisp);

        // Make first triangle.
        vertexList.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation)); //position in relationship to reference point );  // 1. First point that is -5000 lower
        vertexColor.Add(VertexColor(displacement)); //Add Color
        //polyPoints.Add((Vector2)polarPosition.cartesian); //add collider point

        displacement = FresNoise.GetTerrian(model.name, polarRadius);
        polarPosition = new Polar2(polarRadius.radius + displacement, polarRadius.angle - worldAngleDisp);
        vertexList.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation));  // 1. First vertex on circle outline (radius = 0.5f)
        vertexColor.Add(VertexColor(displacement)); //Add Color
        polyPoints.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation)); //add collider point

        polarRadius.angle += angleStep;

        displacement = FresNoise.GetTerrian(model.name, polarRadius);
        polarPosition = new Polar2(polarRadius.radius + displacement, polarRadius.angle - worldAngleDisp);
        vertexList.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation));     // 3. First vertex on circle outline rotated by angle)
        vertexColor.Add(VertexColor(displacement)); //Add Color
        polyPoints.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation)); //add collider point

        displacement = -5000 + FresNoise.GetTerrian(model.name, polarRadius);
        polarPosition = new Polar2(polarRadius.radius + displacement, polarRadius.angle - worldAngleDisp);
        vertexList.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation));     // 4. vertex on circle outline rotated by angle and down one)
        vertexColor.Add(VertexColor(displacement)); //Add Color

        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(2);

        triangleList.Add(0);
        triangleList.Add(2);
        triangleList.Add(3);

        for (int i = 0; i < numVert - 1; i++)
        {
            triangleList.Add(vertexList.Count - 2);                       
            triangleList.Add(vertexList.Count - 3);
            triangleList.Add(vertexList.Count - 1);

            triangleList.Add(vertexList.Count - 2);
            triangleList.Add(vertexList.Count - 1);
            triangleList.Add(vertexList.Count);

            polarRadius.angle += angleStep;

            displacement = FresNoise.GetTerrian(model.name, polarRadius);
            polarPosition = new Polar2(polarRadius.radius + displacement, polarRadius.angle - worldAngleDisp);
            vertexList.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation));     // 3. First vertex on circle outline rotated by angle)
            vertexColor.Add(VertexColor(displacement)); //Add Color
            polyPoints.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation)); //add collider point

            displacement = -5000 + FresNoise.GetTerrian(model.name, polarRadius);
            polarPosition = new Polar2(polarRadius.radius + displacement, polarRadius.angle - worldAngleDisp);
            vertexList.Add((Vector3)Forces.ReferencePosition(polarPosition.cartesian, model.sol.Model.localReferencePoint, model.sol.Model.localReferencePointRotation));    // 3. vertex on circle outline rotated by angle and down one)
            vertexColor.Add(VertexColor(displacement)); //Add Color
            //if (i + 1 == numVert - 1)
            //{
            //    polyPoints.Add((Vector2)polarPosition.cartesian); //add last collider point
            //}
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
        poly.points = polyPoints.ToArray();
        return mesh;
    }

    private Color VertexColor(float displacement)
    {
        if (displacement < 0)
        {
            return Color.blue;
        }
        else if (displacement < 100)
        {
            return Color.yellow;
        }
        else
            return Color.green;
    }

    private List<int> CalculateIdealMesh()
    {

        List<int> list = new List<int>();
        double circumference = model.radius * Mathd.PI * .002; //circumference in km
        double angleStep = meshAngleStep; //angle distance for each km in radians

        Polar2 targetPolar = model.sol.Model.controlModel.Model.polar;

        if (targetPolar.radius > .01f){ //avoid unspawned cameras

            double angle = targetPolar.angle + model.rotation.eulerAngles.z * Mathd.Deg2Rad;
            angle = (angle < 0) ? angle + 2 * Mathd.PI : angle;
            int meshID = Mathd.FloorToInt(angle / angleStep);

            //Add the three mesh IDs

            if (!list.Contains(meshID))
            {
                list.Add(meshID);
            }

            meshID = (meshID + 1 < circumference) ? meshID + 1 : 0;

            if (!list.Contains(meshID))
            {
                list.Add(meshID);
            }

            meshID = (meshID - 2 < 0) ? Mathd.CeilToInt(circumference) - 1 : meshID - 2;

            if (!list.Contains(meshID))
            {
                list.Add(meshID);
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

    private void UpdateControlModel() {
        model.sol.Model.controlModel.Model.NotifyChange();
    }

    public void MakeCircle(int numOfPoints, float radius)
    {
        float angleStep = 360.0f / (float)numOfPoints;
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, -angleStep);
        // Make first triangle.
        vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));  // 1. Circle center.
        vertexList.Add(new Vector3(radius + FresNoise.GetTerrian(model.name,new Polar2 (radius,0)), 0.0f, 0.0f));  // 2. First vertex on circle outline (radius = 0.5f)
        vertexList.Add(quaternion * new Vector3(radius + FresNoise.GetTerrian(model.name, new Polar2(radius, angleStep)), 0.0f, 0.0f));     // 3. First vertex on circle outline rotated by angle)
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
            vertexList.Add(quaternion.eulerAngles = quaternion * new Vector3(radius + FresNoise.GetTerrian(model.name, new Polar2(radius, quaternion.eulerAngles.z)), 0.0f, 0.0f));
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.name = "Circle";

        GetComponent<MeshFilter>().mesh = mesh;
        //GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    
}
