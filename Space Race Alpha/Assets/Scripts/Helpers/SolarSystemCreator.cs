using UnityEngine;
using System.Collections;
using CodeControl;

/// <summary>
/// Functions used to make creating solar system models easier
/// </summary>
public static class SolarSystemCreator{

    /// <summary>
    /// Add sun to the Solar System, only want to add one per sol
    /// </summary>
    /// <param name="sol">Solar system to add sun to</param>
    /// <param name="radius">Radius of sun</param>
    /// <param name="density">Density of sun, usually between .1 and .5</param>
    /// <param name="name">Name of sun</param>
    /// <returns></returns>
    public static SunModel AddSun(SolarSystemModel sol, double radius, double density, string name)
    {
        //Calculate basic info
        Vector3d position = Vector3d.zero;

        //Create Model
        var body = new SunModel();
        body.type = ObjectType.Sun;
        body.surfaceTemperature = radius * 0.0085f;

        AddSolarBody(sol, body, body, position, radius, density, name); //add to solarsystem

        //Controller.Instantiate<SunIconController>("sunIcon", body);

        return body;
    }
    /// <summary>
    /// Add sun to the Solar System by mass of sun, should only add one
    /// </summary>
    /// <param name="sol">Solar system to add sun to</param>
    /// <param name="radius">Radius of sun</param>
    /// <param name="mass">Mass of sun</param>
    /// <param name="name">Name of sun</param>
    /// <returns></returns>
    public static SunModel AddSunByMass(SolarSystemModel sol,double radius, double mass, string name)
    {

        //Calculate basic info
        Vector3d position = Vector3d.zero;
        double density = mass / Forces.CircleArea(radius);

        //Create Model
        var body = new SunModel();
        body.type = ObjectType.Sun;
        body.surfaceTemperature = radius * 0.0085d;

        AddSolarBody(sol, body, body, position, radius, density, name); //add to solarsystem
        //Controller.Instantiate<SunIconController>("sunIcon", body);
        return body;
    }

    /// <summary>
    /// Add planet to Solar System around reference
    /// </summary>
    /// <param name="sol">Solar System</param>
    /// <param name="reference">Reference Body</param>
    /// <param name="radius">radius in m</param>
    /// <param name="LocalPosition"> position in relation to reference object in m</param>
    /// <param name="density">density of object</param>
    /// <param name="name">name of solar body</param>
    /// <returns></returns>
    public static PlanetModel AddPlanet(SolarSystemModel sol, SolarBodyModel reference, double radius, Vector3d LocalPosition, double density, string name)
    {
        //Calculate basic info
        Vector3d position = reference.position + LocalPosition;

        //Set basic info
        var body = new PlanetModel();
        body.type = ObjectType.Planet;
        body.reference = new ModelRef<SolarBodyModel>(reference);
        body.position = position;
        body.velocity = VelocityFromOrbit(body);

        AddSolarBody(sol, reference, body, position, radius, density, name); //add to solarsystem
        //Controller.Instantiate<PlanetIconController>("planetIcon", body);
        return body;
    }
   /// <summary>
    /// Add planet to Solar System around reference
    /// </summary>
    /// <param name="sol">Solar System</param>
    /// <param name="reference">Reference Body</param>
    /// <param name="radius">radius in m</param>
    /// <param name="LocalPosition"> position in relation to reference object in m</param>
    /// <param name="mass">mass of body in kg</param>
    /// <param name="name">name of solar body</param>
    /// <returns></returns>
    public static PlanetModel AddPlanetByMass(SolarSystemModel sol, SolarBodyModel reference, Vector3d LocalPosition, double radius, double mass, string name)
    {
        //Calculate basic info
        double density = mass / Forces.CircleArea(radius);

        var body = new PlanetModel();
        body.type = ObjectType.Planet;
        body.reference = new ModelRef<SolarBodyModel>(reference);
        body.LocalPosition = LocalPosition;
        body.velocity = VelocityFromOrbit(body);

        AddSolarBody(sol, reference, body, body.position, radius, density, name); //add to solarsystem
        //Controller.Instantiate<PlanetIconController>("planetIcon", body);
        return body;
    }
    /// <summary>
    /// Add a craft to a solar body
    /// </summary>
    /// <param name="planet">solar body object</param>
    /// <param name="angle"> global? angle on the solar object in RADIANS</param>
    /// <param name="name">name of craft</param>
    /// <returns></returns>
    public static CraftModel AddCraft(SolarSystemModel sol, SolarBodyModel planet, double angle, string name)
    {
        var body = new CraftModel(); //basic craft info
        body.type = ObjectType.Spacecraft;
        body.reference = new ModelRef<SolarBodyModel>(planet);
        body.sol = new ModelRef<SolarSystemModel>(sol);
        body.reference.Model.crafts.Add(body);
        body.name = name;
        body.mass = 7.5f;
        body.state = ObjectState.Landed;
        

        //craft position rotation info

        Polar2 polarRadius = new Polar2(planet.radius, angle);

        float displacement = FresNoise.GetTerrian(planet.name, polarRadius);
        Polar2 polarPosition = new Polar2(polarRadius.radius + displacement + 4, polarRadius.angle);

        body.surfacePolar = polarPosition; //Set surface position
        body.SurfaceVel = new Vector3d((planet.radius + displacement) * planet.rotationRate, 0, 0); //set surface velocity

        body.rotation = new Quaternion();
        //body.rotation.eulerAngles = new Vector3(0, 0, (float) angle * Mathf.Rad2Deg + planet.rotation.eulerAngles.z);

        sol.allCrafts.Add(body);
        //Controller.Instantiate<CraftIconController>("craftIcon", body);
        return body;
    }
    /// <summary>
    /// Add Craft in orbit around reference object
    /// </summary>
    /// <param name="sol">The Solor System model</param>
    /// <param name="reference"> the reference object</param>
    /// <param name="localPosition"> position above the centeor of reference object</param>
    /// <param name="name">name of craft</param>
    /// <returns></returns>
    public static CraftModel AddCraft(SolarSystemModel sol, SolarBodyModel reference, Vector3d localPosition, string name)
    {
        

        var body = new CraftModel(); //basic craft info
        body.type = ObjectType.Spacecraft;
        body.reference = new ModelRef<SolarBodyModel>(reference);
        body.sol = new ModelRef<SolarSystemModel>(sol);
        body.reference.Model.crafts.Add(body);

        body.name = name;
        body.state = ObjectState.Orbit;
        body.LocalPosition = localPosition;
        body.rotation = new Quaternion();
        body.mass = 7.5f;
        body.velocity = VelocityFromOrbit(body);

        sol.allCrafts.Add(body);

        return body;
    }

    /// <summary>
    /// Add Solar Body to Solar System model. Reference should already be
    /// </summary>
    /// <param name="sol">Solar system to add model to</param>
    /// <param name="reference"> The reference solar body, usually the sun or planet solar object orbits</param>
    /// <param name="body"> solar model object to add to solar system</param>
    /// <param name="position"> world position of model</param>
    /// <param name="radius"> radius of solar body</param>
    /// <param name="density"> density of solar body .1 to 10 ideal</param>
    /// <param name="surfacePressure"> surface pressure of body in atm</param>
    /// <param name="name"> name of solar body</param>
    private static void AddSolarBody(SolarSystemModel sol, SolarBodyModel reference, SolarBodyModel body, Vector3d position, double radius, double density, string name)
    {
        body.reference = new ModelRef<SolarBodyModel>(reference);
        body.sol = new ModelRef<SolarSystemModel>(sol);
        body.reference.Model.solarBodies.Add(body);
        body.position = position;                                     //set given info
        body.mass = density * Forces.CircleArea(radius);
        body.radius = radius;
        body.density = density;
        body.name = name;
        System.Random seed = new System.Random(name.GetHashCode());
        body.rotationRate = 360d / ((double) seed.Next(1, 50) * Date.Hour);
        body.color = new Color(seed.Next(0, 100) / 100f, seed.Next(0, 100) / 100f, seed.Next(0, 100) / 100f);

        if (sol.centerObject.Model == null)                                //set first center of mass object if not alreaady set
        {
            sol.centerObject = new ModelRef<SolarBodyModel>(body);
        }
        else if (body.mass > sol.centerObject.Model.mass)
        {                                                            //If new object created bigger, change center of sol
            sol.centerObject.Delete();
            sol.centerObject = new ModelRef<SolarBodyModel>(body);

            foreach (SolarBodyModel m in sol.allSolarBodies)
            {
                m.SOI = CalculateSOI(m);
            }
        }
        sol.allSolarBodies.Add(body);
        body.SOI = CalculateSOI(body);
    }

    /// <summary>
    /// Calculate Sphere of Influence based on centor solar body distance and mass
    /// </summary>
    /// <param name="m"> solar body to calculate SOI for</param>
    /// <returns></returns>
    private static double CalculateSOI(SolarBodyModel m) 
    {
        double r = Vector3d.Distance(m.position, m.reference.Model.position);
        double rSOI = r * Mathd.Pow(m.mass / m.reference.Model.mass, 0.4f);
        return rSOI;
    }
    /// <summary>
    /// Returns world velocity in the world postion to create an optimal circular orbit
    /// </summary>
    /// <param name="bModel">model to return velocity for</param>
    /// <returns></returns>
    public static Vector3d VelocityFromOrbit(BaseModel bModel)
    {
        Vector3d R = bModel.LocalPosition; // altitude from reference 
        double M2 = bModel.reference.Model.mass; //reference mass

        Vector3d vel = Mathd.Sqrt((Forces.G * M2) / bModel.polar.radius) * Forces.Tangent(R.normalized) + bModel.reference.Model.velocity;

        return vel;
    }
}
