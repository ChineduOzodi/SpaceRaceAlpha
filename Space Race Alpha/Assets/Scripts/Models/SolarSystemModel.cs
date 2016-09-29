using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarSystemModel : Model
{

    //Solarsystem Centerobject
    public ModelRef<SolarBodyModel> centerObject;

    //allSolarsystemMajorBodies
    public ModelRefs<SolarBodyModel> allSolarBodies;

    //Crafts
    public ModelRefs<CraftModel> allCrafts;

    //Debug Options
    public bool showForce = false;
}
