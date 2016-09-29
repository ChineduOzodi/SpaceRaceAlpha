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

    public void AddSun(Vector3 position, float radius, float mass, string name = "Sun")
    {
        var body = new PlanetModel();
        body.type = ObjectType.Sun;

        AddSolarBody(body, position, radius, mass, name); //add to solarsystem

        Controller.Instantiate<PlanetController>("sun", body);
    }
    public void AddPlanet(Vector3 position, float radius, float mass, string name = "Planet")
    {
        var body = new PlanetModel();
        body.type = ObjectType.Planet;

        AddSolarBody(body, position, radius, mass, name); //add to solarsystem

        Controller.Instantiate<PlanetController>("planet",body);
    }

    public CraftController AddCraft(SolarBodyModel planet, float angle, string name = "Craft")
    {
        var body = new CraftModel(); //basic craft info
        body.type = ObjectType.Spacecraft;
        body.reference = new ModelRef<SolarBodyModel>(planet);

        //craft position rotation info
        var pos = Forces.PolarToCartesian(new Vector2(planet.radius, angle + planet.rotation.eulerAngles.z * Mathf.Deg2Rad));
        body.position = new Vector3(pos.x,pos.y);
        body.rotation = new Quaternion();
        body.rotation.eulerAngles = new Vector3(0,0,angle * Mathf.Rad2Deg + planet.rotation.eulerAngles.z - 90);

        model.allCrafts.Add(body);

        return Controller.Instantiate<CraftController>("rocket", body);
    }

    private void AddSolarBody(SolarBodyModel body,  Vector3 position, float radius, float mass, string name)
    {

        body.position = position; //set given info
        body.mass = mass;
        body.radius = radius;
        body.name = name;
        body.velocity = Vector3.zero;

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
        float r = Vector3.Distance(m.position, model.centerObject.Model.position);
        float rSOI = r * Mathf.Pow(m.mass / model.centerObject.Model.mass, 0.4f);
        return rSOI;
    }

    void Update()
    {
        foreach (SolarBodyModel body in model.allSolarBodies)
        {
            Vector3 force = Forces.Force(body, model.allSolarBodies);
            body.force = force;
            body.velocity = Forces.ForceToVelocity(body);
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
            Vector3 force = Forces.Force(body, model.allSolarBodies);
            body.force = force;
            //

            if (model.showForce)
            {
                model.showForce = false;

                ShowForceMessage m = new ShowForceMessage();
                m.color = Color.red;
                //m.parent = new ModelRef<PlanetModel>(model);
                Message.Send(m);
            }
        }
    }

   
}
