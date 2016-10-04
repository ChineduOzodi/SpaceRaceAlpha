using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;

public class SolarSystemController : Controller<SolarSystemModel>
{

    //Center of Mass information
    public Vector3 centerOfMass;
    public float totalMass;
    public Vector3 centerOfMassPotential;

    protected override void OnInitialize()
    {
        //Message.AddListener<AddSolarBodyMessage>();
        model.allSolarBodies = new ModelRefs<SolarBodyModel>();
        model.allCrafts = new ModelRefs<CraftModel>();
    }

    /// <summary>
    /// Create Sun object at center of world position
    /// </summary>
    /// <param name="radius"> radius of sun in m</param>
    /// <param name="name"> sun name</param>
    /// <param name="density">sun density</param>
    /// <returns></returns>
    public SunController AddSun(float radius,  string name = "Sun", float density = .25f)
    {
        //Calculate basic info
        Vector3 position = Vector3.zero;
        float mass = Forces.CircleArea(radius) * density;

        //Create Model
        var body = new SunModel();
        body.type = ObjectType.Sun;
        body.density = density;
        body.velocity = Vector3.zero;
        body.localScale = radius * Vector3.one;
        body.reference = new ModelRef<SolarBodyModel>(body);

        AddSolarBody(body, position, radius, mass, name); //add to solarsystem

        return Controller.Instantiate<SunController>("sun", body);
    }

    public PlanetController AddSun(float radius, float mass, string name = "Sun")
    {

        //Calculate basic info
        Vector3 position = Vector3.zero;
        float density = mass / Forces.CircleArea(radius);

        //Create Model
        var body = new PlanetModel();
        body.type = ObjectType.Sun;
        body.density = density;
        body.velocity = Vector3.zero;
        body.relVel = Vector3.zero;
        body.localScale = radius * Vector3.one;
        body.reference = new ModelRef<SolarBodyModel>(body);

        AddSolarBody(body, position, radius, mass, name); //add to solarsystem

        return Controller.Instantiate<PlanetController>("sun", body);
    }

    public PlanetController AddPlanet(SolarBodyModel reference, float radius, Vector3 altitude, float density = 1f, string name = "Planet")
    {
        //Calculate basic info
        Vector3 position = reference.position + altitude;
        float mass = Forces.CircleArea(radius) * density;

        //Set basic info
        var body = new PlanetModel();
        body.type = ObjectType.Planet;
        body.reference = new ModelRef<SolarBodyModel>(reference);
        body.position = position;
        body.velocity = VelocityFromOrbit(body);
        body.relVel = body.velocity - body.reference.Model.velocity;

        AddSolarBody(body, position, radius, mass, name); //add to solarsystem

        return Controller.Instantiate<PlanetController>("planet", body);
    }

    public PlanetController AddPlanet(SolarBodyModel reference, Vector3 position, float radius, float mass, string name = "Planet")
    {
        //Calculate basic info
        float density = mass / Forces.CircleArea(radius);

        var body = new PlanetModel();
        body.type = ObjectType.Planet;
        body.density = density;
        body.reference = new ModelRef<SolarBodyModel>(reference);

        AddSolarBody(body, position, radius, mass, name); //add to solarsystem

        return Controller.Instantiate<PlanetController>("planet", body);
    }
    /// <summary>
    /// Add a craft to a solar body
    /// </summary>
    /// <param name="planet">solar body object</param>
    /// <param name="angle"> global? angle on the solar object in RADIANS</param>
    /// <param name="name">name of craft</param>
    /// <returns></returns>
    public CraftController AddCraft(SolarBodyModel planet, float angle, string name = "Craft")
    {
        var body = new CraftModel(); //basic craft info
        body.type = ObjectType.Spacecraft;
        body.reference = new ModelRef<SolarBodyModel>(planet);
        body.name = name;
        body.mass = 7.5f;
        body.state = ObjectState.Landed;

        //craft position rotation info
        var pos = Forces.PolarToCartesian(new Vector2(planet.radius, angle + planet.rotation.eulerAngles.z * Mathf.Deg2Rad));
        body.position = new Vector3(pos.x,pos.y) + planet.position;
        body.rotation = new Quaternion();
        body.rotation.eulerAngles = new Vector3(0,0,angle * Mathf.Rad2Deg + planet.rotation.eulerAngles.z - 90);

        model.allCrafts.Add(body);

        return Controller.Instantiate<CraftController>("rocket", body);
    }
    public CraftController AddCraft(SolarBodyModel reference, Vector3 position, string name = "Craft")
    {
        var body = new CraftModel(); //basic craft info
        body.type = ObjectType.Spacecraft;
        body.reference = new ModelRef<SolarBodyModel>(reference);
        body.name = name;
        body.state = ObjectState.Orbit;
        body.position = reference.position + position;
        body.rotation = new Quaternion();
        body.mass = 7.5f;
        body.velocity = VelocityFromOrbit(body);
        body.relVel = body.velocity - body.reference.Model.velocity;

        model.allCrafts.Add(body);

        return Controller.Instantiate<CraftController>("rocket", body);
    }

    private void AddSolarBody(SolarBodyModel body,  Vector3 position, float radius, float mass, string name)
    {

        //Calculate info
        float density = mass / Forces.CircleArea(radius);

        body.position = position; //set given info
        body.mass = mass;
        body.radius = radius;
        body.density = density;
        body.name = name;

        if (model.centerObject == null) //set first center of mass object if not alreaady set
        {
            model.centerObject = new ModelRef<SolarBodyModel>(body);
        }
        else if (body.mass > model.centerObject.Model.mass) { //If new object created beg
            model.centerObject.Delete();
            model.centerObject = new ModelRef<SolarBodyModel>(body);

            foreach (SolarBodyModel m in model.allSolarBodies)
            {
                m.SOI = CalculateSOI(m);
            }
        }
        model.allSolarBodies.Add(body);
        body.SOI = CalculateSOI(body);
        CalculateCM();
    }

    private void CalculateCM() //calculate centor of mass for solar system
    {
        foreach(SolarBodyModel body in model.allSolarBodies)
        {
            centerOfMass = Vector3.zero;
            centerOfMassPotential = Vector3.zero;
            totalMass = 0;

            totalMass += body.mass;

            centerOfMassPotential += body.position;
        }
    }

    private float CalculateSOI(SolarBodyModel m) //Calculate Sphere of Influence based on centor solar body distance and mass
    {
        float r = Vector3.Distance(m.position, m.reference.Model.position);
        float rSOI = r * Mathf.Pow(m.mass / m.reference.Model.mass, 0.4f);
        return rSOI;
    }
    /// <summary>
    /// Returns velocity in the world postion to create an optimal circular orbit
    /// </summary>
    /// <param name="bModel">model to return velocity for</param>
    /// <returns></returns>
    public static Vector3 VelocityFromOrbit(BaseModel bModel)
    {
        Vector3 R = bModel.position - bModel.reference.Model.position; // altitude from reference 
        float M2 = bModel.reference.Model.mass ; //reference mass

        Vector3 vel = Mathf.Sqrt((Forces.G * M2) / R.magnitude) * Forces.Tangent(R.normalized) + bModel.reference.Model.velocity;

        return vel;
    } 

    void FixedUpdate()
    {
        foreach (SolarBodyModel body in model.allSolarBodies)
        {
            Vector3 force = Forces.Force(body);
            body.force = force;
            body.relVel += Forces.ForceToVelocity(body);
            body.velocity = body.relVel + body.reference.Model.velocity;
            body.position = Forces.VelocityToPosition(body);
            body.NotifyChange();
            //rb2D.AddForce(force * Time.deltaTime);

            if (model.showForce)
            {
                model.showForce = false;

                ShowForceMessage m = new ShowForceMessage();
                m.color = Color.red;
                //m.parent = new ModelRef<PlanetModel>(model);
                Message.Send(m);
            }
        }
        foreach (CraftModel body in model.allCrafts)
        {
            if (body.state != ObjectState.Landed)
            {
                Vector3 force = Forces.Force(body, model.allSolarBodies);
                //Vector3 relForce = Forces.PolarToCartesian(new Vector2(body.throttle, body.rotation.eulerAngles.z * Mathf.Deg2Rad));
                //model.force += relForce;
                //Vector3 force = Forces.Force(body) + relForce;
                //Vector3 force = Forces.Force(body);
                body.force = force; //+ Forces.Force(body.reference.Model);
                body.relVel += Forces.ForceToVelocity(force,body.mass);
                //body.velocity = body.relVel + body.reference.Model.velocity;
                //body.position = Forces.VelocityToPosition(body);
                //body.NotifyChange();
            }

            //Check for SOI change
            float closestBody = (body.position - body.reference.Model.position).magnitude;

            for (int i = 0; i < model.allSolarBodies.Count; i++)
            {
                SolarBodyModel solarMod = model.allSolarBodies[i];
                float distance = (body.position - solarMod.position).magnitude;
                if ( distance < closestBody && solarMod.SOI > distance)
                {
                    body.reference.Delete();
                    body.reference = new ModelRef<SolarBodyModel>(solarMod);
                }
            }
        }
    }

   
}
