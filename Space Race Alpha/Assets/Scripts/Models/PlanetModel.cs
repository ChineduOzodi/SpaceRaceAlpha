using UnityEngine;
using System.Collections;
using CodeControl;

public class PlanetModel :  Model{

    public string name;
    public ObjectType type;
    public bool init = false;
    internal bool listsUpdated =  false;

    public Vector3 position; //global position in world settings "Should be zero for planet"
    public Quaternion rotation; //planet rotation speed
    public Vector3 localScale; // scale of planet


    public float mass;
    public Vector3 force;
    public Vector3 velocity;

    public float SOI;

    public ModelRefs<SunModel> suns = new ModelRefs<SunModel>();
    public ModelRefs<PlanetModel> planets = new ModelRefs<PlanetModel>();

    //DebugOptions
    public bool showForce = false;

}
