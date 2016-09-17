using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarSystemModel : Model
{
    //Center of Mass information
    public Vector3 centerOfMass;
    public float totalMass;
    public Vector3 centerOfMassPotential;

    //Solarsystem Centerobject
    public ModelRef<SolarBodyModel> centerObject;

    //allSolarsystemMajorBodies
    public ModelRefs<SolarBodyModel> allSolarBodies;
}
