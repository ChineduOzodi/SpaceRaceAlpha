using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class SolarSystemController : Controller<SolarSystemModel>
{

    Camera mainCam;
    CameraController cam;
    Text dateTime;
    public float accel = 1;

    protected override void OnInitialize()
    {
        //Message.AddListener<AddSolarBodyMessage>();

        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraController>();
        dateTime = GameObject.FindGameObjectWithTag("date").GetComponent<Text>();
        cam.controlMode = ControlMode.Free;

        //-----------Instantiate all solar and craft icons----------//

        foreach (SolarBodyModel body in model.allSolarBodies)
        {
            if (body.Type == ObjectType.Planet)
            {
                Controller.Instantiate<PlanetIconController>("planetIcon", body);
            }
            else if (body.Type == ObjectType.Sun)
            {
                Controller.Instantiate<SunIconController>("sunIcon", body);
            }
        }

        foreach (CraftModel body in model.allCrafts)
        {
            Controller.Instantiate<CraftIconController>("craftIcon", body);
        }
    }

    void Update()
    {
        //Update timeScale
        if (Input.GetKeyDown(KeyCode.Period))
        {
            accel *= 2;
            MessagePanel.SendMessage("Time Accel: " + accel, 5, Color.yellow);
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            accel = 1f;
            MessagePanel.SendMessage("Time Accel: " + accel, 5, Color.white);
        }

        //Update Date
        model.date.AddTime(Time.deltaTime * accel);
        dateTime.text = model.date.GetDateTime();
        //Update Forces
        foreach (SolarBodyModel body in model.allSolarBodies)
        {
            Vector3d force = Forces.Force(body, true);
            body.force = force;
            body.velocity += Forces.ForceToVelocity(body, model.date.deltaTime);
            body.SystemPosition += Forces.VelocityToPosition(body, model.date.deltaTime);
            //body.LocalPositionKeplar(model.date.deltaTime);


            body.Rotation += body.RotationRate * model.date.deltaTime; //Rotate the planet

            body.NotifyChange();

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
            body.CraftControl(model.date.deltaTime);
        }

        if (model.localReferencePoint != null)
        {
            transform.position = (Vector3) (model.localReferencePoint / (Units.km));
        }
    }

   
}
