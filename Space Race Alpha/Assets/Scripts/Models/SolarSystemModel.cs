using UnityEngine;
using System.Collections;
using CodeControl;

public class SolarSystemModel : Model
{
    //------------Public Variables-----------------------//
    public string name;
    public string seed;
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

    //------------Constructors-----------------------//

    public SolarSystemModel() { }

    public SolarSystemModel(int planets)
    {

        SunModel sun = AddSun(Units.Mm, 1.25, "Sun");

        int numberPlants = UnityEngine.Random.Range(4, 10);

        float minPlanetRadius = 10; //in km 
        float maxSolarDistance = 150; //in Gm
        float minSolarDistance = 2; //in Gm

        for (int i = 0; i < planets; i++)
        {
            double planetsize = UnityEngine.Random.Range(minPlanetRadius, 100) * Units.km;
            Polar2 planetPol = new Polar2(UnityEngine.Random.Range(minSolarDistance, maxSolarDistance) * Units.Gm, 
                UnityEngine.Random.Range(0, 2 * Mathf.PI));
            double density = UnityEngine.Random.Range(.1f, 10);

            PlanetModel planet = AddPlanet(sun, planetPol, planetsize, density, "Planet " + i.ToString());

            int numberMoons = UnityEngine.Random.Range(0, 10);

            for (int b = 0; b < numberMoons; b++)
            {
                double moonSize = UnityEngine.Random.Range(10, (float) (planet.radius / Units.km)) * Units.km; //in km
                Polar2 moonPol = new Polar2(UnityEngine.Random.Range(10, (float) (planet.SOI / Units.km)) * Units.km,
                    UnityEngine.Random.Range(0, 2 * Mathf.PI));
                density = UnityEngine.Random.Range(.1f, 10);

                AddPlanet(planet, moonPol, moonSize, density, "Planet " + i.ToString() + ": Moon " + b.ToString());
            }
        }
    }

    //------------Functions-----------------------//

    /// <summary>
    /// Add sun to the Solar System, only want to add one per sol
    /// </summary>
    /// <param name="sol">Solar system to add sun to</param>
    /// <param name="radius">Radius of sun</param>
    /// <param name="density">Density of sun, usually between .1 and .5</param>
    /// <param name="name">Name of sun</param>
    /// <returns></returns>
    public SunModel AddSun(double radius, double density, string name)
    {
        //Calculate basic info
        Vector3d position = Vector3d.zero;
        double surfaceTemp = radius * 0.0085;

        //Create Model
        var body = new SunModel(this, null, position, radius, density, surfaceTemp, name);

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
    public SunModel AddSunByMass(SolarSystemModel sol, double radius, double mass, string name)
    {

        //Calculate basic info
        Vector3d position = Vector3d.zero;
        double density = mass / Forces.SphereVolume(radius);

        return AddSun(radius, density, name);
    }

    /// <summary>
    /// Add planet to Solar System around reference
    /// </summary>
    /// <param name="sol">Solar System</param>
    /// <param name="reference">Reference Body</param>
    /// <param name="pol">position information in relation to the reference object</param>
    /// <param name="density">density of object</param>
    /// <param name="name">name of solar body</param>
    /// <returns></returns>
    public PlanetModel AddPlanet(SolarBodyModel reference, Polar2 pol, double radius,  double density, string name)
    {
        //Calculate basic info
        Vector3d position = reference.SystemPosition + (Vector3d) pol.cartesian;

        //Set basic info
        var body = new PlanetModel(this, reference,position,radius,density,name);
        body.velocity = VelocityFromOrbit(body);

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
    public PlanetModel AddPlanetByMass(SolarBodyModel reference, Polar2 pol, double radius, double mass, string name)
    {
        //Calculate basic info
        double density = mass / Forces.SphereVolume(radius);

        return AddPlanet(reference, pol, radius, density, name);
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
