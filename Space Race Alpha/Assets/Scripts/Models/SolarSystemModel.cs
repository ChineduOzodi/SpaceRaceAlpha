using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarSystemModel : Model
{

    //Solarsystem Centerobject
    public ModelRef<SolarBodyModel> centerObject;

    //allSolarsystemMajorBodies
    public ModelRefs<SolarBodyModel> allSolarBodies;

    //Debug Options
    public bool showForce = false;
}
