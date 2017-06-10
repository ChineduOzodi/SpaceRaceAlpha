using UnityEngine;
using System.Collections;
using CodeControl;

public class PlanetModel :  SolarBodyModel{

    
    public bool init = false;
    internal bool listsUpdated =  false;

    //DebugOptions
    public bool showForce = false;

    //------------------Constructors-------------//

    public PlanetModel() { }

    public PlanetModel(SolarSystemModel sol, SolarBodyModel reference, Vector3d position, double radius, double density, string name):
        base(sol, reference, position, radius, density, name)
    {
        Type = ObjectType.Planet;
    }



}
