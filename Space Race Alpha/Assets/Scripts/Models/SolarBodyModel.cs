using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarBodyModel : BaseModel {


    //------------Public Variables-----------------------//
    public BodyType bodyType = BodyType.SolarBody;
    /// <summary>
    /// in m
    /// </summary>
    public double SOI;
    /// <summary>
    /// atm pressure at altitude 0
    /// </summary>
    public double p0 = 0;
    /// <summary>
    /// scale heigh used for atm pressure 
    /// </summary>
    public double atmScaleHeight = 5600;
    /// <summary>
    /// color to use in map view
    /// </summary>
    public Color color;
    /// <summary>
    /// solar bodies within this solar body
    /// </summary>
    public ModelRefs<SolarBodyModel> solarBodies = new ModelRefs<SolarBodyModel>();
    public ModelRefs<CraftModel> crafts = new ModelRefs<CraftModel>();

    //----------------------Private Variales-------------------------//

    //------------Constructors-----------------------//

    public SolarBodyModel() { }

    /// <summary>
    /// Add Solar Body to Solar System model. Reference should already be
    /// </summary>
    /// <param name="sol">Solar system to add model to</param>
    /// <param name="reference"> The reference solar body, usually the sun or planet solar object orbits</param>
    /// <param name="position"> world position of model</param>
    /// <param name="radius"> radius of solar body</param>
    /// <param name="density"> density of solar body .1 to 10 ideal</param>
    /// <param name="surfacePressure"> surface pressure of body in atm</param>
    /// <param name="name"> name of solar body</param>
    public SolarBodyModel(SolarSystemModel sol, SolarBodyModel reference, Vector3d position, double radius, double density, string name)
    {
        if (reference == null)
            reference = this;
        this.reference = new ModelRef<SolarBodyModel>(reference);
        this.sol = new ModelRef<SolarSystemModel>(sol);
        this.reference.Model.solarBodies.Add(this);
        this.SystemPosition = position;                                     //set given info
        this.mass = density * Forces.SphereVolume(radius);
        this.radius = radius;
        this.density = density;
        this.name = name;
        System.Random seed = new System.Random(name.GetHashCode());
        this.RotationRate = 2 * Mathd.PI / ((double)seed.Next(10, 50) * Date.Hour);
        this.color = new Color(seed.Next(0, 100) / 100f, seed.Next(0, 100) / 100f, seed.Next(0, 100) / 100f);

        if (sol.centerObject.Model == null)                                //set first center of mass object if not alreaady set
        {
            sol.centerObject = new ModelRef<SolarBodyModel>(this);
        }
        else if (this.mass > sol.centerObject.Model.mass)
        {                                                            //If new object created bigger, change center of sol
            sol.centerObject.Delete();
            sol.centerObject = new ModelRef<SolarBodyModel>(this);

            foreach (SolarBodyModel m in sol.allSolarBodies)
            {
                m.CalculateSOI();
            }
        }
        sol.allSolarBodies.Add(this);
        CalculateSOI();
    }
    //-----------------------Funcitons------------------------------------//

    /// <summary>
    /// Add a craft to a solar body
    /// </summary>
    /// <param name="model">CraftModel</param>
    /// <param name="angle"> global? angle on the solar object in RADIANS</param>
    /// <param name="name">name of craft</param>
    /// <returns></returns>
    public CraftModel AddCraft(CraftModel model, double angle)
    {
        //var body = CraftModel.LiquidFuelContainer; //basic craft info
        //body.AddCraftModal(CraftModel.SpaceEngine, new Vector3(0, -1), 180 * Mathd.Deg2Rad);

        model.reference = new ModelRef<SolarBodyModel>(this);
        model.sol = new ModelRef<SolarSystemModel>(sol.Model);
        model.State = ObjectState.Landed;

        crafts.Add(model);
        sol.Model.allCrafts.Add(model);

        //craft position rotation info

        Polar2 polarRadius = new Polar2(radius, angle);

        double displacement = FresNoise.GetTerrian(name, polarRadius);
        Polar2 polarPosition = new Polar2(radius + displacement - model.LocalPosition.y, angle);

        model.polar = polarPosition; //Set surface position
        model.SurfaceVel = Vector3d.zero;// new Vector3d(-(planet.radius + displacement) * planet.RotationRate, 0, 0); //set surface velocity

        model.Rotation = angle - .5 * Mathd.PI;
        //body.rotation.eulerAngles = new Vector3(0, 0, (double) angle * Mathd.Rad2Deg + planet.rotation.eulerAngles.z);

        return model;
    }
    /// <summary>
    /// Add Craft in orbit around reference object
    /// </summary>
    /// <param name="model">The Craft Model</param>
    /// <param name="reference"> the reference object</param>
    /// <param name="localPosition"> position above the centeor of reference object</param>
    /// <param name="name">name of craft</param>
    /// <returns></returns>
    public CraftModel AddCraft(CraftModel model, Polar2 pol)
    {


        model.reference = new ModelRef<SolarBodyModel>(this);
        model.sol = new ModelRef<SolarSystemModel>(sol.Model);
        model.State = ObjectState.Orbit;

        sol.Model.allCrafts.Add(model);
        crafts.Add(model);

        model.LocalPosition = pol.cartesian;
        model.LocalRotation = 0;
        model.velocity = SolarSystemModel.VelocityFromOrbit(model);

        return model;
    }

    //------------------------Helper Functions----------------------------//

    /// <summary>
    /// Calculate Sphere of Influence based on centor solar body distance and mass
    /// </summary>
    /// <param name="m"> solar body to calculate SOI for</param>
    /// <returns>SOI radial distance</returns>
    public void CalculateSOI()
    {
        double r = Vector3d.Distance(SystemPosition, reference.Model.SystemPosition);
        double rSOI = r * Mathd.Pow(mass / reference.Model.mass, 0.4f);
        SOI =  rSOI;
    }
}
