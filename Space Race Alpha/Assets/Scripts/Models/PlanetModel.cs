using UnityEngine;
using System.Collections;
using CodeControl;

public class PlanetModel :  Model{

    public string name;

    public Vector3 position; //global position in world settings "Should be zero for planet"
    public Quaternion rotation; //planet rotation speed
    public Vector3 localScale; // scale of planet


    public float mass;
    public Vector3 force;

    public ModelRefs<SunModel> suns;
    public ModelRefs<PlanetModel> planets;

}
