using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarBodyModel : BaseModel {

    public BodyType bodyType = BodyType.SolarBody;


    public double SOI;
    
    /// <summary>
    /// atm pressure at altitude 0
    /// </summary>
    public double p0 = 0;
    /// <summary>
    /// scale heigh used for atm pressure 
    /// </summary>
    public double atmScaleHeight = 5600;

    public Color color;

    public ModelRefs<SolarBodyModel> solarBodies = new ModelRefs<SolarBodyModel>();
    public ModelRefs<CraftModel> crafts = new ModelRefs<CraftModel>();

    /// <summary>
    /// velocity of rotation of body
    /// </summary>
    public new Vector3d SurfaceVel
    {
        get { return new Vector3d(radius * -reference.Model.RotationRate, 0, 0); }

    }

    //----------------------Private Variales-------------------------//
}
