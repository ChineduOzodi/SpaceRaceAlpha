using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarBodyModel : BaseModel {

    public BodyType bodyType = BodyType.SolarBody;
    //public ModelRef<SolarSystemModel> system;

    public float SOI;
    public float radius;

    //public ModelRefs<SolarBodyModel> solarBodies = new ModelRefs<SolarBodyModel>();
    public ModelRefs<CraftModel> crafts = new ModelRefs<CraftModel>();
}
