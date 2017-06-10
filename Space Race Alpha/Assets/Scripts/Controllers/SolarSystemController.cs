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

    protected override void OnInitialize()
    {
        //Message.AddListener<AddSolarBodyMessage>();

        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraController>();

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
            Time.timeScale += 10;
            MessagePanel.SendMessage("Time Accel: " + Time.timeScale.ToString(), 5, Color.yellow);
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            Time.timeScale = 1f;
            MessagePanel.SendMessage("Time Accel: " + Time.timeScale.ToString(), 5, Color.white);
        }
        //Update Forces
        foreach (SolarBodyModel body in model.allSolarBodies)
        {
            Vector3d force = Forces.Force(body,model.allSolarBodies);
            body.force = force;
            body.velocity += Forces.ForceToVelocity(body, Time.deltaTime);
            body.SystemPosition += Forces.VelocityToPosition(body, Time.deltaTime);

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
            body.CraftControl(Time.deltaTime);
        }

        if (model.localReferencePoint != null)
        {
            transform.position = (Vector3) (model.localReferencePoint / (Units.km));
        }
    }

   
}
