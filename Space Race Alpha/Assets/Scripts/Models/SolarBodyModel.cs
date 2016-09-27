using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarBodyModel : BaseModel {

    public BodyType bodyType = BodyType.SolarBody;

    public float SOI;
    public float radius;

    public ModelRefs<SolarBodyModel> solarBodies = new ModelRefs<SolarBodyModel>();

}
