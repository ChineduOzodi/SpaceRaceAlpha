using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarBodyModel : BaseModel {

    public BodyType bodyType = BodyType.SolarBody;


    public double SOI;
    public double radius;

    public Color color;

    public ModelRefs<SolarBodyModel> solarBodies = new ModelRefs<SolarBodyModel>();
    public ModelRefs<CraftModel> crafts = new ModelRefs<CraftModel>();
}
