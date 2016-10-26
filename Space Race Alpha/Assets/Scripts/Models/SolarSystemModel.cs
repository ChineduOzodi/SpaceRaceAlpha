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
    public Vector3d localReferenceVel = Vector3d.zero;
    public Vector3d localReferenceForce = Vector3d.zero;
    public ModelRef<CraftModel> controlModel;

    //Map View References
    public ModelRef<SolarBodyModel> mapViewReference = new ModelRef<SolarBodyModel>();

    //Debug Options
    public bool showForce = false;
}
