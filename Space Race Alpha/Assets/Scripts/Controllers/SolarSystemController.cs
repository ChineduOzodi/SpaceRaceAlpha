using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class SolarSystemController : Controller<SolarSystemModel>
{
    internal Text messageText;
    protected override void OnInitialize()
    {
        //Message.AddListener<AddSolarBodyMessage>();
        messageText = GameObject.FindGameObjectWithTag("messageText").GetComponent<Text>();
        messageText.text = "1";
    }

    void Update()
    {
        //UPdate timeScale
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Time.timeScale *= 2;
            messageText.text = Time.timeScale.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            Time.timeScale *= .5f;
            messageText.text = Time.timeScale.ToString();
        }
        //Update Forces
        foreach (SolarBodyModel body in model.allSolarBodies)
        {
            Vector3d force = Forces.Force(body);
            body.force = force;
            body.LocalVelocity += Forces.ForceToVelocity(body);
            body.position = Forces.VelocityToPosition(body);

            body.Rotation += body.RotationRate * Time.deltaTime; //Rotate the planet

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
        foreach (CraftModel body in model.allCrafts)                                            //set craft forces and locations when applicable
        {
            Vector3d force = Forces.Force(body);
            body.force = force;
            if (!body.spawned) //Sets craft that are not spawned
            {
                if (body.state != ObjectState.Landed)
                {
                    body.LocalVelocity += Forces.ForceToVelocity(force, body.mass);
                    body.position = Forces.VelocityToPosition(body);
                }
                else
                {
                    body.LocalPosition = body.LocalPosition;
                    body.SurfaceVel = body.SurfaceVel;

                }
            }
            else //Spwned craft are controlled by the rigidbody2D system
            {

            }

            //Check for SOI change
            double closestBody = (body.position - body.reference.Model.position).magnitude;

            for (int i = 0; i < model.allSolarBodies.Count; i++)
            {
                SolarBodyModel solarMod = model.allSolarBodies[i];
                double distance = (body.position - solarMod.position).magnitude;
                if ( distance < closestBody && solarMod.SOI > distance)
                {
                    body.reference.Delete();
                    body.reference = new ModelRef<SolarBodyModel>(solarMod);
                    body.position = body.position;
                    body.velocity = body.velocity;
                }
            }
        }

        if (model.localReferencePoint != null)
        {
            transform.position = (Vector3) (model.localReferencePoint / Units.km);
        }
    }

   
}
