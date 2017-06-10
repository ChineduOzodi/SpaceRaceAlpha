using UnityEngine;
using System.Collections;
using CodeControl;

public class SunModel : PlanetModel {
    
    public double surfaceTemperature;

    

    //------------------Constructors-------------//
    
    public SunModel() { }

    public SunModel(SolarSystemModel sol, SolarBodyModel reference, Vector3d position, double radius, double density, double surfaceTemp, string name):
        base(sol, reference, position, radius, density, name)
    {
        Type = ObjectType.Sun;
        surfaceTemperature = surfaceTemp;
    }

}
