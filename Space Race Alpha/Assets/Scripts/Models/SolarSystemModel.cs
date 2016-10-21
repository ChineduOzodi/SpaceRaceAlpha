using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarSystemModel : Model
{

    //Solarsystem Centerobject
    public ModelRef<SolarBodyModel> centerObject = new ModelRef<SolarBodyModel>();

    //allSolarsystemMajorBodies
    public ModelRefs<SolarBodyModel> allSolarBodies = new ModelRefs<SolarBodyModel>();

    //Crafts
    public ModelRefs<CraftModel> allCrafts = new ModelRefs<CraftModel>();

    //References

    public Vector3d localReferencePoint = Vector3d.zero;
    public Quaternion localReferencePointRotation = Quaternion.identity;
    public ModelRef<CraftModel> controlModel;

    //Debug Options
    public bool showForce = false;
}
